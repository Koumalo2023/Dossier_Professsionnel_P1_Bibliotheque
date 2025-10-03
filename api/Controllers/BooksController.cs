using api.Helpers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IGoogleBooksService _googleBooksService;

        public BooksController(IBookService bookService, IGoogleBooksService googleBooksService)
        {
            _bookService = bookService;
            _googleBooksService = googleBooksService;
        }

        /// <summary>
        /// Récupère tous les livres avec pagination et filtres
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBooks(
            [FromQuery] string? category = null,
            [FromQuery] string? author = null,
            [FromQuery] bool? availableOnly = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _bookService.GetAllBooksAsync(category, author, availableOnly, page, pageSize);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                items = result.Data.Items,
                totalCount = result.Data.TotalCount,
                currentPage = result.Data.CurrentPage,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages
            });
        }

        /// <summary>
        /// Récupère un livre par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Recherche avancée de livres
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchBooks(
            [FromQuery] string q,
            [FromQuery] string? searchIn = "title,author",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _bookService.SearchBooksAsync(q, searchIn, page, pageSize);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                items = result.Data.Items,
                totalCount = result.Data.TotalCount,
                searchTerm = q,
                currentPage = result.Data.CurrentPage,
                pageSize = result.Data.PageSize
            });
        }

        /// <summary>
        /// Récupère les livres par catégorie
        /// </summary>
        [HttpGet("categories/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooksByCategory(
            Guid categoryId,
            [FromQuery] bool? availableOnly = null,
            [FromQuery] string? sortBy = "title",
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _bookService.GetBooksByCategoryAsync(categoryId, availableOnly, sortBy, sortOrder, page, pageSize);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                items = result.Data.Items,
                totalCount = result.Data.TotalCount,
                currentPage = result.Data.CurrentPage,
                pageSize = result.Data.PageSize
            });
        }

        /// <summary>
        /// Récupère les livres récemment ajoutés
        /// </summary>
        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentBooks(
            [FromQuery] int days = 30,
            [FromQuery] int limit = 10)
        {
            var result = await _bookService.GetRecentBooksAsync(days, limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les livres les plus populaires
        /// </summary>
        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularBooks(
            [FromQuery] string period = "MONTH",
            [FromQuery] int limit = 10)
        {
            var result = await _bookService.GetPopularBooksAsync(period, limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                items = result.Data,
                period = period,
                totalBooks = result.Data.Count()
            });
        }

        /// <summary>
        /// Crée un nouveau livre
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookService.CreateBookAsync(createBookDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(nameof(GetBookById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Met à jour un livre existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookService.UpdateBookAsync(id, updateBookDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Supprime un livre
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Ajoute des exemplaires à un livre
        /// </summary>
        [HttpPost("{id}/copies")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> AddCopies(Guid id, [FromBody] AddCopiesRequest request)
        {
            var result = await _bookService.AddCopiesAsync(id, request.Quantity, request.Reason);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                message = result.Message,
                book = result.Data
            });
        }

        /// <summary>
        /// Met à jour la disponibilité d'un livre
        /// </summary>
        [HttpPut("{id}/availability")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateAvailabilityRequest request)
        {
            var result = await _bookService.UpdateAvailabilityAsync(id, request.Available, request.Reason);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                message = result.Message,
                book = result.Data
            });
        }

        /// <summary>
        /// Récupère les statistiques d'un livre
        /// </summary>
        [HttpGet("{id}/stats")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetBookStats(Guid id)
        {
            var result = await _bookService.GetBookStatsAsync(id);
            
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère toutes les catégories
        /// </summary>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _bookService.GetAllCategoriesAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère une catégorie par son ID
        /// </summary>
        [HttpGet("categories/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _bookService.GetCategoryByIdAsync(id);
            
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Crée une nouvelle catégorie
        /// </summary>
        [HttpPost("categories")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookService.CreateCategoryAsync(createCategoryDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Met à jour une catégorie existante
        /// </summary>
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookService.UpdateCategoryAsync(id, updateCategoryDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Supprime une catégorie (si non utilisée)
        /// </summary>
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _bookService.DeleteCategoryAsync(id);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    
        /// <summary>
        /// Importe un livre depuis Google Books API par son ISBN
        /// </summary>
        [HttpPost("import/google")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ImportBookFromGoogle([FromBody] ImportBookFromGoogleDto importDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
    
            var result = await _googleBooksService.ImportBookFromGoogleAsync(importDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });
    
            return CreatedAtAction(nameof(GetBookById), new { id = result.Data.Id }, result.Data);
        }
    
        /// <summary>
        /// Recherche des livres dans Google Books API
        /// </summary>
        [HttpGet("import/google/search")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> SearchGoogleBooks(
            [FromQuery] string q,
            [FromQuery] int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Le terme de recherche est requis" });
    
            var result = await _googleBooksService.SearchBooksAsync(q, maxResults);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });
    
            return Ok(new
            {
                items = result.Data,
                searchTerm = q,
                totalResults = result.Data.Count
            });
        }
    }
    
    /// <summary>
    /// Requête pour ajouter des exemplaires
    /// </summary>
    public class AddCopiesRequest
    {
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Requête pour mettre à jour la disponibilité
    /// </summary>
    public class UpdateAvailabilityRequest
    {
        public bool Available { get; set; }
        public string? Reason { get; set; }
    }
}