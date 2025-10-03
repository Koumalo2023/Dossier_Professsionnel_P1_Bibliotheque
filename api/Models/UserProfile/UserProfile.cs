using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models
{
    /// <summary>
    /// Profil utilisateur enrichi avec préférences et statistiques
    /// </summary>
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        // Préférences de lecture
        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(100)]
        public string? FavoriteAuthor { get; set; }

        [MaxLength(100)]
        public string? FavoriteGenre { get; set; }

        // Paramètres de notification
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool NewBookAlerts { get; set; } = true;
        public bool ReturnReminders { get; set; } = true;
        public bool ReviewReminders { get; set; } = false;

        // Préférences d'affichage
        public string Theme { get; set; } = "light"; // light, dark, auto
        public int ItemsPerPage { get; set; } = 20;
        public string Language { get; set; } = "fr";

        // Statistiques de lecture
        public int TotalBooksRead { get; set; } = 0;
        public int TotalPagesRead { get; set; } = 0;
        public double AverageReadingTime { get; set; } = 0; // en minutes
        public string? ReadingLevel { get; set; } // beginner, intermediate, advanced

        // Métadonnées
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserCategoryPreference> CategoryPreferences { get; set; } = new List<UserCategoryPreference>();
        public virtual ICollection<UserReadingGoal> ReadingGoals { get; set; } = new List<UserReadingGoal>();
        public virtual ICollection<UserReadingHistory> ReadingHistory { get; set; } = new List<UserReadingHistory>();
    }

    /// <summary>
    /// Préférences par catégorie pour les recommandations
    /// </summary>
    public class UserCategoryPreference
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public UserProfile UserProfile { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [Range(1, 10)]
        public int PreferenceScore { get; set; } = 5; // 1-10

        public DateTime LastInteracted { get; set; } = DateTime.UtcNow;
        public int InteractionCount { get; set; } = 1;

        // Métadonnées
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Objectifs de lecture de l'utilisateur
    /// </summary>
    public class UserReadingGoal
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public UserProfile UserProfile { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int TargetBooks { get; set; }

        public int CurrentProgress { get; set; } = 0;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Historique de lecture détaillé
    /// </summary>
    public class UserReadingHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public UserProfile UserProfile { get; set; } = null!;

        [Required]
        public Guid BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        [Required]
        public Guid LoanId { get; set; }

        [ForeignKey("LoanId")]
        public Loan Loan { get; set; } = null!;
        [Required]
        public DateTime StartedReading { get; set; }

        public DateTime? FinishedReading { get; set; }

        public int? ReadingTimeMinutes { get; set; } // Temps total de lecture en minutes

        [Range(1, 5)]
        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }

        public bool IsFavorite { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}