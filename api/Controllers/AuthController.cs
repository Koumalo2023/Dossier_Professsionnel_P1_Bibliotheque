using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public AuthController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            return result.Success
                ? Ok(new
                {
                    success = true,
                    message = result.Message,
                    user = result.User // Ajout des données utilisateur créé
                })
                : BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto,
            [FromHeader(Name = "X-Forwarded-For")] string ipAddress)
        {
            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                var authResult = await _authService.LoginAsync(loginDto, ipAddress);

                var response = new
                {
                    token = authResult.Token,
                    refreshToken = authResult.RefreshToken,
                    tokenExpires = authResult.TokenExpires.ToString("o"),
                    user = new
                    {
                        id = authResult.User.Id,
                        name = authResult.User.Name,
                        email = authResult.User.Email,
                        role = authResult.User.Roles
                    }
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    error = "authentication_failed",
                    message = "Email ou mot de passe incorrect",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "server_error",
                    message = "Erreur lors de l'authentification",
                    details = _env.IsDevelopment() ? ex.Message : null
                });
            }
        }


        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _authService.GetCurrentUserAsync();
            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Roles,
                createdAt = user.CreatedAt.ToString("o"),
                updatedAt = user.UpdatedAt.ToString("o")
            });
        }

        // GET /api/auth/users
        [HttpGet("users")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsersAsync();

            return result.Success
                ? Ok(result.Data.Select(u => new {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    role = u.Roles,
                    createdAt = u.CreatedAt.ToString("o"),
                    updatedAt = u.UpdatedAt.ToString("o")
                }).ToList())
                : BadRequest(new { errors = result.Errors });
        }

        // DELETE /api/auth/users/{userId}
        [HttpDelete("users/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _authService.DeleteUserAsync(userId);

            return result.Success
                ? Ok(new
                {
                    success = true,
                    message = result.Message
                })
                : BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
        }

        // PUT /api/auth/users/{userId}
        [HttpPut("users/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _authService.UpdateUserAsync(userId, updateUserDto);

            return result.Success
                ? Ok(new
                {
                    success = true,
                    message = result.Message,
                    user = result.UpdatedUser
                })
                : BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
        }

        // AuthController.cs
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
    [FromBody] RefreshTokenRequestDto request,
    [FromHeader(Name = "X-Forwarded-For")] string ipAddress)
        {
            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                var response = await _authService.RefreshTokenAsync(request, ipAddress);
                return Ok(new
                {
                    token = response.Token,
                    refreshToken = response.RefreshToken,
                    tokenExpires = response.TokenExpires.ToString("o")
                });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new
                {
                    error = "invalid_token",
                    message = ex.Message
                });
            }
        }

    }
}
