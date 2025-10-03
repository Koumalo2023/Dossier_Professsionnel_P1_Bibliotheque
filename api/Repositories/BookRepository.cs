using api.Data;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des livres
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Book>> GetAllAsync(string? category = null, string? author = null, bool? availableOnly = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Books
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category.Name.Contains(category));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (availableOnly.HasValue && availableOnly.Value)
            {
                query = query.Where(b => b.AvailableCopies > 0);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Book>(items, totalCount, page, pageSize);
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByIsbnAsync(string isbn)
        {
            return await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Isbn == isbn);
        }

        public async Task<PagedResult<Book>> SearchAsync(string query, string? searchIn = "title,author,description", int page = 1, int pageSize = 20)
        {
            var searchTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var booksQuery = _context.Books
                .Include(b => b.Category)
                .AsQueryable();

            var searchFields = searchIn?.Split(',') ?? new[] { "title", "author" };

            foreach (var term in searchTerms)
            {
                booksQuery = booksQuery.Where(b =>
                    (searchFields.Contains("title") && b.Title.ToLower().Contains(term)) ||
                    (searchFields.Contains("author") && b.Author.ToLower().Contains(term)));
            }

            var totalCount = await booksQuery.CountAsync();
            var items = await booksQuery
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Book>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Book>> GetByCategoryAsync(Guid categoryId, bool? availableOnly = null, string? sortBy = "title", string? sortOrder = "asc", int page = 1, int pageSize = 20)
        {
            var query = _context.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId);

            if (availableOnly.HasValue && availableOnly.Value)
            {
                query = query.Where(b => b.AvailableCopies > 0);
            }

            query = sortBy?.ToLower() switch
            {
                "author" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
                "publicationdate" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(b => b.PublicationDate) : query.OrderBy(b => b.PublicationDate),
                _ => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Book>(items, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<Book>> GetRecentAsync(int days = 30, int limit = 10)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.CreatedAt >= cutoffDate)
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetPopularAsync(string period = "MONTH", int limit = 10)
        {
            var cutoffDate = period.ToUpper() switch
            {
                "WEEK" => DateTime.UtcNow.AddDays(-7),
                "MONTH" => DateTime.UtcNow.AddMonths(-1),
                "YEAR" => DateTime.UtcNow.AddYears(-1),
                _ => DateTime.UtcNow.AddMonths(-1)
            };

            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Loans)
                .Where(b => b.Loans.Any(l => l.LoanDate >= cutoffDate))
                .OrderByDescending(b => b.Loans.Count(l => l.LoanDate >= cutoffDate))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            book.UpdatedAt = DateTime.UtcNow;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActiveLoansOrReservationsAsync(Guid bookId)
        {
            var hasActiveLoans = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.Status == "BORROWED");

            var hasActiveReservations = await _context.Reservations
                .AnyAsync(r => r.BookId == bookId && r.Status == "PENDING");

            return hasActiveLoans || hasActiveReservations;
        }

        public async Task<bool> AddCopiesAsync(Guid bookId, int quantity)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            book.TotalCopies += quantity;
            book.AvailableCopies += quantity;
            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(Guid bookId, bool available, string? reason = null)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            // Pour simplifier, on met availableCopies à 0 si non disponible
            book.AvailableCopies = available ? book.TotalCopies : 0;
            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookStatsDto> GetStatsAsync(Guid bookId)
        {
            var book = await _context.Books
                .Include(b => b.Loans)
                .Include(b => b.Reservations)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return null;

            var totalBorrows = book.Loans.Count;
            var currentActiveLoans = book.Loans.Count(l => l.Status == "BORROWED");
            var totalReservations = book.Reservations.Count;

            var completedLoans = book.Loans.Where(l => l.ReturnDate.HasValue).ToList();
            var averageLoanDuration = completedLoans.Any() 
                ? completedLoans.Average(l => (l.ReturnDate.Value - l.LoanDate).TotalDays)
                : 0;

            var lastBorrowed = book.Loans.OrderByDescending(l => l.LoanDate).FirstOrDefault()?.LoanDate;

            return new BookStatsDto
            {
                BookId = bookId,
                Title = book.Title,
                TotalBorrows = totalBorrows,
                CurrentActiveLoans = currentActiveLoans,
                TotalReservations = totalReservations,
                AverageLoanDurationDays = Math.Round(averageLoanDuration, 1),
                PopularityRank = 0, // À calculer en fonction de tous les livres
                LastBorrowed = lastBorrowed
            };
        }

        public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null)
        {
            var query = _context.Books.Where(b => b.Isbn == isbn);
            
            if (excludeBookId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Books)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Vérifier si la catégorie a des livres associés
            if (await CategoryHasBooksAsync(id))
            {
                throw new InvalidOperationException("Impossible de supprimer la catégorie : elle contient des livres");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryHasBooksAsync(Guid categoryId)
        {
            return await _context.Books.AnyAsync(b => b.CategoryId == categoryId);
        }
    }
}