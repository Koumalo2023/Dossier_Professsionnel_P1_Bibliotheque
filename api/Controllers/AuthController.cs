using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Errors);
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return result.Success
                ? Ok(new { token = result.Data })
                : Unauthorized(result.Errors);
        }

        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _authService.GetCurrentUserAsync();
            return Ok(user);
        }

        // GET /api/auth/users
        [HttpGet("users")]
        //[Authorize(Roles = "Admin")] // Autoriser uniquement les administrateurs
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsersAsync();
            return result.Success ? Ok(result.Data) : BadRequest(result.Errors);
        }

        // DELETE /api/auth/users/{userId}
        [HttpDelete("users/{userId}")]
        //[Authorize(Roles = "Admin")] // Autoriser uniquement les administrateurs
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _authService.DeleteUserAsync(userId);
            return result.Success ? Ok(result.Message) : BadRequest(result.Errors);
        }

        // PUT /api/auth/users/{userId}
        [HttpPut("users/{userId}")]
        //[Authorize(Roles = "Admin")] // Autoriser uniquement les administrateurs
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _authService.UpdateUserAsync(userId, updateUserDto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Errors);
        }

    }
}
