using api.Data;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des réservations
    /// </summary>
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Reservation>> GetAllAsync(string? status = null, Guid? userId = null, Guid? bookId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId.Value);
            }

            if (bookId.HasValue)
            {
                query = query.Where(r => r.BookId == bookId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.ReservationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Reservation>(items, totalCount, page, pageSize);
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PagedResult<Reservation>> GetByUserIdAsync(Guid userId, string? status = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.ReservationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Reservation>(items, totalCount, page, pageSize);
        }

        public async Task<bool> HasPendingReservationAsync(Guid userId, Guid bookId)
        {
            return await _context.Reservations
                .AnyAsync(r => r.UserId == userId && 
                              r.BookId == bookId && 
                              r.Status == "PENDING");
        }

        public async Task<bool> IsBookAvailableAsync(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            // Vérifier s'il y a des copies disponibles
            if (book.AvailableCopies <= 0) return false;

            // Vérifier s'il y a des emprunts actifs
            var hasActiveLoans = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.Status == "BORROWED");

            return !hasActiveLoans;
        }

        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanCancelAsync(Guid reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return false;

            return reservation.Status != "FULFILLED" && reservation.Status != "EXPIRED";
        }

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.Status == "AVAILABLE" && 
                           r.ExpiryDate.HasValue && 
                           r.ExpiryDate.Value < now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAvailableReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.Status == "AVAILABLE")
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid reservationId, string status, DateTime? availableSince = null)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return false;

            reservation.Status = status;
            if (availableSince.HasValue)
            {
                reservation.AvailableSince = availableSince.Value;
                // Définir la date d'expiration (ex: 48 heures)
                reservation.ExpiryDate = availableSince.Value.AddHours(48);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}