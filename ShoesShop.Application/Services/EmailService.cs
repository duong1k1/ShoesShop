using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShoesShop.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp; // Đảm bảo sử dụng đúng thư viện SmtpClient của MailKit

namespace ShoesShop.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        // Hàm khởi tạo chuẩn (Constructor)
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string stringSubject, string body)
        {
            var email = new MimeMessage();

            // Lấy thông tin cấu hình từ appsettings.json
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = stringSubject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            var host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");

            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            // Đăng nhập bằng App Password cấu hình từ file json
            await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}