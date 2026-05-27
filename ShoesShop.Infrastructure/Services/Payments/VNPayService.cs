using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ShoesShop.Application.Interfaces.Payments;

namespace ShoesShop.Infrastructure.Services.Payments;

public class VNPayService : IVNPayService
{
    private readonly IConfiguration _configuration;

    public VNPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CreatePaymentResult CreatePaymentUrl(CreatePaymentRequest request)
    {
        var tmnCode = _configuration["Payment:VNPay:TmnCode"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:TmnCode.");

        var hashSecret = _configuration["Payment:VNPay:HashSecret"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:HashSecret.");

        var paymentUrl = _configuration["Payment:VNPay:PaymentUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:PaymentUrl.");

        var returnUrl = _configuration["Payment:VNPay:ReturnUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:ReturnUrl.");

        var ipnUrl = _configuration["Payment:VNPay:IpnUrl"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:IpnUrl.");

        var requestId = request.OrderCode;

        var vnpParams = new SortedDictionary<string, string>
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(CultureInfo.InvariantCulture),
            ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
            ["vnp_CurrCode"] = "VND",
            ["vnp_IpAddr"] = string.IsNullOrWhiteSpace(request.ClientIpAddress) ? "127.0.0.1" : request.ClientIpAddress,
            ["vnp_Locale"] = "vn",
            ["vnp_OrderInfo"] = request.OrderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_TxnRef"] = requestId,
            ["vnp_ExpireDate"] = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss")
        };

        var queryString = BuildQueryString(vnpParams);
        var secureHash = HmacSha512(hashSecret, queryString);

        var url = $"{paymentUrl}?{queryString}&vnp_SecureHash={secureHash}";

        return new CreatePaymentResult
        {
            RequestId = requestId,
            PaymentUrl = url,
            RawResponse = JsonSerializer.Serialize(new
            {
                provider = "VNPay",
                requestId,
                ipnUrl
            })
        };
    }

    public PaymentCallbackResult VerifyCallback(Dictionary<string, string> queryParams)
    {
        var hashSecret = _configuration["Payment:VNPay:HashSecret"]
            ?? throw new InvalidOperationException("Thiếu Payment:VNPay:HashSecret.");

        if (!queryParams.TryGetValue("vnp_SecureHash", out var receivedHash))
        {
            return new PaymentCallbackResult
            {
                IsValidSignature = false,
                RawData = JsonSerializer.Serialize(queryParams)
            };
        }

        var filtered = queryParams
            .Where(x =>
                !string.Equals(x.Key, "vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(x.Key, "vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.Key, x => x.Value);

        var sorted = new SortedDictionary<string, string>(filtered);

        var signData = BuildQueryString(sorted);
        var calculatedHash = HmacSha512(hashSecret, signData);

        var isValid = string.Equals(receivedHash, calculatedHash, StringComparison.OrdinalIgnoreCase);

        var responseCode = queryParams.GetValueOrDefault("vnp_ResponseCode");
        var transactionStatus = queryParams.GetValueOrDefault("vnp_TransactionStatus");

        return new PaymentCallbackResult
        {
            IsValidSignature = isValid,
            IsSuccess = isValid && responseCode == "00" && transactionStatus == "00",
            OrderCode = queryParams.GetValueOrDefault("vnp_TxnRef") ?? string.Empty,
            TransactionCode = queryParams.GetValueOrDefault("vnp_TransactionNo"),
            ResponseCode = responseCode,
            TransactionStatus = transactionStatus,
            RawData = JsonSerializer.Serialize(queryParams)
        };
    }

    private static string BuildQueryString(SortedDictionary<string, string> data)
    {
        return string.Join("&", data
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{WebUtility.UrlEncode(x.Key)}={WebUtility.UrlEncode(x.Value)}"));
    }

    private static string HmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);

        using var hmac = new HMACSHA512(keyBytes);

        var hashBytes = hmac.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
