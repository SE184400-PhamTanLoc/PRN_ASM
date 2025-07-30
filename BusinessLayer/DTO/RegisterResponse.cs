namespace BusinessLayer.DTO
{
    public class RegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
} 