namespace api.DTOs
{
    /// <summary>
    /// DTO pour transférer les données d'un utilisateur.
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
