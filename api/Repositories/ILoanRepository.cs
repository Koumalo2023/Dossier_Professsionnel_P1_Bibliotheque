using api.Helpers;
using api.Models;

namespace api.Repositories
{
    /// <summary>
    /// Interface pour le repository des emprunts
    /// </summary>
    public interface ILoanRepository
    {
        /// <summary>
        /// Récupère tous les emprunts avec pagination et filtres
        /// </summary>
        Task<PagedResult<Loan>> GetAllAsync(string? status = null, Guid? userId = null, bool? overdueOnly = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère un emprunt par son ID
        /// </summary>
        Task<Loan?> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère les emprunts d'un utilisateur spécifique
        /// </summary>
        Task<PagedResult<Loan>> GetByUserIdAsync(Guid userId, string? status = null, int limit = 20, int page = 1);

        /// <summary>
        /// Récupère les emprunts en retard
        /// </summary>
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();

        /// <summary>
        /// Vérifie si un utilisateur peut emprunter un livre (pas de copies disponibles ou réservations actives)
        /// </summary>
        Task<bool> CanBorrowBookAsync(Guid bookId, Guid userId);

        /// <summary>
        /// Vérifie si un emprunt peut être prolongé (pas encore prolongé et pas en retard)
        /// </summary>
        Task<bool> CanExtendLoanAsync(Guid loanId);

        /// <summary>
        /// Vérifie si un livre a des copies disponibles
        /// </summary>
        Task<bool> IsBookAvailableAsync(Guid bookId);

        /// <summary>
        /// Vérifie si un utilisateur a déjà emprunté un livre spécifique et ne l'a pas encore retourné
        /// </summary>
        Task<bool> HasActiveLoanAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Récupère le nombre d'emprunts actifs d'un utilisateur
        /// </summary>
        Task<int> GetActiveLoansCountAsync(Guid userId);

        /// <summary>
        /// Ajoute un nouvel emprunt
        /// </summary>
        Task<Loan> AddAsync(Loan loan);

        /// <summary>
        /// Met à jour un emprunt existant
        /// </summary>
        Task<Loan> UpdateAsync(Loan loan);

        /// <summary>
        /// Supprime un emprunt
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Marque un emprunt comme retourné
        /// </summary>
        Task<bool> ReturnLoanAsync(Guid loanId, DateTime returnDate);

        /// <summary>
        /// Prolonge un emprunt (ajoute des jours à la date de retour)
        /// </summary>
        Task<bool> ExtendLoanAsync(Guid loanId, int additionalDays);

        /// <summary>
        /// Récupère les statistiques d'emprunt d'un utilisateur
        /// </summary>
        Task<UserLoanStats> GetUserLoanStatsAsync(Guid userId);

        /// <summary>
        /// Récupère le nombre total d'emprunts en retard
        /// </summary>
        Task<int> GetTotalOverdueCountAsync();
    }

    /// <summary>
    /// Statistiques d'emprunt d'un utilisateur
    /// </summary>
    public class UserLoanStats
    {
        public int TotalBorrowed { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int OverdueCount { get; set; }
    }
}