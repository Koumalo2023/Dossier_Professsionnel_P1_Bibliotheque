using api.Models;

namespace api.Helpers
{
    /// <summary>
    /// Classe générique pour standardiser les réponses des services.
    /// </summary>
    /// <typeparam name="T">Type des données retournées par la réponse.</typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Indique si l'opération a réussi.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message descriptif de la réponse (par exemple, "User registered successfully").
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Données retournées par l'opération (par exemple, un token JWT ou un objet utilisateur).
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Liste des erreurs en cas d'échec de l'opération.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        // Nouvelle propriété pour les données utilisateur
        public UserDto User { get; set; }

        // Pour les mises à jour
        public UserDto UpdatedUser { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public ServiceResponse()
        {
            Success = false;
            Message = string.Empty;
            Data = default;
        }
    }
}
