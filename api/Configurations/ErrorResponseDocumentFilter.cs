using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using api.Middleware;

namespace api.Configurations
{
    /// <summary>
    /// Filtre de document pour ajouter la définition ErrorResponse aux composants Swagger
    /// </summary>
    public class ErrorResponseDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Applique le filtre au document Swagger
        /// </summary>
        /// <param name="swaggerDoc">Document Swagger</param>
        /// <param name="context">Contexte du document</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Ajouter le schéma ErrorResponse aux composants
            swaggerDoc.Components ??= new OpenApiComponents();
            swaggerDoc.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();
            
            if (!swaggerDoc.Components.Schemas.ContainsKey("ErrorResponse"))
            {
                swaggerDoc.Components.Schemas["ErrorResponse"] = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["success"] = new OpenApiSchema 
                        { 
                            Type = "boolean", 
                            Description = "Indique si l'opération a réussi",
                            Example = new Microsoft.OpenApi.Any.OpenApiBoolean(false)
                        },
                        ["errorCode"] = new OpenApiSchema 
                        { 
                            Type = "string", 
                            Description = "Code d'erreur unique identifiant le type d'erreur",
                            Example = new Microsoft.OpenApi.Any.OpenApiString("VALIDATION_ERROR")
                        },
                        ["message"] = new OpenApiSchema 
                        { 
                            Type = "string", 
                            Description = "Message décrivant l'erreur",
                            Example = new Microsoft.OpenApi.Any.OpenApiString("Une erreur de validation s'est produite")
                        },
                        ["details"] = new OpenApiSchema 
                        { 
                            Type = "object", 
                            Description = "Détails supplémentaires sur l'erreur",
                            Nullable = true
                        },
                        ["timestamp"] = new OpenApiSchema 
                        { 
                            Type = "string", 
                            Format = "date-time", 
                            Description = "Horodatage de l'erreur",
                            Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-01T12:00:00Z")
                        },
                        ["path"] = new OpenApiSchema 
                        { 
                            Type = "string", 
                            Description = "Chemin de la requête qui a causé l'erreur",
                            Nullable = true,
                            Example = new Microsoft.OpenApi.Any.OpenApiString("/api/auth/login")
                        },
                        ["stackTrace"] = new OpenApiSchema 
                        { 
                            Type = "string", 
                            Description = "Trace de la pile (uniquement en développement)",
                            Nullable = true
                        }
                    },
                    Required = new HashSet<string> { "success", "errorCode", "message", "timestamp" }
                };
            }
        }
    }
}