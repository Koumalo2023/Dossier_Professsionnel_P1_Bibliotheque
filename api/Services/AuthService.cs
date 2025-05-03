using api.DTOs;
using api.DTOs.ApplicationUser;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Services
{
    public interface IAuthService
    {
        // Enregistre un nouvel utilisateur
        Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto);

        // Connecte un utilisateur existant et retourne un token JWT
        Task<ServiceResponse<string>> LoginAsync(LoginDto loginDto);

        // Récupère les informations de l'utilisateur connecté
        Task<UserDto> GetCurrentUserAsync();

        // Vérifie si un utilisateur a un rôle spécifique
        Task<bool> HasRoleAsync(string userId, string role);

        // Ajoute un rôle à un utilisateur
        Task<ServiceResponse<string>> AddRoleAsync(string userId, string role);

        // Supprime un rôle d'un utilisateur
        Task<ServiceResponse<string>> RemoveRoleAsync(string userId, string role);

        // Liste tous les utilisateurs
        Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync();

        // Supprime un utilisateur par son ID
        Task<ServiceResponse<string>> DeleteUserAsync(string userId);

        // Met à jour un utilisateur
        Task<ServiceResponse<string>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Name = registerDto.Name
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            await _userManager.AddToRoleAsync(user, "USER");
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        public async Task<ServiceResponse<string>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            var token = GenerateJwtToken(user);
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Login successful",
                Data = token
            };
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };
        }

        public async Task<ServiceResponse<string>> AddRoleAsync(string userId, string role)
        {
            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid user ID format" }
                };
            }

            var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded
                ? new ServiceResponse<string> { Success = true, Message = $"Role '{role}' added successfully" }
                : new ServiceResponse<string> { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }

        public async Task<bool> HasRoleAsync(string userId, string role)
        {
            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<ServiceResponse<string>> RemoveRoleAsync(string userId, string role)
        {
            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid user ID format" }
                };
            }

            var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded
                ? new ServiceResponse<string> { Success = true, Message = $"Role '{role}' removed successfully" }
                : new ServiceResponse<string> { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }


       
        public async Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
            }).ToList();

            return new ServiceResponse<List<UserDto>>
            {
                Success = true,
                Data = userDtos
            };
        }

       
        public async Task<ServiceResponse<string>> DeleteUserAsync(string userId)
        {
            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid user ID format" }
                };
            }

            var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? new ServiceResponse<string> { Success = true, Message = "User deleted successfully" }
                : new ServiceResponse<string> { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }

        public async Task<ServiceResponse<string>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid user ID format" }
                };
            }

            var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new ServiceResponse<string> { Success = true, Message = "User updated successfully" }
                : new ServiceResponse<string> { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            // Ajouter le rôle uniquement si l'utilisateur en a un
            var roles = _userManager.GetRolesAsync(user).Result;
            if (roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.First()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
