using System;
using System.Runtime.Serialization;

namespace api.Exceptions
{
    /// <summary>
    /// Exception de base pour les erreurs métier de l'application.
    /// </summary>
    [Serializable]
    public class BusinessException : Exception
    {
        /// <summary>
        /// Code d'erreur unique identifiant le type d'erreur.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Détails supplémentaires sur l'erreur.
        /// </summary>
        public object Details { get; }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException.
        /// </summary>
        public BusinessException() : base("Une erreur métier s'est produite.")
        {
            ErrorCode = "BUSINESS_ERROR";
        }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public BusinessException(string message) : base(message)
        {
            ErrorCode = "BUSINESS_ERROR";
        }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException avec un message et un code d'erreur.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        /// <param name="errorCode">Code d'erreur unique</param>
        public BusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException avec un message, un code d'erreur et des détails.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        /// <param name="errorCode">Code d'erreur unique</param>
        /// <param name="details">Détails supplémentaires sur l'erreur</param>
        public BusinessException(string message, string errorCode, object details) : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException avec un message et une exception interne.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        /// <param name="innerException">Exception à l'origine de cette exception</param>
        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = "BUSINESS_ERROR";
        }

        /// <summary>
        /// Initialise une nouvelle instance de BusinessException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ErrorCode = info.GetString(nameof(ErrorCode)) ?? "BUSINESS_ERROR";
            Details = info.GetValue(nameof(Details), typeof(object));
        }

        /// <summary>
        /// Définit les données de sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(Details), Details);
        }
    }

    /// <summary>
    /// Exception levée lorsqu'une ressource n'est pas trouvée.
    /// </summary>
    [Serializable]
    public class NotFoundException : BusinessException
    {
        /// <summary>
        /// Type de la ressource non trouvée.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// Identifiant de la ressource non trouvée.
        /// </summary>
        public object ResourceId { get; }

        /// <summary>
        /// Initialise une nouvelle instance de NotFoundException.
        /// </summary>
        public NotFoundException() : base("La ressource demandée n'a pas été trouvée.", "RESOURCE_NOT_FOUND")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de NotFoundException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public NotFoundException(string message) : base(message, "RESOURCE_NOT_FOUND")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de NotFoundException pour une ressource spécifique.
        /// </summary>
        /// <param name="resourceType">Type de la ressource</param>
        /// <param name="resourceId">Identifiant de la ressource</param>
        public NotFoundException(string resourceType, object resourceId) 
            : base($"{resourceType} avec l'identifiant {resourceId} n'a pas été trouvé(e).", "RESOURCE_NOT_FOUND")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        /// <summary>
        /// Initialise une nouvelle instance de NotFoundException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceType = info.GetString(nameof(ResourceType));
            ResourceId = info.GetValue(nameof(ResourceId), typeof(object));
        }

        /// <summary>
        /// Définit les données de sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ResourceType), ResourceType);
            info.AddValue(nameof(ResourceId), ResourceId);
        }
    }

    /// <summary>
    /// Exception levée lorsqu'une validation échoue.
    /// </summary>
    [Serializable]
    public class ValidationException : BusinessException
    {
        /// <summary>
        /// Détails des erreurs de validation.
        /// </summary>
        public new object Details { get; }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException.
        /// </summary>
        public ValidationException() : base("Une ou plusieurs validations ont échoué.", "VALIDATION_ERROR")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public ValidationException(string message) : base(message, "VALIDATION_ERROR")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException avec des erreurs de validation.
        /// </summary>
        /// <param name="validationErrors">Erreurs de validation</param>
        public ValidationException(object validationErrors) 
            : base("Une ou plusieurs validations ont échoué.", "VALIDATION_ERROR", validationErrors)
        {
            Details = validationErrors;
        }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Details = info.GetValue(nameof(Details), typeof(object));
        }

        /// <summary>
        /// Définit les données de sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Details), Details);
        }
    }

    /// <summary>
    /// Exception levée lorsqu'un accès non autorisé est détecté.
    /// </summary>
    [Serializable]
    public class UnauthorizedException : BusinessException
    {
        /// <summary>
        /// Initialise une nouvelle instance de UnauthorizedException.
        /// </summary>
        public UnauthorizedException() : base("Accès non autorisé.", "UNAUTHORIZED")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de UnauthorizedException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public UnauthorizedException(string message) : base(message, "UNAUTHORIZED")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de UnauthorizedException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Exception levée lorsqu'un conflit est détecté (ex: ressource déjà existante).
    /// </summary>
    [Serializable]
    public class ConflictException : BusinessException
    {
        /// <summary>
        /// Type de la ressource en conflit.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// Identifiant de la ressource en conflit.
        /// </summary>
        public object ResourceId { get; }

        /// <summary>
        /// Initialise une nouvelle instance de ConflictException.
        /// </summary>
        public ConflictException() : base("Un conflit a été détecté.", "CONFLICT")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ConflictException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public ConflictException(string message) : base(message, "CONFLICT")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ConflictException pour une ressource spécifique.
        /// </summary>
        /// <param name="resourceType">Type de la ressource</param>
        /// <param name="resourceId">Identifiant de la ressource</param>
        public ConflictException(string resourceType, object resourceId) 
            : base($"{resourceType} avec l'identifiant {resourceId} existe déjà.", "CONFLICT")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        /// <summary>
        /// Initialise une nouvelle instance de ConflictException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceType = info.GetString(nameof(ResourceType));
            ResourceId = info.GetValue(nameof(ResourceId), typeof(object));
        }

        /// <summary>
        /// Définit les données de sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ResourceType), ResourceType);
            info.AddValue(nameof(ResourceId), ResourceId);
        }
    }

    /// <summary>
    /// Exception levée lorsqu'une opération n'est pas autorisée en raison de règles métier.
    /// </summary>
    [Serializable]
    public class ForbiddenException : BusinessException
    {
        /// <summary>
        /// Type d'opération interdite.
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Raison de l'interdiction.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Initialise une nouvelle instance de ForbiddenException.
        /// </summary>
        public ForbiddenException() : base("Opération interdite.", "FORBIDDEN")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ForbiddenException avec un message spécifique.
        /// </summary>
        /// <param name="message">Message décrivant l'erreur</param>
        public ForbiddenException(string message) : base(message, "FORBIDDEN")
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ForbiddenException avec des détails spécifiques.
        /// </summary>
        /// <param name="operation">Type d'opération interdite</param>
        /// <param name="reason">Raison de l'interdiction</param>
        public ForbiddenException(string operation, string reason) 
            : base($"Opération '{operation}' interdite : {reason}", "FORBIDDEN")
        {
            Operation = operation;
            Reason = reason;
        }

        /// <summary>
        /// Initialise une nouvelle instance de ForbiddenException pour la sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Operation = info.GetString(nameof(Operation));
            Reason = info.GetString(nameof(Reason));
        }

        /// <summary>
        /// Définit les données de sérialisation.
        /// </summary>
        /// <param name="info">Informations de sérialisation</param>
        /// <param name="context">Contexte de sérialisation</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Operation), Operation);
            info.AddValue(nameof(Reason), Reason);
        }
    }
}