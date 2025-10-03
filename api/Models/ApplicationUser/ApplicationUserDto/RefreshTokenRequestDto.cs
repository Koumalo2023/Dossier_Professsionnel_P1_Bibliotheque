using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
