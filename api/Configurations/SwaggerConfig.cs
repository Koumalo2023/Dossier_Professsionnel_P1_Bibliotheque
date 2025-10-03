using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using api.Middleware;
using api.Configurations;

namespace api.Configurations
{
    /// <summary>
    /// Configuration centralisée pour Swagger/OpenAPI.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Titre de l'API affiché dans Swagger UI.
        /// </summary>
        public string Title { get; set; } = "Library Management API";

        /// <summary>
        /// Version de l'API.
        /// </summary>
        public string Version { get; set; } = "v1";

        /// <summary>
        /// Description de l'API.
        /// </summary>
        public string Description { get; set; } = "API de gestion de bibliothèque avec authentification JWT";

        /// <summary>
        /// URL des termes de service.
        /// </summary>
        public string? TermsOfService { get; set; }

        /// <summary>
        /// Informations de contact.
        /// </summary>
        public ContactInfo? Contact { get; set; }

        /// <summary>
        /// Informations de licence.
        /// </summary>
        public LicenseInfo? License { get; set; }

        /// <summary>
        /// Chemin vers le fichier XML de documentation.
        /// </summary>
        public string? XmlCommentsPath { get; set; }

        /// <summary>
        /// Active la documentation XML.
        /// </summary>
        public bool EnableXmlComments { get; set; } = true;

        /// <summary>
        /// Active l'authentification JWT dans Swagger.
        /// </summary>
        public bool EnableJwtAuth { get; set; } = true;

        /// <summary>
        /// Active les filtres d'opération personnalisés.
        /// </summary>
        public bool EnableOperationFilters { get; set; } = true;

        /// <summary>
        /// Charge la configuration Swagger à partir de la configuration de l'application.
        /// </summary>
        /// <param name="configuration">Configuration de l'application</param>
        /// <returns>Instance de SwaggerConfig</returns>
        public static SwaggerConfig LoadFromConfiguration(IConfiguration configuration)
        {
            var config = new SwaggerConfig();
            configuration.GetSection("Swagger").Bind(config);
            return config;
        }

        /// <summary>
        /// Configure les options SwaggerGen.
        /// </summary>
        /// <param name="options">Options SwaggerGen à configurer</param>
        public void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            // Configuration de base
            options.SwaggerDoc(Version, new OpenApiInfo
            {
                Title = Title,
                Version = Version,
                Description = Description,
                TermsOfService = TermsOfService != null ? new Uri(TermsOfService) : null,
                Contact = Contact?.ToOpenApiContact(),
                License = License?.ToOpenApiLicense()
            });

            // Configuration de l'authentification JWT
            if (EnableJwtAuth)
            {
                ConfigureJwtAuthentication(options);
            }

            // Configuration des commentaires XML
            if (EnableXmlComments && !string.IsNullOrEmpty(XmlCommentsPath))
            {
                ConfigureXmlComments(options, XmlCommentsPath);
            }

            // Filtres d'opération
            if (EnableOperationFilters)
            {
                ConfigureOperationFilters(options);
            }

            // Configuration supplémentaire
            ConfigureAdditionalSettings(options);
        }

        /// <summary>
        /// Configure l'authentification JWT pour Swagger.
        /// </summary>
        /// <param name="options">Options SwaggerGen</param>
        private void ConfigureJwtAuthentication(SwaggerGenOptions options)
        {
            // Définition du schéma de sécurité JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = """
                JWT Authorization header utilisant le schéma Bearer.
                Entrez 'Bearer' [espace] puis votre token JWT.
                Exemple: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                """,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            // Exigence de sécurité globale
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        /// <summary>
        /// Configure les commentaires XML pour la documentation.
        /// </summary>
        /// <param name="options">Options SwaggerGen</param>
        /// <param name="xmlPath">Chemin vers le fichier XML</param>
        private void ConfigureXmlComments(SwaggerGenOptions options, string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
            else
            {
                // Tentative de trouver le fichier XML automatiquement
                var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var defaultXmlPath = Path.Combine(basePath ?? string.Empty, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                
                if (File.Exists(defaultXmlPath))
                {
                    options.IncludeXmlComments(defaultXmlPath);
                }
            }
        }

        /// <summary>
        /// Configure les filtres d'opération personnalisés.
        /// </summary>
        /// <param name="options">Options SwaggerGen</param>
        private void ConfigureOperationFilters(SwaggerGenOptions options)
        {
            // Filtre pour ajouter les en-têtes CORS
            options.OperationFilter<CorsHeadersOperationFilter>();
            
            // Filtre pour ajouter les codes de réponse communs
            options.OperationFilter<CommonResponsesOperationFilter>();
        }

        /// <summary>
        /// Configure les paramètres supplémentaires de Swagger.
        /// </summary>
        /// <param name="options">Options SwaggerGen</param>
        private void ConfigureAdditionalSettings(SwaggerGenOptions options)
        {
            // Support des énumérations comme strings
            options.SchemaFilter<EnumSchemaFilter>();

            // Ignorer les propriétés obsolètes
            options.SchemaFilter<IgnoreObsoletePropertiesFilter>();

            // Ordonner les actions par ordre alphabétique
            options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

            // Tag personnalisé pour les contrôleurs
            options.TagActionsBy(apiDesc =>
            {
                return new[] { apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Default" };
            });

            // Ajouter le schéma ErrorResponse aux composants
            options.AddServer(new OpenApiServer { Url = "/" });
            options.SchemaFilter<ErrorResponseSchemaFilter>();
            
            // Définir explicitement le schéma ErrorResponse dans les composants
            options.DocumentFilter<ErrorResponseDocumentFilter>();
        }

        /// <summary>
        /// Valide la configuration Swagger.
        /// </summary>
        /// <returns>Résultat de la validation</returns>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Le titre de l'API est requis");

            if (string.IsNullOrWhiteSpace(Version))
                errors.Add("La version de l'API est requise");

            if (string.IsNullOrWhiteSpace(Description))
                errors.Add("La description de l'API est requise");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                ErrorMessage = errors.Count > 0 ? string.Join("; ", errors) : null
            };
        }
    }

    /// <summary>
    /// Informations de contact pour Swagger.
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Nom du contact.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Email du contact.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// URL du contact.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Convertit en OpenApiContact.
        /// </summary>
        /// <returns>Instance OpenApiContact</returns>
        public OpenApiContact ToOpenApiContact()
        {
            return new OpenApiContact
            {
                Name = Name,
                Email = Email,
                Url = Url != null ? new Uri(Url) : null
            };
        }
    }

    /// <summary>
    /// Informations de licence pour Swagger.
    /// </summary>
    public class LicenseInfo
    {
        /// <summary>
        /// Nom de la licence.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// URL de la licence.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Convertit en OpenApiLicense.
        /// </summary>
        /// <returns>Instance OpenApiLicense</returns>
        public OpenApiLicense ToOpenApiLicense()
        {
            return new OpenApiLicense
            {
                Name = Name,
                Url = Url != null ? new Uri(Url) : null
            };
        }
    }

    /// <summary>
    /// Résultat de validation.
    /// </summary>
    public class ValidationResult
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

    // Filtres d'opération personnalisés

    /// <summary>
    /// Filtre pour ajouter les en-têtes CORS aux opérations.
    /// </summary>
    public class CorsHeadersOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applique le filtre à l'opération.
        /// </summary>
        /// <param name="operation">Opération Swagger</param>
        /// <param name="context">Contexte de l'opération</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            // Ajouter les en-têtes CORS communs
            if (!operation.Parameters.Any(p => p.Name == "Origin"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Origin",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = "Origine de la requête CORS"
                });
            }
        }
    }

    /// <summary>
    /// Filtre pour ajouter les réponses communes aux opérations.
    /// </summary>
    public class CommonResponsesOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applique le filtre à l'opération.
        /// </summary>
        /// <param name="operation">Opération Swagger</param>
        /// <param name="context">Contexte de l'opération</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses ??= new OpenApiResponses();

            // Réponses communes
            if (!operation.Responses.ContainsKey("400"))
            {
                operation.Responses.Add("400", new OpenApiResponse
                {
                    Description = "Requête invalide",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "ErrorResponse"
                                }
                            }
                        }
                    }
                });
            }

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Non autorisé"
                });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Accès interdit"
                });
            }

            if (!operation.Responses.ContainsKey("500"))
            {
                operation.Responses.Add("500", new OpenApiResponse
                {
                    Description = "Erreur interne du serveur"
                });
            }
        }
    }

    /// <summary>
    /// Filtre pour afficher les énumérations comme des strings.
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applique le filtre au schéma.
        /// </summary>
        /// <param name="schema">Schéma OpenAPI</param>
        /// <param name="context">Contexte du schéma</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Type = "string";
                schema.Enum.Clear();
                foreach (var enumValue in Enum.GetNames(context.Type))
                {
                    schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumValue));
                }
            }
        }
    }

    /// <summary>
    /// Filtre pour ignorer les propriétés marquées comme obsolètes.
    /// </summary>
    public class IgnoreObsoletePropertiesFilter : ISchemaFilter
    {
        /// <summary>
        /// Applique le filtre au schéma.
        /// </summary>
        /// <param name="schema">Schéma OpenAPI</param>
        /// <param name="context">Contexte du schéma</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null) return;

            var obsoleteProperties = context.Type.GetProperties()
                .Where(prop => prop.IsDefined(typeof(ObsoleteAttribute), true))
                .Select(prop => prop.Name);

            foreach (var propName in obsoleteProperties)
            {
                if (schema.Properties.ContainsKey(propName))
                {
                    schema.Properties.Remove(propName);
                }
            }
        }
    }
}