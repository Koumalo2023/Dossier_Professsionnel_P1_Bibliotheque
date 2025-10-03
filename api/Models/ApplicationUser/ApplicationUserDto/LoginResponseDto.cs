

namespace api.Models
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public UserDto User { get; set; } = new UserDto();
    }
}
