namespace api.Models
{
    /// <summary>
    /// DTO pour transférer les données d'une notification.
    /// </summary>
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "INFO";
        public bool Read { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
