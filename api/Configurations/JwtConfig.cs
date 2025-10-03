using Microsoft.Extensions.Configuration;

namespace api.Configurations
{
    /// <summary>
    /// Configuration pour les paramètres JWT.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// Clé secrète pour signer les tokens JWT.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Émetteur du token (issuer).
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Audience du token.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Durée de validité du token en minutes.
        /// </summary>
        public int ExpiryInMinutes { get; set; }

        /// <summary>
        /// Durée de validité du refresh token en jours.
        /// </summary>
        public int RefreshTokenExpiryInDays { get; set; } = 7;

        /// <summary>
        /// Charge la configuration JWT depuis la configuration de l'application.
        /// </summary>
        /// <param name="configuration">Configuration de l'application</param>
        /// <returns>Instance de JwtConfig</returns>
        public static JwtConfig LoadFromConfiguration(IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            return new JwtConfig
            {
                Key = jwtSection["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
                Issuer = jwtSection["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
                Audience = jwtSection["Audience"] ?? throw new ArgumentNullException("Jwt:Audience"),
                ExpiryInMinutes = int.Parse(jwtSection["ExpiryInMinutes"] ?? "15"),
                RefreshTokenExpiryInDays = int.Parse(jwtSection["RefreshTokenExpiryInDays"] ?? "7")
            };
        }
    }
}