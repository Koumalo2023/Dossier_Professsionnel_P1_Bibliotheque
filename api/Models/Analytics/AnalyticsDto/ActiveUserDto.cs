using System;

namespace api.Models
{
    /// <summary>
    /// DTO pour les utilisateurs les plus actifs
    /// </summary>
    public class ActiveUserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public int LoanCount { get; set; }
    }
}