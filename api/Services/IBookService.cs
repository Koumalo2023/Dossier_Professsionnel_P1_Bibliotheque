using api.Helpers;
using api.Models;

namespace api.Services
{
    /// <summary>
    /// Interface pour le service de gestion des livres
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Récupère tous les livres avec pagination et filtres
        /// </summary>
        Task<ServiceResponse<PagedResult<BookDto>>> GetAllBooksAsync(string? category = null, string? author = null, bool? availableOnly = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère un livre par son ID
        /// </summary>
        Task<ServiceResponse<BookDto>> GetBookByIdAsync(Guid id);

        /// <summary>
        /// Recherche des livres avec critères avancés
        /// </summary>
        Task<ServiceResponse<PagedResult<BookDto>>> SearchBooksAsync(string query, string? searchIn = "title,author", int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les livres par catégorie
        /// </summary>
        Task<ServiceResponse<PagedResult<BookDto>>> GetBooksByCategoryAsync(Guid categoryId, bool? availableOnly = null, string? sortBy = "title", string? sortOrder = "asc", int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les livres récemment ajoutés
        /// </summary>
        Task<ServiceResponse<IEnumerable<BookDto>>> GetRecentBooksAsync(int days = 30, int limit = 10);

        /// <summary>
        /// Récupère les livres les plus populaires
        /// </summary>
        Task<ServiceResponse<IEnumerable<BookDto>>> GetPopularBooksAsync(string period = "MONTH", int limit = 10);

        /// <summary>
        /// Crée un nouveau livre
        /// </summary>
        Task<ServiceResponse<BookDto>> CreateBookAsync(CreateBookDto createBookDto);

        /// <summary>
        /// Met à jour un livre existant
        /// </summary>
        Task<ServiceResponse<BookDto>> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto);

        /// <summary>
        /// Supprime un livre
        /// </summary>
        Task<ServiceResponse<bool>> DeleteBookAsync(Guid id);

        /// <summary>
        /// Ajoute des exemplaires à un livre
        /// </summary>
        Task<ServiceResponse<BookDto>> AddCopiesAsync(Guid bookId, int quantity, string reason);

        /// <summary>
        /// Met à jour la disponibilité d'un livre
        /// </summary>
        Task<ServiceResponse<BookDto>> UpdateAvailabilityAsync(Guid bookId, bool available, string? reason = null);

        /// <summary>
        /// Récupère les statistiques d'un livre
        /// </summary>
        Task<ServiceResponse<BookStatsDto>> GetBookStatsAsync(Guid bookId);

        /// <summary>
        /// Vérifie si un ISBN existe déjà
        /// </summary>
        Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null);

        /// <summary>
        /// Récupère toutes les catégories
        /// </summary>
        Task<ServiceResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();

        /// <summary>
        /// Récupère une catégorie par son ID
        /// </summary>
        Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Crée une nouvelle catégorie
        /// </summary>
        Task<ServiceResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        /// <summary>
        /// Met à jour une catégorie existante
        /// </summary>
        Task<ServiceResponse<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto);

        /// <summary>
        /// Supprime une catégorie (si non utilisée)
        /// </summary>
        Task<ServiceResponse<bool>> DeleteCategoryAsync(Guid id);
    }

    /// <summary>
    /// DTO pour les statistiques d'un livre
    /// </summary>
    public class BookStatsDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public int TotalBorrows { get; set; }
        public int CurrentActiveLoans { get; set; }
        public int TotalReservations { get; set; }
        public double AverageLoanDurationDays { get; set; }
        public int PopularityRank { get; set; }
        public DateTime? LastBorrowed { get; set; }
    }
}