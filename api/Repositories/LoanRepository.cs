using api.Data;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des emprunts
    /// </summary>
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Loan>> GetAllAsync(string? status = null, Guid? userId = null, bool? overdueOnly = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (userId.HasValue)
            {
                query = query.Where(l => l.UserId == userId.Value);
            }

            if (overdueOnly.HasValue && overdueOnly.Value)
            {
                var now = DateTime.UtcNow;
                query = query.Where(l => l.DueDate < now && l.Status == "BORROWED");
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.LoanDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Loan>(items, totalCount, page, pageSize);
        }

        public async Task<Loan?> GetByIdAsync(Guid id)
        {
            return await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<PagedResult<Loan>> GetByUserIdAsync(Guid userId, string? status = null, int limit = 20, int page = 1)
        {
            var query = _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .Where(l => l.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.LoanDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return new PagedResult<Loan>(items, totalCount, page, limit);
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .Where(l => l.DueDate < now && l.Status == "BORROWED")
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }

        public async Task<bool> CanBorrowBookAsync(Guid bookId, Guid userId)
        {
            // Vérifier si le livre a des copies disponibles
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.AvailableCopies <= 0)
                return false;

            // Vérifier si l'utilisateur a déjà un emprunt actif pour ce livre
            var hasActiveLoan = await _context.Loans
                .AnyAsync(l => l.UserId == userId && l.BookId == bookId && l.Status == "BORROWED");

            if (hasActiveLoan)
                return false;

            // Vérifier s'il y a des réservations actives pour ce livre par d'autres utilisateurs
            var hasActiveReservations = await _context.Reservations
                .AnyAsync(r => r.BookId == bookId && r.Status == "PENDING" && r.UserId != userId);

            return !hasActiveReservations;
        }

        public async Task<bool> CanExtendLoanAsync(Guid loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            // Vérifier si l'emprunt est déjà en retard
            if (loan.DueDate < DateTime.UtcNow && loan.Status == "BORROWED")
                return false;

            // Vérifier si l'emprunt n'a pas déjà été prolongé (simplifié - on pourrait ajouter un champ IsExtended)
            // Pour l'instant, on vérifie simplement que le statut est BORROWED
            return loan.Status == "BORROWED";
        }

        public async Task<bool> IsBookAvailableAsync(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            return book != null && book.AvailableCopies > 0;
        }

        public async Task<bool> HasActiveLoanAsync(Guid userId, Guid bookId)
        {
            return await _context.Loans
                .AnyAsync(l => l.UserId == userId && l.BookId == bookId && l.Status == "BORROWED");
        }

        public async Task<int> GetActiveLoansCountAsync(Guid userId)
        {
            return await _context.Loans
                .CountAsync(l => l.UserId == userId && l.Status == "BORROWED");
        }

        public async Task<Loan> AddAsync(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<Loan> UpdateAsync(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReturnLoanAsync(Guid loanId, DateTime returnDate)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.ReturnDate = returnDate;
            loan.Status = "RETURNED";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExtendLoanAsync(Guid loanId, int additionalDays)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.DueDate = loan.DueDate.AddDays(additionalDays);
            // On pourrait ajouter un champ IsExtended = true pour suivre les prolongations

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserLoanStats> GetUserLoanStatsAsync(Guid userId)
        {
            var totalBorrowed = await _context.Loans
                .CountAsync(l => l.UserId == userId);

            var currentlyBorrowed = await _context.Loans
                .CountAsync(l => l.UserId == userId && l.Status == "BORROWED");

            var overdueCount = await _context.Loans
                .CountAsync(l => l.UserId == userId && l.DueDate < DateTime.UtcNow && l.Status == "BORROWED");

            return new UserLoanStats
            {
                TotalBorrowed = totalBorrowed,
                CurrentlyBorrowed = currentlyBorrowed,
                OverdueCount = overdueCount
            };
        }

        public async Task<int> GetTotalOverdueCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .CountAsync(l => l.DueDate < now && l.Status == "BORROWED");
        }
    }
}