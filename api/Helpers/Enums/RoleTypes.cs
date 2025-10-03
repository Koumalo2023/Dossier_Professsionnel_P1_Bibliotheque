namespace api.Helpers.Enums
{
    /// <summary>
    /// Définit les types de rôles disponibles dans l'application
    /// </summary>
    public static class RoleTypes
    {
        /// <summary>
        /// Rôle administrateur avec accès complet
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Rôle manager avec droits de gestion
        /// </summary>
        public const string Manager = "Manager";

        /// <summary>
        /// Rôle utilisateur standard
        /// </summary>
        public const string User = "User";
    }
}