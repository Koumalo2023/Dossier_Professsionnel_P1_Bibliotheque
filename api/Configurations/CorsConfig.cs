using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Configurations
{
    /// <summary>
    /// Configuration pour la politique CORS (Cross-Origin Resource Sharing).
    /// </summary>
    public class CorsConfig
    {
        /// <summary>
        /// Nom de la politique CORS pour l'environnement de développement.
        /// </summary>
        public const string DevelopmentPolicy = "DevelopmentPolicy";

        /// <summary>
        /// Nom de la politique CORS pour l'environnement de production.
        /// </summary>
        public const string ProductionPolicy = "ProductionPolicy";

        /// <summary>
        /// Liste des origines autorisées pour l'environnement de développement.
        /// </summary>
        public List<string> DevelopmentOrigins { get; set; } = new List<string>();

        /// <summary>
        /// Liste des origines autorisées pour l'environnement de production.
        /// </summary>
        public List<string> ProductionOrigins { get; set; } = new List<string>();

        /// <summary>
        /// Méthodes HTTP autorisées.
        /// </summary>
        public List<string> AllowedMethods { get; set; } = new List<string>
        {
            "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"
        };

        /// <summary>
        /// En-têtes autorisés.
        /// </summary>
        public List<string> AllowedHeaders { get; set; } = new List<string>
        {
            "Authorization", "Content-Type", "X-Requested-With", "X-CSRF-Token"
        };

        /// <summary>
        /// En-têtes exposés au client.
        /// </summary>
        public List<string> ExposedHeaders { get; set; } = new List<string>
        {
            "Content-Disposition", "X-Pagination"
        };

        /// <summary>
        /// Indique si les credentials sont autorisés.
        /// </summary>
        public bool AllowCredentials { get; set; } = true;

        /// <summary>
        /// Durée de mise en cache des pré-vérifications CORS (en secondes).
        /// </summary>
        public int PreflightCacheDuration { get; set; } = 600;

        /// <summary>
        /// Charge la configuration CORS à partir de la configuration de l'application.
        /// </summary>
        /// <param name="configuration">Configuration de l'application</param>
        /// <returns>Instance de CorsConfig</returns>
        public static CorsConfig LoadFromConfiguration(IConfiguration configuration)
        {
            var corsConfig = new CorsConfig();

            // Charger les origines de développement
            var devOrigins = configuration.GetSection("Cors:DevelopmentOrigins").Get<List<string>>();
            if (devOrigins != null)
            {
                corsConfig.DevelopmentOrigins = devOrigins;
            }

            // Charger les origines de production
            var prodOrigins = configuration.GetSection("Cors:ProductionOrigins").Get<List<string>>();
            if (prodOrigins != null)
            {
                corsConfig.ProductionOrigins = prodOrigins;
            }

            // Charger les méthodes autorisées
            var allowedMethods = configuration.GetSection("Cors:AllowedMethods").Get<List<string>>();
            if (allowedMethods != null)
            {
                corsConfig.AllowedMethods = allowedMethods;
            }

            // Charger les en-têtes autorisés
            var allowedHeaders = configuration.GetSection("Cors:AllowedHeaders").Get<List<string>>();
            if (allowedHeaders != null)
            {
                corsConfig.AllowedHeaders = allowedHeaders;
            }

            // Charger les en-têtes exposés
            var exposedHeaders = configuration.GetSection("Cors:ExposedHeaders").Get<List<string>>();
            if (exposedHeaders != null)
            {
                corsConfig.ExposedHeaders = exposedHeaders;
            }

            // Charger les autres paramètres
            corsConfig.AllowCredentials = configuration.GetValue("Cors:AllowCredentials", true);
            corsConfig.PreflightCacheDuration = configuration.GetValue("Cors:PreflightCacheDuration", 600);

            return corsConfig;
        }

        /// <summary>
        /// Valide la configuration CORS.
        /// </summary>
        /// <returns>True si la configuration est valide, sinon false avec un message d'erreur</returns>
        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (!DevelopmentOrigins.Any() && !ProductionOrigins.Any())
            {
                return (false, "Au moins une origine doit être configurée pour le développement ou la production.");
            }

            if (!AllowedMethods.Any())
            {
                return (false, "Au moins une méthode HTTP doit être autorisée.");
            }

            if (!AllowedHeaders.Any())
            {
                return (false, "Au moins un en-tête doit être autorisé.");
            }

            if (PreflightCacheDuration < 0)
            {
                return (false, "La durée de mise en cache des pré-vérifications doit être positive.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Obtient les origines autorisées pour un environnement spécifique.
        /// </summary>
        /// <param name="isDevelopment">True si l'environnement est de développement</param>
        /// <returns>Liste des origines autorisées</returns>
        public List<string> GetAllowedOrigins(bool isDevelopment)
        {
            return isDevelopment ? DevelopmentOrigins : ProductionOrigins;
        }

        /// <summary>
        /// Obtient le nom de la politique CORS pour un environnement spécifique.
        /// </summary>
        /// <param name="isDevelopment">True si l'environnement est de développement</param>
        /// <returns>Nom de la politique CORS</returns>
        public string GetPolicyName(bool isDevelopment)
        {
            return isDevelopment ? DevelopmentPolicy : ProductionPolicy;
        }

        /// <summary>
        /// Vérifie si une origine est autorisée pour un environnement spécifique.
        /// </summary>
        /// <param name="origin">Origine à vérifier</param>
        /// <param name="isDevelopment">True si l'environnement est de développement</param>
        /// <returns>True si l'origine est autorisée</returns>
        public bool IsOriginAllowed(string origin, bool isDevelopment)
        {
            var allowedOrigins = GetAllowedOrigins(isDevelopment);
            return allowedOrigins.Contains(origin) || allowedOrigins.Contains("*");
        }

        /// <summary>
        /// Crée une configuration CORS par défaut.
        /// </summary>
        /// <returns>Configuration CORS par défaut</returns>
        public static CorsConfig CreateDefault()
        {
            return new CorsConfig
            {
                DevelopmentOrigins = new List<string>
                {
                    "https://localhost:4200",
                    "http://localhost:4200",
                    "https://localhost:3000",
                    "http://localhost:3000",
                    "https://localhost:5001",
                    "http://localhost:5000"
                },
                ProductionOrigins = new List<string>
                {
                    "https://bibliotheque-app.example.com",
                    "https://www.bibliotheque-app.example.com"
                },
                AllowedMethods = new List<string>
                {
                    "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"
                },
                AllowedHeaders = new List<string>
                {
                    "Authorization", "Content-Type", "X-Requested-With", "X-CSRF-Token"
                },
                ExposedHeaders = new List<string>
                {
                    "Content-Disposition", "X-Pagination"
                },
                AllowCredentials = true,
                PreflightCacheDuration = 600
            };
        }
    }
}