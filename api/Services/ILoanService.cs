using api.Helpers;
using api.Models;

namespace api.Services
{
    /// <summary>
    /// Interface pour le service de gestion des emprunts
    /// </summary>
    public interface ILoanService
    {
        /// <summary>
        /// Crée un nouvel emprunt pour un utilisateur
        /// </summary>
        Task<ServiceResponse<LoanDto>> CreateLoanAsync(Guid userId, CreateLoanDto createLoanDto);

        /// <summary>
        /// Récupère tous les emprunts avec pagination et filtres
        /// </summary>
        Task<ServiceResponse<PagedResult<LoanDto>>> GetAllLoansAsync(string? status = null, Guid? userId = null, bool? overdueOnly = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les emprunts d'un utilisateur spécifique
        /// </summary>
        Task<ServiceResponse<UserLoanHistoryResponse>> GetUserLoanHistoryAsync(Guid userId, string? status = null, int limit = 20);

        /// <summary>
        /// Récupère les emprunts en retard
        /// </summary>
        Task<ServiceResponse<OverdueLoansResponse>> GetOverdueLoansAsync();

        /// <summary>
        /// Récupère un emprunt par son ID
        /// </summary>
        Task<ServiceResponse<LoanDto>> GetLoanByIdAsync(Guid id);

        /// <summary>
        /// Retourne un livre emprunté
        /// </summary>
        Task<ServiceResponse<LoanDto>> ReturnLoanAsync(Guid loanId, Guid currentUserId, bool isAdminOrManager = false);

        /// <summary>
        /// Prolonge un emprunt
        /// </summary>
        Task<ServiceResponse<LoanDto>> ExtendLoanAsync(Guid loanId, Guid currentUserId, int additionalDays = 7);

        /// <summary>
        /// Envoie une notification de retard pour un emprunt
        /// </summary>
        Task<ServiceResponse<NotificationSentResponse>> SendOverdueNotificationAsync(Guid loanId);

        /// <summary>
        /// Vérifie si un utilisateur peut emprunter un livre
        /// </summary>
        Task<ServiceResponse<CanBorrowResponse>> CanBorrowBookAsync(Guid bookId, Guid userId);

        /// <summary>
        /// Récupère les statistiques d'emprunt d'un utilisateur
        /// </summary>
        Task<ServiceResponse<UserLoanStats>> GetUserLoanStatsAsync(Guid userId);
    }

    /// <summary>
    /// Réponse pour l'historique des emprunts d'un utilisateur
    /// </summary>
    public class UserLoanHistoryResponse
    {
        public IEnumerable<LoanDto> Items { get; set; } = new List<LoanDto>();
        public int TotalBorrowed { get; set; }
        public int CurrentlyBorrowed { get; set; }
    }

    /// <summary>
    /// Réponse pour les emprunts en retard
    /// </summary>
    public class OverdueLoansResponse
    {
        public IEnumerable<OverdueLoanDto> Items { get; set; } = new List<OverdueLoanDto>();
        public int TotalOverdue { get; set; }
    }

    /// <summary>
    /// DTO pour un emprunt en retard
    /// </summary>
    public class OverdueLoanDto
    {
        public Guid Id { get; set; }
        public BookInfoDto Book { get; set; } = null!;
        public UserInfoDto User { get; set; } = null!;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }

    /// <summary>
    /// Informations sur le livre pour les DTOs d'emprunt
    /// </summary>
    public class BookInfoDto
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
    }

    /// <summary>
    /// Informations sur l'utilisateur pour les DTOs d'emprunt
    /// </summary>
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    /// <summary>
    /// Réponse pour l'envoi de notification
    /// </summary>
    public class NotificationSentResponse
    {
        public string Message { get; set; } = null!;
        public Guid NotificationId { get; set; }
    }

    /// <summary>
    /// Réponse pour la vérification d'emprunt
    /// </summary>
    public class CanBorrowResponse
    {
        public bool CanBorrow { get; set; }
        public string Reason { get; set; } = null!;
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