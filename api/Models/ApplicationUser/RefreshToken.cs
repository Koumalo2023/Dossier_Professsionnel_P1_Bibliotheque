namespace api.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } 
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => !IsExpired;

        // Relation avec l'utilisateur
        public ApplicationUser User { get; set; }
    }
}
