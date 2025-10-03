using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Représente un enregistrement de métriques analytiques pour le suivi des statistiques.
    /// </summary>
    public class AnalyticsLog
    {
        /// <summary>
        /// Identifiant unique de l'enregistrement analytique (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Type de métrique : "MostBorrowed", "ActiveUsers", "OverdueLoans", etc.
        /// </summary>
        [Required]
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
        /// Date de création de l'enregistrement.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}