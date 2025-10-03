using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api.Models;
using api.Configurations;

namespace api.Helpers
{
    /// <summary>
    /// Helper pour la gestion des tokens JWT.
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Génère un token JWT pour un utilisateur.
        /// </summary>
        /// <param name="user">Utilisateur pour lequel générer le token</param>
        /// <param name="roles">Rôles de l'utilisateur</param>
        /// <param name="config">Configuration JWT</param>
        /// <returns>Token JWT généré</returns>
        public static string GenerateToken(ApplicationUser user, IEnumerable<string> roles, JwtConfig config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            // Ajouter les rôles comme claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(config.ExpiryInMinutes),
                Issuer = config.Issuer,
                Audience = config.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Valide un token JWT.
        /// </summary>
        /// <param name="token">Token à valider</param>
        /// <param name="config">Configuration JWT</param>
        /// <returns>ClaimsPrincipal si le token est valide, null sinon</returns>
        public static ClaimsPrincipal? ValidateToken(string token, JwtConfig config)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(config.Key);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extrait l'ID utilisateur depuis un token JWT.
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="config">Configuration JWT</param>
        /// <returns>ID de l'utilisateur ou null si invalide</returns>
        public static Guid? GetUserIdFromToken(string token, JwtConfig config)
        {
            var principal = ValidateToken(token, config);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }

        /// <summary>
        /// Extrait les rôles depuis un token JWT.
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="config">Configuration JWT</param>
        /// <returns>Liste des rôles ou liste vide si invalide</returns>
        public static List<string> GetRolesFromToken(string token, JwtConfig config)
        {
            var principal = ValidateToken(token, config);
            var roleClaims = principal?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
            return roleClaims.ToList();
        }

        /// <summary>
        /// Vérifie si un token JWT est expiré.
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="config">Configuration JWT</param>
        /// <returns>True si le token est expiré, false sinon</returns>
        public static bool IsTokenExpired(string token, JwtConfig config)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Génère un refresh token sécurisé.
        /// </summary>
        /// <returns>Refresh token généré</returns>
        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Calcule la date d'expiration d'un refresh token.
        /// </summary>
        /// <param name="config">Configuration JWT</param>
        /// <returns>Date d'expiration</returns>
        public static DateTime GetRefreshTokenExpiry(JwtConfig config)
        {
            return DateTime.UtcNow.AddDays(config.RefreshTokenExpiryInDays);
        }
    }
}