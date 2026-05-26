namespace ShoesShop.Application.DTOs
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? FullName { get; set; }
    }
}