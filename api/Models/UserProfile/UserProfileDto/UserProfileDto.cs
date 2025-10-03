using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour la représentation du profil utilisateur
    /// </summary>
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        // Informations personnelles
        [MaxLength(500, ErrorMessage = "La biographie ne peut pas dépasser 500 caractères.")]
        public string? Bio { get; set; }

        [MaxLength(100, ErrorMessage = "Le nom de l'auteur favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteAuthor { get; set; }

        [MaxLength(100, ErrorMessage = "Le genre favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteGenre { get; set; }

        // Paramètres de notification
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool NewBookAlerts { get; set; } = true;
        public bool ReturnReminders { get; set; } = true;
        public bool ReviewReminders { get; set; } = false;

        // Préférences d'affichage
        public string Theme { get; set; } = "light";
        public int ItemsPerPage { get; set; } = 20;
        public string Language { get; set; } = "fr";

        // Statistiques de lecture
        public int TotalBooksRead { get; set; } = 0;
        public int TotalPagesRead { get; set; } = 0;
        public double AverageReadingTime { get; set; } = 0;
        public string? ReadingLevel { get; set; }

        // Métadonnées
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Données liées
        public List<UserCategoryPreferenceDto> CategoryPreferences { get; set; } = new();
        public List<UserReadingGoalDto> ReadingGoals { get; set; } = new();
        public List<UserReadingHistoryDto> ReadingHistory { get; set; } = new();
    }

    /// <summary>
    /// DTO pour la création d'un profil utilisateur
    /// </summary>
    public class CreateUserProfileDto
    {
        [MaxLength(500, ErrorMessage = "La biographie ne peut pas dépasser 500 caractères.")]
        public string? Bio { get; set; }

        [MaxLength(100, ErrorMessage = "Le nom de l'auteur favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteAuthor { get; set; }

        [MaxLength(100, ErrorMessage = "Le genre favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteGenre { get; set; }

        // Paramètres de notification
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool NewBookAlerts { get; set; } = true;
        public bool ReturnReminders { get; set; } = true;
        public bool ReviewReminders { get; set; } = false;

        // Préférences d'affichage
        public string Theme { get; set; } = "light";
        
        [Range(5, 100, ErrorMessage = "Le nombre d'éléments par page doit être compris entre 5 et 100.")]
        public int ItemsPerPage { get; set; } = 20;
        
        public string Language { get; set; } = "fr";
    }

    /// <summary>
    /// DTO pour la mise à jour du profil utilisateur
    /// </summary>
    public class UpdateUserProfileDto
    {
        [MaxLength(500, ErrorMessage = "La biographie ne peut pas dépasser 500 caractères.")]
        public string? Bio { get; set; }

        [MaxLength(100, ErrorMessage = "Le nom de l'auteur favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteAuthor { get; set; }

        [MaxLength(100, ErrorMessage = "Le genre favori ne peut pas dépasser 100 caractères.")]
        public string? FavoriteGenre { get; set; }

        // Paramètres de notification
        public bool? EmailNotifications { get; set; }
        public bool? PushNotifications { get; set; }
        public bool? NewBookAlerts { get; set; }
        public bool? ReturnReminders { get; set; }
        public bool? ReviewReminders { get; set; }

        // Préférences d'affichage
        public string? Theme { get; set; }
        
        [Range(5, 100, ErrorMessage = "Le nombre d'éléments par page doit être compris entre 5 et 100.")]
        public int? ItemsPerPage { get; set; }
        
        public string? Language { get; set; }
    }

    /// <summary>
    /// DTO pour les préférences de catégorie
    /// </summary>
    public class UserCategoryPreferenceDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        [Range(1, 10, ErrorMessage = "Le score de préférence doit être compris entre 1 et 10.")]
        public int PreferenceScore { get; set; } = 5;
        
        public DateTime LastInteracted { get; set; }
        public int InteractionCount { get; set; } = 1;
    }

    /// <summary>
    /// DTO pour la création/mise à jour des préférences de catégorie
    /// </summary>
    public class CreateUserCategoryPreferenceDto
    {
        [Required(ErrorMessage = "L'ID de la catégorie est obligatoire.")]
        public Guid CategoryId { get; set; }

        [Range(1, 10, ErrorMessage = "Le score de préférence doit être compris entre 1 et 10.")]
        public int PreferenceScore { get; set; } = 5;
    }

    /// <summary>
    /// DTO pour les objectifs de lecture
    /// </summary>
    public class UserReadingGoalDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Le titre de l'objectif est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le nombre de livres cible est obligatoire.")]
        [Range(1, 1000, ErrorMessage = "Le nombre de livres cible doit être compris entre 1 et 1000.")]
        public int TargetBooks { get; set; }

        public int CurrentProgress { get; set; } = 0;
        public double ProgressPercentage => TargetBooks > 0 ? (double)CurrentProgress / TargetBooks * 100 : 0;

        [Required(ErrorMessage = "La date de début est obligatoire.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire.")]
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un objectif de lecture
    /// </summary>
    public class CreateUserReadingGoalDto
    {
        [Required(ErrorMessage = "Le titre de l'objectif est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le nombre de livres cible est obligatoire.")]
        [Range(1, 1000, ErrorMessage = "Le nombre de livres cible doit être compris entre 1 et 1000.")]
        public int TargetBooks { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire.")]
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un objectif de lecture
    /// </summary>
    public class UpdateUserReadingGoalDto
    {
        [MaxLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Range(1, 1000, ErrorMessage = "Le nombre de livres cible doit être compris entre 1 et 1000.")]
        public int? TargetBooks { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour l'historique de lecture
    /// </summary>
    public class UserReadingHistoryDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid LoanId { get; set; }
        
        // Informations du livre
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string? BookCoverUrl { get; set; }

        public DateTime StartedReading { get; set; }
        public DateTime? FinishedReading { get; set; }
        public int? ReadingTimeMinutes { get; set; }

        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int? Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "L'avis ne peut pas dépasser 1000 caractères.")]
        public string? Review { get; set; }

        public bool IsFavorite { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un historique de lecture
    /// </summary>
    public class CreateUserReadingHistoryDto
    {
        [Required(ErrorMessage = "L'ID du livre est obligatoire.")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "L'ID de l'emprunt est obligatoire.")]
        public Guid LoanId { get; set; }

        [Required(ErrorMessage = "La date de début de lecture est obligatoire.")]
        public DateTime StartedReading { get; set; }

        public DateTime? FinishedReading { get; set; }
        public int? ReadingTimeMinutes { get; set; }

        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int? Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "L'avis ne peut pas dépasser 1000 caractères.")]
        public string? Review { get; set; }

        public bool IsFavorite { get; set; } = false;
    }

    /// <summary>
    /// DTO pour la mise à jour d'un historique de lecture
    /// </summary>
    public class UpdateUserReadingHistoryDto
    {
        public DateTime? FinishedReading { get; set; }
        public int? ReadingTimeMinutes { get; set; }

        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int? Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "L'avis ne peut pas dépasser 1000 caractères.")]
        public string? Review { get; set; }

        public bool? IsFavorite { get; set; }
    }

    /// <summary>
    /// DTO pour les statistiques de lecture de l'utilisateur
    /// </summary>
    public class UserReadingStatsDto
    {
        public int TotalBooksRead { get; set; }
        public int TotalPagesRead { get; set; }
        public double AverageReadingTime { get; set; }
        public int ActiveGoals { get; set; }
        public int CompletedGoals { get; set; }
        public string? FavoriteCategory { get; set; }
        public string? FavoriteAuthor { get; set; }
        public int TotalReadingTimeMinutes { get; set; }
        public double BooksPerMonth { get; set; }
        public Dictionary<string, int> BooksByCategory { get; set; } = new();
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}