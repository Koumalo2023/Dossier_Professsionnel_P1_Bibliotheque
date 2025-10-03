using api.Helpers;
using api.Models;

namespace api.Repositories
{
    /// <summary>
    /// Interface pour le repository des livres
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Récupère tous les livres avec pagination et filtres
        /// </summary>
        Task<PagedResult<Book>> GetAllAsync(string? category = null, string? author = null, bool? availableOnly = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère un livre par son ID
        /// </summary>
        Task<Book?> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère un livre par son ISBN
        /// </summary>
        Task<Book?> GetByIsbnAsync(string isbn);

        /// <summary>
        /// Recherche des livres avec critères avancés
        /// </summary>
        Task<PagedResult<Book>> SearchAsync(string query, string? searchIn = "title,author,description", int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les livres par catégorie
        /// </summary>
        Task<PagedResult<Book>> GetByCategoryAsync(Guid categoryId, bool? availableOnly = null, string? sortBy = "title", string? sortOrder = "asc", int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les livres récemment ajoutés
        /// </summary>
        Task<IEnumerable<Book>> GetRecentAsync(int days = 30, int limit = 10);

        /// <summary>
        /// Récupère les livres les plus populaires
        /// </summary>
        Task<IEnumerable<Book>> GetPopularAsync(string period = "MONTH", int limit = 10);

        /// <summary>
        /// Ajoute un nouveau livre
        /// </summary>
        Task<Book> AddAsync(Book book);

        /// <summary>
        /// Met à jour un livre existant
        /// </summary>
        Task<Book> UpdateAsync(Book book);

        /// <summary>
        /// Supprime un livre
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Vérifie si un livre a des emprunts ou réservations actifs
        /// </summary>
        Task<bool> HasActiveLoansOrReservationsAsync(Guid bookId);

        /// <summary>
        /// Ajoute des exemplaires à un livre
        /// </summary>
        Task<bool> AddCopiesAsync(Guid bookId, int quantity);

        /// <summary>
        /// Met à jour la disponibilité d'un livre
        /// </summary>
        Task<bool> UpdateAvailabilityAsync(Guid bookId, bool available, string? reason = null);

        /// <summary>
        /// Récupère les statistiques d'un livre
        /// </summary>
        Task<BookStatsDto> GetStatsAsync(Guid bookId);

        /// <summary>
        /// Vérifie si un ISBN existe déjà
        /// </summary>
        Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null);

        /// <summary>
        /// Récupère toutes les catégories
        /// </summary>
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Récupère une catégorie par son ID
        /// </summary>
        Task<Category?> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Récupère une catégorie par son nom
        /// </summary>
        Task<Category?> GetCategoryByNameAsync(string name);

        /// <summary>
        /// Crée une nouvelle catégorie
        /// </summary>
        Task<Category> CreateCategoryAsync(Category category);

        /// <summary>
        /// Met à jour une catégorie existante
        /// </summary>
        Task<Category> UpdateCategoryAsync(Category category);

        /// <summary>
        /// Supprime une catégorie (si non utilisée)
        /// </summary>
        Task<bool> DeleteCategoryAsync(Guid id);

        /// <summary>
        /// Vérifie si une catégorie a des livres associés
        /// </summary>
        Task<bool> CategoryHasBooksAsync(Guid categoryId);
    }

    
}