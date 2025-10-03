using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace api.Configurations
{
    /// <summary>
    /// Configuration pour l'initialisation des rôles et utilisateurs par défaut.
    /// </summary>
    public class SeedUserConfig
    {
        /// <summary>
        /// Indique si l'initialisation des données de seed est activée.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Liste des rôles à créer lors de l'initialisation.
        /// </summary>
        public List<RoleConfig> Roles { get; set; } = new();

        /// <summary>
        /// Liste des utilisateurs à créer lors de l'initialisation.
        /// </summary>
        public List<UserConfig> Users { get; set; } = new();

        /// <summary>
        /// Configuration pour le mode développement.
        /// </summary>
        public DevelopmentConfig Development { get; set; } = new();

        /// <summary>
        /// Configuration pour le mode production.
        /// </summary>
        public ProductionConfig Production { get; set; } = new();

        /// <summary>
        /// Charge la configuration à partir de la configuration de l'application.
        /// </summary>
        /// <param name="configuration">Configuration de l'application</param>
        /// <returns>Instance de SeedUserConfig</returns>
        public static SeedUserConfig LoadFromConfiguration(IConfiguration configuration)
        {
            var config = new SeedUserConfig();
            configuration.GetSection("SeedUser").Bind(config);
            
            // Configuration par défaut si non spécifiée
            if (config.Roles.Count == 0)
            {
                config.Roles = GetDefaultRoles();
            }

            if (config.Users.Count == 0)
            {
                config.Users = GetDefaultUsers();
            }

            return config;
        }

        /// <summary>
        /// Obtient la configuration des rôles par défaut.
        /// </summary>
        /// <returns>Liste des rôles par défaut</returns>
        private static List<RoleConfig> GetDefaultRoles()
        {
            return new List<RoleConfig>
            {
                new RoleConfig { Name = "Admin", Description = "Administrateur du système" },
                new RoleConfig { Name = "Manager", Description = "Gestionnaire de bibliothèque" },
                new RoleConfig { Name = "User", Description = "Utilisateur standard" }
            };
        }

        /// <summary>
        /// Obtient la configuration des utilisateurs par défaut.
        /// </summary>
        /// <returns>Liste des utilisateurs par défaut</returns>
        private static List<UserConfig> GetDefaultUsers()
        {
            return new List<UserConfig>
            {
                new UserConfig 
                { 
                    Email = "admin@bibliotheque.com", 
                    Password = "Admin123!", 
                    Name = "Administrateur",
                    Roles = new List<string> { "Admin" }
                },
                new UserConfig 
                { 
                    Email = "manager@bibliotheque.com", 
                    Password = "Manager123!", 
                    Name = "Gestionnaire",
                    Roles = new List<string> { "Manager" }
                },
                new UserConfig 
                { 
                    Email = "user@bibliotheque.com", 
                    Password = "User123!", 
                    Name = "Utilisateur Standard",
                    Roles = new List<string> { "User" }
                }
            };
        }

        /// <summary>
        /// Valide la configuration.
        /// </summary>
        /// <returns>Résultat de la validation</returns>
        public SeedValidationResult Validate()
        {
            var errors = new List<string>();

            if (!Enabled)
            {
                return new SeedValidationResult { IsValid = true };
            }

            if (Roles.Count == 0)
            {
                errors.Add("Aucun rôle n'est configuré pour l'initialisation");
            }

            foreach (var user in Users)
            {
                var userErrors = user.Validate();
                if (!userErrors.IsValid)
                {
                    errors.Add($"Erreur pour l'utilisateur {user.Email}: {userErrors.ErrorMessage}");
                }
            }

            return new SeedValidationResult
            {
                IsValid = errors.Count == 0,
                ErrorMessage = errors.Count > 0 ? string.Join("; ", errors) : null
            };
        }
    }

    /// <summary>
    /// Configuration d'un rôle à créer.
    /// </summary>
    public class RoleConfig
    {
        /// <summary>
        /// Nom du rôle.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description du rôle.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valide la configuration du rôle.
        /// </summary>
        /// <returns>Résultat de la validation</returns>
        public SeedValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Le nom du rôle est requis");
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                errors.Add("La description du rôle est requise");
            }

            return new SeedValidationResult
            {
                IsValid = errors.Count == 0,
                ErrorMessage = errors.Count > 0 ? string.Join("; ", errors) : null
            };
        }
    }

    /// <summary>
    /// Configuration d'un utilisateur à créer.
    /// </summary>
    public class UserConfig
    {
        /// <summary>
        /// Adresse email de l'utilisateur.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe de l'utilisateur.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Nom complet de l'utilisateur.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Indique si l'email est confirmé.
        /// </summary>
        public bool EmailConfirmed { get; set; } = true;

        /// <summary>
        /// Liste des rôles à assigner à l'utilisateur.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Valide la configuration de l'utilisateur.
        /// </summary>
        /// <returns>Résultat de la validation</returns>
        public SeedValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Email))
            {
                errors.Add("L'email de l'utilisateur est requis");
            }
            else if (!IsValidEmail(Email))
            {
                errors.Add("L'email de l'utilisateur n'est pas valide");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                errors.Add("Le mot de passe de l'utilisateur est requis");
            }
            else if (Password.Length < 6)
            {
                errors.Add("Le mot de passe doit contenir au moins 6 caractères");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Le nom de l'utilisateur est requis");
            }

            if (Roles.Count == 0)
            {
                errors.Add("L'utilisateur doit avoir au moins un rôle");
            }

            return new SeedValidationResult
            {
                IsValid = errors.Count == 0,
                ErrorMessage = errors.Count > 0 ? string.Join("; ", errors) : null
            };
        }

        /// <summary>
        /// Vérifie si l'email est valide.
        /// </summary>
        /// <param name="email">Email à vérifier</param>
        /// <returns>True si l'email est valide</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Configuration spécifique au mode développement.
    /// </summary>
    public class DevelopmentConfig
    {
        /// <summary>
        /// Indique si les données de test doivent être créées.
        /// </summary>
        public bool CreateTestData { get; set; } = true;

        /// <summary>
        /// Nombre d'utilisateurs de test à créer.
        /// </summary>
        public int TestUserCount { get; set; } = 10;

        /// <summary>
        /// Mot de passe par défaut pour les utilisateurs de test.
        /// </summary>
        public string TestUserPassword { get; set; } = "Test123!";
    }

    /// <summary>
    /// Configuration spécifique au mode production.
    /// </summary>
    public class ProductionConfig
    {
        /// <summary>
        /// Indique si la création d'utilisateurs par défaut est obligatoire.
        /// </summary>
        public bool RequireDefaultUsers { get; set; } = true;

        /// <summary>
        /// Indique si les mots de passe doivent être générés automatiquement.
        /// </summary>
        public bool GeneratePasswords { get; set; } = false;

        /// <summary>
        /// Longueur des mots de passe générés.
        /// </summary>
        public int GeneratedPasswordLength { get; set; } = 12;
    }

    /// <summary>
    /// Service pour l'initialisation des données de seed.
    /// </summary>
    public class SeedUserService
    {
        private readonly SeedUserConfig _config;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SeedUserService> _logger;

        /// <summary>
        /// Initialise une nouvelle instance du service de seed.
        /// </summary>
        /// <param name="config">Configuration du seed</param>
        /// <param name="roleManager">Gestionnaire de rôles</param>
        /// <param name="userManager">Gestionnaire d'utilisateurs</param>
        /// <param name="logger">Logger</param>
        public SeedUserService(
            SeedUserConfig config,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<SeedUserService> logger)
        {
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Exécute l'initialisation des données de seed.
        /// </summary>
        /// <returns>Task représentant l'opération asynchrone</returns>
        public async Task InitializeAsync()
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("L'initialisation des données de seed est désactivée");
                return;
            }

            _logger.LogInformation("Début de l'initialisation des données de seed");

            try
            {
                // Créer les rôles
                await CreateRolesAsync();

                // Créer les utilisateurs
                await CreateUsersAsync();

                _logger.LogInformation("Initialisation des données de seed terminée avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation des données de seed");
                throw;
            }
        }

        /// <summary>
        /// Crée les rôles configurés.
        /// </summary>
        /// <returns>Task représentant l'opération asynchrone</returns>
        private async Task CreateRolesAsync()
        {
            foreach (var roleConfig in _config.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleConfig.Name))
                {
                    var role = new ApplicationRole(roleConfig.Name)
                    {
                        Description = roleConfig.Description
                    };

                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Rôle {RoleName} créé avec succès", roleConfig.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Erreur lors de la création du rôle {RoleName}: {Errors}", 
                            roleConfig.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("Le rôle {RoleName} existe déjà", roleConfig.Name);
                }
            }
        }

        /// <summary>
        /// Crée les utilisateurs configurés.
        /// </summary>
        /// <returns>Task représentant l'opération asynchrone</returns>
        private async Task CreateUsersAsync()
        {
            foreach (var userConfig in _config.Users)
            {
                var existingUser = await _userManager.FindByEmailAsync(userConfig.Email);
                if (existingUser != null)
                {
                    _logger.LogInformation("L'utilisateur {Email} existe déjà", userConfig.Email);
                    
                    // Mettre à jour les rôles de l'utilisateur existant
                    await UpdateUserRolesAsync(existingUser, userConfig.Roles);
                    continue;
                }

                var user = new ApplicationUser
                {
                    UserName = userConfig.Email,
                    Email = userConfig.Email,
                    Name = userConfig.Name,
                    EmailConfirmed = userConfig.EmailConfirmed
                };

                var result = await _userManager.CreateAsync(user, userConfig.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur {Email} créé avec succès", userConfig.Email);

                    // Assigner les rôles
                    await AssignUserRolesAsync(user, userConfig.Roles);
                }
                else
                {
                    _logger.LogWarning("Erreur lors de la création de l'utilisateur {Email}: {Errors}", 
                        userConfig.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        /// <summary>
        /// Assigner les rôles à un utilisateur.
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <param name="roles">Liste des rôles</param>
        /// <returns>Task représentant l'opération asynchrone</returns>
        private async Task AssignUserRolesAsync(ApplicationUser user, List<string> roles)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(roles).ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (removeResult.Succeeded)
                {
                    _logger.LogInformation("Rôles retirés de l'utilisateur {Email}: {Roles}", 
                        user.Email, string.Join(", ", rolesToRemove));
                }
            }

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (addResult.Succeeded)
                {
                    _logger.LogInformation("Rôles assignés à l'utilisateur {Email}: {Roles}", 
                        user.Email, string.Join(", ", rolesToAdd));
                }
            }
        }

        /// <summary>
        /// Mettre à jour les rôles d'un utilisateur existant.
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <param name="roles">Nouvelle liste de rôles</param>
        /// <returns>Task représentant l'opération asynchrone</returns>
        private async Task UpdateUserRolesAsync(ApplicationUser user, List<string> roles)
        {
            await AssignUserRolesAsync(user, roles);
        }
    }

    /// <summary>
    /// Résultat de validation pour la configuration de seed.
    /// </summary>
    public class SeedValidationResult
    {
        /// <summary>
        /// Indique si la configuration est valide.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Message d'erreur en cas de configuration invalide.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}