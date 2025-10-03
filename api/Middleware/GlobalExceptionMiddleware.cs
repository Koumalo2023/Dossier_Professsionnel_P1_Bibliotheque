using api.Exceptions;
using api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace api.Middleware
{
    /// <summary>
    /// Middleware global pour la gestion centralisée des exceptions.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// Initialise une nouvelle instance du middleware.
        /// </summary>
        /// <param name="next">Delegate suivant dans le pipeline</param>
        /// <param name="logger">Logger pour enregistrer les erreurs</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Traite la requête HTTP et intercepte les exceptions.
        /// </summary>
        /// <param name="context">Contexte HTTP</param>
        /// <returns>Task représentant l'opération asynchrone</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Gère l'exception et génère une réponse HTTP cohérente.
        /// </summary>
        /// <param name="context">Contexte HTTP</param>
        /// <param name="exception">Exception à gérer</param>
        /// <returns>Task représentant l'opération asynchrone</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Déterminer le code HTTP et le message d'erreur
            var (statusCode, errorResponse) = MapExceptionToErrorResponse(exception);

            // Enregistrer l'erreur avec le niveau approprié
            LogException(exception, statusCode);

            // Configurer la réponse HTTP
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Sérialiser la réponse d'erreur
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        /// <summary>
        /// Mappe une exception vers une réponse d'erreur appropriée.
        /// </summary>
        /// <param name="exception">Exception à mapper</param>
        /// <returns>Tuple contenant le code HTTP et la réponse d'erreur</returns>
        private (HttpStatusCode statusCode, ErrorResponse errorResponse) MapExceptionToErrorResponse(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = validationEx.ErrorCode,
                        Message = validationEx.Message,
                        Details = validationEx.Details,
                        Timestamp = DateTime.UtcNow
                    }
                ),
                NotFoundException notFoundEx => (
                    HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = notFoundEx.ErrorCode,
                        Message = notFoundEx.Message,
                        Details = new { notFoundEx.ResourceType, notFoundEx.ResourceId },
                        Timestamp = DateTime.UtcNow
                    }
                ),
                UnauthorizedException unauthorizedEx => (
                    HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = unauthorizedEx.ErrorCode,
                        Message = unauthorizedEx.Message,
                        Timestamp = DateTime.UtcNow
                    }
                ),
                ForbiddenException forbiddenEx => (
                    HttpStatusCode.Forbidden,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = forbiddenEx.ErrorCode,
                        Message = forbiddenEx.Message,
                        Details = new { forbiddenEx.Operation, forbiddenEx.Reason },
                        Timestamp = DateTime.UtcNow
                    }
                ),
                ConflictException conflictEx => (
                    HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = conflictEx.ErrorCode,
                        Message = conflictEx.Message,
                        Details = new { conflictEx.ResourceType, conflictEx.ResourceId },
                        Timestamp = DateTime.UtcNow
                    }
                ),
                BusinessException businessEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = businessEx.ErrorCode,
                        Message = businessEx.Message,
                        Details = businessEx.Details,
                        Timestamp = DateTime.UtcNow
                    }
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "INTERNAL_SERVER_ERROR",
                        Message = "Une erreur interne s'est produite.",
                        Timestamp = DateTime.UtcNow
                    }
                )
            };
        }

        /// <summary>
        /// Enregistre l'exception avec le niveau de log approprié.
        /// </summary>
        /// <param name="exception">Exception à enregistrer</param>
        /// <param name="statusCode">Code HTTP de l'erreur</param>
        private void LogException(Exception exception, HttpStatusCode statusCode)
        {
            var logLevel = GetLogLevel(exception, statusCode);
            var logMessage = $"Erreur HTTP {(int)statusCode} - {exception.GetType().Name}";

            _logger.Log(logLevel, exception, logMessage);

            // Enregistrer les détails supplémentaires pour les erreurs de validation
            if (exception is ValidationException validationEx && validationEx.Details != null)
            {
                _logger.LogInformation("Détails de validation: {@ValidationDetails}", validationEx.Details);
            }
        }

        /// <summary>
        /// Détermine le niveau de log approprié selon le type d'exception et le code HTTP.
        /// </summary>
        /// <param name="exception">Exception à évaluer</param>
        /// <param name="statusCode">Code HTTP de l'erreur</param>
        /// <returns>Niveau de log approprié</returns>
        private LogLevel GetLogLevel(Exception exception, HttpStatusCode statusCode)
        {
            return exception switch
            {
                // Les erreurs côté client (4xx) sont loggées en Information
                ValidationException => LogLevel.Information,
                NotFoundException => LogLevel.Information,
                UnauthorizedException => LogLevel.Information,
                ForbiddenException => LogLevel.Information,
                ConflictException => LogLevel.Information,
                BusinessException => LogLevel.Warning,
                // Les erreurs côté serveur (5xx) sont loggées en Error
                _ when (int)statusCode >= 500 => LogLevel.Error,
                // Par défaut, Warning
                _ => LogLevel.Warning
            };
        }
    }

    /// <summary>
    /// Réponse d'erreur standardisée pour l'API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Indique si l'opération a réussi.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Code d'erreur unique identifiant le type d'erreur.
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Message décrivant l'erreur.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Détails supplémentaires sur l'erreur.
        /// </summary>
        public object? Details { get; set; }

        /// <summary>
        /// Horodatage de l'erreur.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Chemin de la requête qui a causé l'erreur.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Trace de la pile (uniquement en développement).
        /// </summary>
        public string? StackTrace { get; set; }
    }
}