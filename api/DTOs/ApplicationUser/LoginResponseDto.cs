namespace api.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}
