using api.DTOs;
using api.DTOs.ApplicationUser;
using api.Helpers;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace api.Services
{
    public interface IAuthService
    {
        // Enregistre un nouvel utilisateur
        Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto);

        // Connecte un utilisateur existant et retourne un token JWT
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string ipAddress);

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

        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress);
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUserRepository _userRepository;

       public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _userRepository = userRepository;
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

            await _userManager.AddToRoleAsync(user, "User");
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        // AuthService.cs
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect");

            var jwtToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            await _userRepository.CreateAsync(refreshToken);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                TokenExpires = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpires = refreshToken.Expires
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new SecurityTokenException("Utilisateur introuvable");

            var refreshToken = await _userRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.UserId != userId || !refreshToken.IsActive)
                throw new SecurityTokenException("Refresh token invalide");

            // Remplacer l'ancien refresh token
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await _userRepository.CreateAsync(newRefreshToken);
            await _userRepository.UpdateAsync(refreshToken);
            await _userRepository.RevokeDescendantsAsync(refreshToken, ipAddress, "Remplacé par nouveau token");

            // Générer nouveau JWT
            var newJwtToken = await GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token,
                TokenExpires = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpires = newRefreshToken.Expires
            };
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Token invalide");

            return principal;
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
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
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
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
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
        

        // Nouvelle version corrigée
        private async Task<string> GenerateJwtToken(ApplicationUser user) // Ajout de async et Task<string>
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            var roles = await _userManager.GetRolesAsync(user); 
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
