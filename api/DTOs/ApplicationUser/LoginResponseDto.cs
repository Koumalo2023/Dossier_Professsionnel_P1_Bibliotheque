using api.DTOs.ApplicationUser;

namespace api.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public UserDto User { get; set; } = new UserDto();
    }
}
