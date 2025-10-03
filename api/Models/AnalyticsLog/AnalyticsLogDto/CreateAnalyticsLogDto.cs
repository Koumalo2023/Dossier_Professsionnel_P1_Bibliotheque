using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour créer un nouvel enregistrement de métriques analytiques.
    /// </summary>
    public class CreateAnalyticsLogDto
    {
        /// <summary>
        /// Type de métrique : "MostBorrowed", "ActiveUsers", "OverdueLoans", etc.
        /// </summary>
        [Required(ErrorMessage = "Le type de métrique est obligatoire.")]
        public string MetricType { get; set; } = null!;

        /// <summary>
        /// Valeur numérique de la métrique.
        /// </summary>
        [Required(ErrorMessage = "La valeur est obligatoire.")]
        public int Value { get; set; }

        /// <summary>
        /// Date de référence (ex: mois, jour).
        /// </summary>
        [Required(ErrorMessage = "La date de référence est obligatoire.")]
        public DateTime ReferenceDate { get; set; }

        /// <summary>
        /// Optionnel : ID du livre ou utilisateur associé.
        /// </summary>
        public Guid? ReferenceId { get; set; }
    }
}