using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using api.Middleware;

namespace api.Configurations
{
    /// <summary>
    /// Filtre de schéma pour ajouter la définition ErrorResponse aux composants Swagger
    /// </summary>
    public class ErrorResponseSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applique le filtre au schéma
        /// </summary>
        /// <param name="schema">Schéma OpenAPI</param>
        /// <param name="context">Contexte du schéma</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ErrorResponse))
            {
                schema.Type = "object";
                schema.Properties = new Dictionary<string, OpenApiSchema>
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
                };
                schema.Required = new HashSet<string> { "success", "errorCode", "message", "timestamp" };
            }
        }
    }
}