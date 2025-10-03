using api.Helpers;
using api.Models;
using api.Repositories;
using AutoMapper;

namespace api.Services
{
    /// <summary>
    /// Service pour la gestion des réservations
    /// </summary>
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository reservationRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<ReservationDto>> CreateReservationAsync(Guid userId, CreateReservationDto createReservationDto)
        {
            try
            {
                // Vérifier si le livre existe
                var book = await _bookRepository.GetByIdAsync(createReservationDto.BookId);
                if (book == null)
                {
                    return new ServiceResponse<ReservationDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                // Vérifier si le livre est disponible (échec si disponible - utiliser /api/loans à la place)
                var isBookAvailable = await _reservationRepository.IsBookAvailableAsync(createReservationDto.BookId);
                if (isBookAvailable)
                {
                    return new ServiceResponse<ReservationDto> 
                    { 
                        Success = false, 
                        Message = "Le livre est disponible. Veuillez utiliser l'endpoint d'emprunt à la place." 
                    };
                }

                // Vérifier si l'utilisateur a déjà une réservation en attente pour ce livre
                var hasPendingReservation = await _reservationRepository.HasPendingReservationAsync(userId, createReservationDto.BookId);
                if (hasPendingReservation)
                {
                    return new ServiceResponse<ReservationDto> 
                    { 
                        Success = false, 
                        Message = "Vous avez déjà une réservation en attente pour ce livre" 
                    };
                }

                // Créer la réservation
                var reservation = _mapper.Map<Reservation>(createReservationDto);
                reservation.UserId = userId;
                reservation.Status = "PENDING";
                reservation.ReservationDate = DateTime.UtcNow;

                var createdReservation = await _reservationRepository.AddAsync(reservation);
                var reservationDto = _mapper.Map<ReservationDto>(createdReservation);

                return new ServiceResponse<ReservationDto> 
                { 
                    Success = true, 
                    Message = "Réservation créée avec succès",
                    Data = reservationDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la création de la réservation",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<ReservationDto>>> GetAllReservationsAsync(string? status = null, Guid? userId = null, Guid? bookId = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var reservationsResult = await _reservationRepository.GetAllAsync(status, userId, bookId, page, pageSize);
                var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservationsResult.Items);
                
                var pagedResult = new PagedResult<ReservationDto>(
                    reservationDtos, 
                    reservationsResult.TotalCount, 
                    reservationsResult.CurrentPage, 
                    reservationsResult.PageSize
                );

                return new ServiceResponse<PagedResult<ReservationDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<ReservationDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des réservations",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<ReservationDto>>> GetUserReservationsAsync(Guid userId, string? status = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var reservationsResult = await _reservationRepository.GetByUserIdAsync(userId, status, page, pageSize);
                var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservationsResult.Items);
                
                var pagedResult = new PagedResult<ReservationDto>(
                    reservationDtos, 
                    reservationsResult.TotalCount, 
                    reservationsResult.CurrentPage, 
                    reservationsResult.PageSize
                );

                return new ServiceResponse<PagedResult<ReservationDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<ReservationDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des réservations de l'utilisateur",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<ReservationDto>> GetReservationByIdAsync(Guid id)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(id);
                if (reservation == null)
                {
                    return new ServiceResponse<ReservationDto> 
                    { 
                        Success = false, 
                        Message = "Réservation non trouvée" 
                    };
                }

                var reservationDto = _mapper.Map<ReservationDto>(reservation);
                return new ServiceResponse<ReservationDto> 
                { 
                    Success = true, 
                    Data = reservationDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération de la réservation",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<bool>> CancelReservationAsync(Guid reservationId, Guid currentUserId, bool isAdminOrManager = false)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(reservationId);
                if (reservation == null)
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Réservation non trouvée" 
                    };
                }

                // Vérifier les permissions
                if (!isAdminOrManager && reservation.UserId != currentUserId)
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Vous n'êtes pas autorisé à annuler cette réservation" 
                    };
                }

                // Vérifier si la réservation peut être annulée
                var canCancel = await _reservationRepository.CanCancelAsync(reservationId);
                if (!canCancel)
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Impossible d'annuler cette réservation (déjà honorée ou expirée)" 
                    };
                }

                var result = await _reservationRepository.DeleteAsync(reservationId);
                if (!result)
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Erreur lors de l'annulation de la réservation" 
                    };
                }

                return new ServiceResponse<bool> 
                { 
                    Success = true, 
                    Message = "Réservation annulée avec succès",
                    Data = true 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> 
                { 
                    Success = false, 
                    Message = "Erreur lors de l'annulation de la réservation",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<bool>> CanReserveBookAsync(Guid bookId)
        {
            try
            {
                var isBookAvailable = await _reservationRepository.IsBookAvailableAsync(bookId);
                return new ServiceResponse<bool> 
                { 
                    Success = true, 
                    Data = !isBookAvailable,
                    Message = isBookAvailable ? "Le livre est disponible (utiliser emprunt)" : "Le livre peut être réservé"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la vérification de la réservation",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReservationDto>>> GetExpiredReservationsAsync()
        {
            try
            {
                var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync();
                var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(expiredReservations);
                
                return new ServiceResponse<IEnumerable<ReservationDto>> 
                { 
                    Success = true, 
                    Data = reservationDtos 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ReservationDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des réservations expirées",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReservationDto>>> GetAvailableReservationsAsync()
        {
            try
            {
                var availableReservations = await _reservationRepository.GetAvailableReservationsAsync();
                var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(availableReservations);
                
                return new ServiceResponse<IEnumerable<ReservationDto>> 
                { 
                    Success = true, 
                    Data = reservationDtos 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ReservationDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des réservations disponibles",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<ReservationDto>> UpdateReservationStatusAsync(Guid reservationId, string status, DateTime? availableSince = null)
        {
            try
            {
                var result = await _reservationRepository.UpdateStatusAsync(reservationId, status, availableSince);
                if (!result)
                {
                    return new ServiceResponse<ReservationDto> 
                    { 
                        Success = false, 
                        Message = "Réservation non trouvée" 
                    };
                }

                var updatedReservation = await _reservationRepository.GetByIdAsync(reservationId);
                var reservationDto = _mapper.Map<ReservationDto>(updatedReservation);
                
                return new ServiceResponse<ReservationDto> 
                { 
                    Success = true, 
                    Message = "Statut de la réservation mis à jour avec succès",
                    Data = reservationDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la mise à jour du statut de la réservation",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}