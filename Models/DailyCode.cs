namespace MyWheelApp.Models
{
    public class DailyCode
    {
        public string Code { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? UsedBy { get; set; }
    }

    public class DailyCodeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }

    public class DailyCodeResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public int RemainingUses { get; set; }
    }
}