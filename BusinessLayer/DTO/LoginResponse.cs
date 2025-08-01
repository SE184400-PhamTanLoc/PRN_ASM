namespace BusinessLayer.DTO
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
} 