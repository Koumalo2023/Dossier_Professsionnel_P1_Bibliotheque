using System;

namespace api.Models
{
    /// <summary>
    /// DTO pour représenter un enregistrement de métriques analytiques.
    /// </summary>
    public class AnalyticsLogDto
    {
        /// <summary>
        /// Identifiant unique de l'enregistrement analytique.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Type de métrique : "MostBorrowed", "ActiveUsers", "OverdueLoans", etc.
        /// </summary>
        public string MetricType { get; set; } = null!;

        /// <summary>
        /// Valeur numérique de la métrique.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Date de référence (ex: mois, jour).
        /// </summary>
        public DateTime ReferenceDate { get; set; }

        /// <summary>
        /// Optionnel : ID du livre ou utilisateur associé.
        /// </summary>
        public Guid? ReferenceId { get; set; }

        /// <summary>
        /// Titre du livre associé (si applicable).
        /// </summary>
        public string? BookTitle { get; set; }

        /// <summary>
        /// Nom de l'utilisateur associé (si applicable).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Date de création de l'enregistrement.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}