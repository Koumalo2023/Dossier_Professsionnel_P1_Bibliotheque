namespace api.DTOs
{
    /// <summary>
    /// DTO pour transférer les données d'une notification.
    /// </summary>
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
