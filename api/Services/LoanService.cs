using api.Helpers;
using api.Models;
using api.Repositories;
using AutoMapper;

namespace api.Services
{
    /// <summary>
    /// Service pour la gestion des emprunts
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<LoanDto>> CreateLoanAsync(Guid userId, CreateLoanDto createLoanDto)
        {
            try
            {
                // Vérifier si le livre existe
                var book = await _bookRepository.GetByIdAsync(createLoanDto.BookId);
                if (book == null)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                // Vérifier si l'utilisateur peut emprunter le livre
                var canBorrow = await _loanRepository.CanBorrowBookAsync(createLoanDto.BookId, userId);
                if (!canBorrow)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Impossible d'emprunter ce livre : aucune copie disponible ou réservé par un autre utilisateur" 
                    };
                }

                // Créer l'emprunt
                var loan = _mapper.Map<Loan>(createLoanDto);
                loan.UserId = userId;
                loan.Status = "BORROWED";
                loan.LoanDate = DateTime.UtcNow;

                var createdLoan = await _loanRepository.AddAsync(loan);
                var loanDto = _mapper.Map<LoanDto>(createdLoan);

                // Mettre à jour la disponibilité du livre
                await _bookRepository.UpdateAvailabilityAsync(createLoanDto.BookId, book.AvailableCopies - 1 > 0);

                return new ServiceResponse<LoanDto> 
                { 
                    Success = true, 
                    Message = "Emprunt créé avec succès",
                    Data = loanDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoanDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la création de l'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<LoanDto>>> GetAllLoansAsync(string? status = null, Guid? userId = null, bool? overdueOnly = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var loansResult = await _loanRepository.GetAllAsync(status, userId, overdueOnly, page, pageSize);
                var loanDtos = _mapper.Map<IEnumerable<LoanDto>>(loansResult.Items);
                
                var pagedResult = new PagedResult<LoanDto>(
                    loanDtos, 
                    loansResult.TotalCount, 
                    loansResult.CurrentPage, 
                    loansResult.PageSize
                );

                return new ServiceResponse<PagedResult<LoanDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<LoanDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des emprunts",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<UserLoanHistoryResponse>> GetUserLoanHistoryAsync(Guid userId, string? status = null, int limit = 20)
        {
            try
            {
                var loansResult = await _loanRepository.GetByUserIdAsync(userId, status, limit, 1);
                var loanDtos = _mapper.Map<IEnumerable<LoanDto>>(loansResult.Items);
                
                var stats = await _loanRepository.GetUserLoanStatsAsync(userId);

                var response = new UserLoanHistoryResponse
                {
                    Items = loanDtos,
                    TotalBorrowed = stats.TotalBorrowed,
                    CurrentlyBorrowed = stats.CurrentlyBorrowed
                };

                return new ServiceResponse<UserLoanHistoryResponse> 
                { 
                    Success = true, 
                    Data = response 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserLoanHistoryResponse> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération de l'historique des emprunts",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<OverdueLoansResponse>> GetOverdueLoansAsync()
        {
            try
            {
                var overdueLoans = await _loanRepository.GetOverdueLoansAsync();
                var totalOverdue = await _loanRepository.GetTotalOverdueCountAsync();

                var overdueLoanDtos = overdueLoans.Select(loan => new OverdueLoanDto
                {
                    Id = loan.Id,
                    Book = new BookInfoDto
                    {
                        Title = loan.Book.Title,
                        Author = loan.Book.Author
                    },
                    User = new UserInfoDto
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name,
                        Email = loan.User.Email
                    },
                    LoanDate = loan.LoanDate,
                    DueDate = loan.DueDate,
                    DaysOverdue = (int)(DateTime.UtcNow - loan.DueDate).TotalDays
                });

                var response = new OverdueLoansResponse
                {
                    Items = overdueLoanDtos,
                    TotalOverdue = totalOverdue
                };

                return new ServiceResponse<OverdueLoansResponse> 
                { 
                    Success = true, 
                    Data = response 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<OverdueLoansResponse> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des emprunts en retard",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<LoanDto>> GetLoanByIdAsync(Guid id)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(id);
                if (loan == null)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Emprunt non trouvé" 
                    };
                }

                var loanDto = _mapper.Map<LoanDto>(loan);
                return new ServiceResponse<LoanDto> 
                { 
                    Success = true, 
                    Data = loanDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoanDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération de l'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<LoanDto>> ReturnLoanAsync(Guid loanId, Guid currentUserId, bool isAdminOrManager = false)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Emprunt non trouvé" 
                    };
                }

                // Vérifier les permissions
                if (!isAdminOrManager && loan.UserId != currentUserId)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Vous n'êtes pas autorisé à retourner cet emprunt" 
                    };
                }

                // Vérifier si l'emprunt n'est pas déjà retourné
                if (loan.Status == "RETURNED")
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Cet emprunt a déjà été retourné" 
                    };
                }

                var result = await _loanRepository.ReturnLoanAsync(loanId, DateTime.UtcNow);
                if (!result)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Erreur lors du retour de l'emprunt" 
                    };
                }

                // Mettre à jour la disponibilité du livre
                var book = await _bookRepository.GetByIdAsync(loan.BookId);
                if (book != null)
                {
                    await _bookRepository.UpdateAvailabilityAsync(loan.BookId, book.AvailableCopies + 1 > 0);
                }

                var updatedLoan = await _loanRepository.GetByIdAsync(loanId);
                var loanDto = _mapper.Map<LoanDto>(updatedLoan);
                
                return new ServiceResponse<LoanDto> 
                { 
                    Success = true, 
                    Message = "Livre retourné avec succès",
                    Data = loanDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoanDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors du retour de l'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<LoanDto>> ExtendLoanAsync(Guid loanId, Guid currentUserId, int additionalDays = 7)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Emprunt non trouvé" 
                    };
                }

                // Vérifier les permissions
                if (loan.UserId != currentUserId)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Vous n'êtes pas autorisé à prolonger cet emprunt" 
                    };
                }

                // Vérifier si l'emprunt peut être prolongé
                var canExtend = await _loanRepository.CanExtendLoanAsync(loanId);
                if (!canExtend)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Impossible de prolonger cet emprunt (déjà en retard ou déjà prolongé)" 
                    };
                }

                var result = await _loanRepository.ExtendLoanAsync(loanId, additionalDays);
                if (!result)
                {
                    return new ServiceResponse<LoanDto> 
                    { 
                        Success = false, 
                        Message = "Erreur lors de la prolongation de l'emprunt" 
                    };
                }

                var updatedLoan = await _loanRepository.GetByIdAsync(loanId);
                var loanDto = _mapper.Map<LoanDto>(updatedLoan);
                
                return new ServiceResponse<LoanDto> 
                { 
                    Success = true, 
                    Message = $"Emprunt prolongé de {additionalDays} jours avec succès",
                    Data = loanDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoanDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la prolongation de l'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<NotificationSentResponse>> SendOverdueNotificationAsync(Guid loanId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                {
                    return new ServiceResponse<NotificationSentResponse> 
                    { 
                        Success = false, 
                        Message = "Emprunt non trouvé" 
                    };
                }

                // Vérifier si l'emprunt est effectivement en retard
                if (loan.DueDate >= DateTime.UtcNow || loan.Status != "BORROWED")
                {
                    return new ServiceResponse<NotificationSentResponse> 
                    { 
                        Success = false, 
                        Message = "Cet emprunt n'est pas en retard" 
                    };
                }

                // Simuler l'envoi de notification (pour l'instant, on retourne un GUID fictif)
                var notificationId = Guid.NewGuid();
                
                var response = new NotificationSentResponse
                {
                    Message = "Notification envoyée avec succès",
                    NotificationId = notificationId
                };

                return new ServiceResponse<NotificationSentResponse> 
                { 
                    Success = true, 
                    Data = response 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<NotificationSentResponse> 
                { 
                    Success = false, 
                    Message = "Erreur lors de l'envoi de la notification",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<CanBorrowResponse>> CanBorrowBookAsync(Guid bookId, Guid userId)
        {
            try
            {
                var canBorrow = await _loanRepository.CanBorrowBookAsync(bookId, userId);
                var reason = canBorrow ? 
                    "Livre disponible pour l'emprunt" : 
                    "Impossible d'emprunter ce livre (aucune copie disponible ou réservé par un autre utilisateur)";

                var response = new CanBorrowResponse
                {
                    CanBorrow = canBorrow,
                    Reason = reason
                };

                return new ServiceResponse<CanBorrowResponse> 
                { 
                    Success = true, 
                    Data = response 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CanBorrowResponse> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la vérification de l'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<UserLoanStats>> GetUserLoanStatsAsync(Guid userId)
        {
            try
            {
                var stats = await _loanRepository.GetUserLoanStatsAsync(userId);
                var serviceStats = new UserLoanStats
                {
                    TotalBorrowed = stats.TotalBorrowed,
                    CurrentlyBorrowed = stats.CurrentlyBorrowed,
                    OverdueCount = stats.OverdueCount
                };
                
                return new ServiceResponse<UserLoanStats>
                {
                    Success = true,
                    Data = serviceStats
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserLoanStats>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des statistiques d'emprunt",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}