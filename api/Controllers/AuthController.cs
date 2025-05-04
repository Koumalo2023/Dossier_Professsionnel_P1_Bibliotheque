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
                    user = result.User 
                })
                : BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var authResult = await _authService.LoginAsync(loginDto);

                // Vérification null supplémentaire
                if (authResult?.User == null)
                {
                    return BadRequest(new { error = "user_data_missing", message = "Données utilisateur manquantes" });
                }

                return Ok(new
                {
                    token = authResult.Token,
                    tokenExpires = authResult.TokenExpires.ToString("o"),
                    user = new
                    {
                        id = authResult.User.Id,
                        name = authResult.User.Name,
                        email = authResult.User.Email,
                        role = authResult.User.Roles
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = "auth_failed", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "server_error", message = ex.Message });
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


        [HttpPost("users/{userId}/roles")]
        [Authorize(Roles = "Admin,Manager")] 
        public async Task<IActionResult> AddRoleAsync(string userId, [FromBody] AddRoleDto addRoleDto)
        {
            try
            {
                var result = await _authService.AddRoleAsync(userId, addRoleDto.Role);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "server_error",
                    message = ex.Message
                });
            }
        }

        [HttpDelete("users/{userId}/roles/{role}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> RemoveRoleAsync(string userId, string role)
        {
            try
            {
                var result = await _authService.RemoveRoleAsync(userId, role);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "server_error",
                    message = ex.Message
                });
            }
        }
    }
}
