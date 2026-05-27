using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ShoesShop.Application.Interfaces.Payments;

namespace ShoesShop.Infrastructure.Services.Payments;

public class MomoService : IMomoService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public MomoService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<CreatePaymentResult> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var partnerCode = _configuration["Payment:Momo:PartnerCode"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:PartnerCode.");

        var accessKey = _configuration["Payment:Momo:AccessKey"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:AccessKey.");

        var secretKey = _configuration["Payment:Momo:SecretKey"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:SecretKey.");

        var paymentUrl = _configuration["Payment:Momo:PaymentUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:PaymentUrl.");

        var redirectUrl = _configuration["Payment:Momo:RedirectUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:RedirectUrl.");

        var ipnUrl = _configuration["Payment:Momo:IpnUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:IpnUrl.");

        var requestType = _configuration["Payment:Momo:RequestType"] ?? "captureWallet";

        var requestId = Guid.NewGuid().ToString("N");
        var orderId = request.OrderCode;
        var amount = ((long)request.Amount).ToString();
        var extraData = "";

        var rawSignature =
            $"accessKey={accessKey}" +
            $"&amount={amount}" +
            $"&extraData={extraData}" +
            $"&ipnUrl={ipnUrl}" +
            $"&orderId={orderId}" +
            $"&orderInfo={request.OrderInfo}" +
            $"&partnerCode={partnerCode}" +
            $"&redirectUrl={redirectUrl}" +
            $"&requestId={requestId}" +
            $"&requestType={requestType}";

        var signature = HmacSha256(secretKey, rawSignature);

        var body = new
        {
            partnerCode,
            accessKey,
            requestId,
            amount,
            orderId,
            orderInfo = request.OrderInfo,
            redirectUrl,
            ipnUrl,
            extraData,
            requestType,
            signature,
            lang = "vi"
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(paymentUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(responseContent);
        var root = document.RootElement;

        var payUrl = root.TryGetProperty("payUrl", out var payUrlElement)
            ? payUrlElement.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(payUrl))
            throw new InvalidOperationException("MoMo không trả về payUrl.");

        return new CreatePaymentResult
        {
            RequestId = requestId,
            PaymentUrl = payUrl,
            RawResponse = responseContent
        };
    }

    public PaymentCallbackResult VerifyCallback(Dictionary<string, string> data)
    {
        var secretKey = _configuration["Payment:Momo:SecretKey"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:SecretKey.");

        var accessKey = _configuration["Payment:Momo:AccessKey"]
            ?? throw new InvalidOperationException("Thiếu Payment:Momo:AccessKey.");

        if (!data.TryGetValue("signature", out var receivedSignature))
        {
            return new PaymentCallbackResult
            {
                IsValidSignature = false,
                RawData = JsonSerializer.Serialize(data)
            };
        }

        var rawSignature =
            $"accessKey={accessKey}" +
            $"&amount={data.GetValueOrDefault("amount")}" +
            $"&extraData={data.GetValueOrDefault("extraData")}" +
            $"&message={data.GetValueOrDefault("message")}" +
            $"&orderId={data.GetValueOrDefault("orderId")}" +
            $"&orderInfo={data.GetValueOrDefault("orderInfo")}" +
            $"&orderType={data.GetValueOrDefault("orderType")}" +
            $"&partnerCode={data.GetValueOrDefault("partnerCode")}" +
            $"&payType={data.GetValueOrDefault("payType")}" +
            $"&requestId={data.GetValueOrDefault("requestId")}" +
            $"&responseTime={data.GetValueOrDefault("responseTime")}" +
            $"&resultCode={data.GetValueOrDefault("resultCode")}" +
            $"&transId={data.GetValueOrDefault("transId")}";

        var calculatedSignature = HmacSha256(secretKey, rawSignature);

        var isValid = string.Equals(receivedSignature, calculatedSignature, StringComparison.OrdinalIgnoreCase);
        var resultCode = data.GetValueOrDefault("resultCode");

        return new PaymentCallbackResult
        {
            IsValidSignature = isValid,
            IsSuccess = isValid && resultCode == "0",
            OrderCode = data.GetValueOrDefault("orderId") ?? string.Empty,
            TransactionCode = data.GetValueOrDefault("transId"),
            ResponseCode = resultCode,
            TransactionStatus = data.GetValueOrDefault("message"),
            RawData = JsonSerializer.Serialize(data)
        };
    }

    private static string HmacSha256(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);

        using var hmac = new HMACSHA256(keyBytes);

        var hashBytes = hmac.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}