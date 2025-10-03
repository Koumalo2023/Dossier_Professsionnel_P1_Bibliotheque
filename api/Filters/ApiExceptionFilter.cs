using api.Exceptions;
using api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; 
using System.Net;

namespace api.Filters
{
    /// <summary>
    /// Filtre d'exception global pour les contrôleurs API qui capture et gère les exceptions de manière cohérente.
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly bool _isDevelopment;

        /// <summary>
        /// Initialise une nouvelle instance du filtre d'exception API.
        /// </summary>
        /// <param name="logger">Logger pour enregistrer les erreurs</param>
        /// <param name="environment">Environnement d'exécution</param>
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _isDevelopment = environment.IsDevelopment();
        }

        /// <summary>
        /// Méthode appelée lorsqu'une exception se produit dans un contrôleur.
        /// </summary>
        /// <param name="context">Contexte de l'exception</param>
        public void OnException(ExceptionContext context)
        {
            // Déterminer le code HTTP et la réponse d'erreur
            var (statusCode, errorResponse) = MapExceptionToErrorResponse(context.Exception);

            // Enregistrer l'erreur avec le niveau approprié
            LogException(context.Exception, statusCode);

            // Configurer le résultat de l'exception
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)statusCode
            };

            // Marquer l'exception comme gérée
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Mappe une exception vers une réponse d'erreur appropriée.
        /// </summary>
        /// <param name="exception">Exception à mapper</param>
        /// <returns>Tuple contenant le code HTTP et la réponse d'erreur</returns>
        private (HttpStatusCode statusCode, ErrorResponse errorResponse) MapExceptionToErrorResponse(Exception exception)
        {
            var errorResponse = new ErrorResponse
            {
                Success = false,
                Timestamp = DateTime.UtcNow
            };

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
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
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
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                UnauthorizedException unauthorizedEx => (
                    HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = unauthorizedEx.ErrorCode,
                        Message = unauthorizedEx.Message,
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
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
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
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
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
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
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                // Exceptions système
                ArgumentException argumentEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "INVALID_ARGUMENT",
                        Message = "Un ou plusieurs arguments sont invalides.",
                        Details = new { argumentEx.ParamName, argumentEx.Message },
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                InvalidOperationException invalidOpEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "INVALID_OPERATION",
                        Message = "L'opération demandée n'est pas valide.",
                        Details = invalidOpEx.Message,
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                NotImplementedException notImplEx => (
                    HttpStatusCode.NotImplemented,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "NOT_IMPLEMENTED",
                        Message = "La fonctionnalité demandée n'est pas encore implémentée.",
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                TimeoutException timeoutEx => (
                    HttpStatusCode.RequestTimeout,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "TIMEOUT",
                        Message = "L'opération a expiré.",
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
                    }
                ),
                // Exception générale
                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Success = false,
                        ErrorCode = "INTERNAL_SERVER_ERROR",
                        Message = _isDevelopment ? exception.Message : "Une erreur interne s'est produite.",
                        Timestamp = DateTime.UtcNow,
                        StackTrace = _isDevelopment ? exception.StackTrace : null
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
            var logMessage = $"Exception dans le contrôleur - {exception.GetType().Name}";

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
                ArgumentException => LogLevel.Information,
                InvalidOperationException => LogLevel.Warning,
                BusinessException => LogLevel.Warning,
                // Les erreurs côté serveur (5xx) sont loggées en Error
                _ when (int)statusCode >= 500 => LogLevel.Error,
                // Par défaut, Warning
                _ => LogLevel.Warning
            };
        }
    }

    /// <summary>
    /// Attribut pour appliquer le filtre d'exception API aux contrôleurs ou actions spécifiques.
    /// </summary>
    public class ApiExceptionFilterAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initialise une nouvelle instance de l'attribut ApiExceptionFilter.
        /// </summary>
        public ApiExceptionFilterAttribute() : base(typeof(ApiExceptionFilter))
        {
        }
    }
}