using System;

namespace api.Models 
{
    /// <summary>
    /// DTO pour les logs d'audit (Analytics)
    /// </summary>
    public class AnalyticsAuditLogDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public string? EntityId { get; set; }
        public string Details { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string IpAddress { get; set; } = null!;
    }
}