using api.Configurations;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 

namespace api.Services
{
    public interface IAuthService
    {
        // Enregistre un nouvel utilisateur
        Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto);

        // Connecte un utilisateur existant et retourne un token JWT
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);

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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            JwtConfig jwtConfig,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtConfig = jwtConfig;
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

            // Add user to default 'User' role using UserManager
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                // If role assignment fails, delete the user to maintain consistency
                await _userManager.DeleteAsync(user);
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = roleResult.Errors.Select(e => e.Description).ToList()
                };
            }

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        // AuthService.cs
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect");

            var jwtToken = await GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = jwtToken,
                TokenExpires = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = await GetUserRolesAsync(user.Id)
                }
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
                Roles = await GetUserRolesAsync(user.Id)
            };
        }

        public async Task<ServiceResponse<string>> AddRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { "User not found" } };
            }

            // Vérifier si le rôle existe
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { $"Role '{roleName}' not found" } };
            }

            // Vérifier si l'utilisateur a déjà le rôle
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { $"User already has role '{roleName}'" } };
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new ServiceResponse<string> { Success = true, Message = $"Role '{roleName}' added successfully" };
        }

        public async Task<bool> HasRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<ServiceResponse<string>> RemoveRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { "User not found" } };
            }

            // Vérifier si le rôle existe
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { $"Role '{roleName}' not found" } };
            }

            // Vérifier si l'utilisateur a le rôle
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                return new ServiceResponse<string> { Success = false, Errors = new List<string> { $"User does not have role '{roleName}'" } };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new ServiceResponse<string> { Success = true, Message = $"Role '{roleName}' removed successfully" };
        }


       
        public async Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();
            
            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = await GetUserRolesAsync(user.Id)
                });
            }

            return new ServiceResponse<List<UserDto>> { Success = true, Data = userDtos };
        }
        
        private async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new List<string>();
            }
            
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
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
        

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await GetUserRolesAsync(user.Id);
            return JwtHelper.GenerateToken(user, roles, _jwtConfig);
        }
    }
}
