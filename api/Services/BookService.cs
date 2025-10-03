using api.Helpers;
using api.Models;
using api.Repositories;
using AutoMapper;

namespace api.Services
{
    /// <summary>
    /// Service pour la gestion des livres
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedResult<BookDto>>> GetAllBooksAsync(string? category = null, string? author = null, bool? availableOnly = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var booksResult = await _bookRepository.GetAllAsync(category, author, availableOnly, page, pageSize);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(booksResult.Items);
                
                var pagedResult = new PagedResult<BookDto>(
                    bookDtos, 
                    booksResult.TotalCount, 
                    booksResult.CurrentPage, 
                    booksResult.PageSize
                );

                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des livres",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookDto>> GetBookByIdAsync(Guid id)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(id);
                if (book == null)
                {
                    return new ServiceResponse<BookDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                var bookDto = _mapper.Map<BookDto>(book);
                return new ServiceResponse<BookDto> 
                { 
                    Success = true, 
                    Data = bookDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération du livre",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<BookDto>>> SearchBooksAsync(string query, string? searchIn = "title,author", int page = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return new ServiceResponse<PagedResult<BookDto>> 
                    { 
                        Success = false, 
                        Message = "Le terme de recherche est requis" 
                    };
                }

                var booksResult = await _bookRepository.SearchAsync(query, searchIn, page, pageSize);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(booksResult.Items);
                
                var pagedResult = new PagedResult<BookDto>(
                    bookDtos, 
                    booksResult.TotalCount, 
                    booksResult.CurrentPage, 
                    booksResult.PageSize
                );

                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la recherche des livres",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<BookDto>>> GetBooksByCategoryAsync(Guid categoryId, bool? availableOnly = null, string? sortBy = "title", string? sortOrder = "asc", int page = 1, int pageSize = 20)
        {
            try
            {
                var booksResult = await _bookRepository.GetByCategoryAsync(categoryId, availableOnly, sortBy, sortOrder, page, pageSize);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(booksResult.Items);
                
                var pagedResult = new PagedResult<BookDto>(
                    bookDtos, 
                    booksResult.TotalCount, 
                    booksResult.CurrentPage, 
                    booksResult.PageSize
                );

                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = true, 
                    Data = pagedResult 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<BookDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des livres par catégorie",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<BookDto>>> GetRecentBooksAsync(int days = 30, int limit = 10)
        {
            try
            {
                var books = await _bookRepository.GetRecentAsync(days, limit);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                
                return new ServiceResponse<IEnumerable<BookDto>> 
                { 
                    Success = true, 
                    Data = bookDtos 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<BookDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des livres récents",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<BookDto>>> GetPopularBooksAsync(string period = "MONTH", int limit = 10)
        {
            try
            {
                var books = await _bookRepository.GetPopularAsync(period, limit);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                
                return new ServiceResponse<IEnumerable<BookDto>> 
                { 
                    Success = true, 
                    Data = bookDtos 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<BookDto>> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des livres populaires",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookDto>> CreateBookAsync(CreateBookDto createBookDto)
        {
            try
            {
                // Vérifier si l'ISBN existe déjà
                if (await _bookRepository.IsbnExistsAsync(createBookDto.Isbn))
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = "Un livre avec cet ISBN existe déjà"
                    };
                }

                // Rechercher ou créer la catégorie
                var category = await _bookRepository.GetCategoryByNameAsync(createBookDto.Category);
                if (category == null)
                {
                    // Créer la catégorie si elle n'existe pas
                    var newCategory = new Category
                    {
                        Name = createBookDto.Category,
                        Description = $"Catégorie pour {createBookDto.Category}",
                        Icon = "📚"
                    };
                    category = await _bookRepository.CreateCategoryAsync(newCategory);
                }

                // Créer le livre manuellement pour éviter les problèmes de mapping AutoMapper
                var book = new Book
                {
                    Title = createBookDto.Title,
                    Author = createBookDto.Author,
                    Isbn = createBookDto.Isbn.Replace("-", "").Replace(" ", ""), // Normalisation ISBN
                    PublicationDate = createBookDto.PublicationDate,
                    CoverUrl = string.IsNullOrEmpty(createBookDto.CoverUrl) ? null : createBookDto.CoverUrl,
                    TotalCopies = createBookDto.TotalCopies,
                    AvailableCopies = createBookDto.TotalCopies,
                    CategoryId = category.Id,
                    Category = category,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                var createdBook = await _bookRepository.AddAsync(book);
                var bookDto = _mapper.Map<BookDto>(createdBook);
                
                return new ServiceResponse<BookDto>
                {
                    Success = true,
                    Message = "Livre créé avec succès",
                    Data = bookDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookDto>
                {
                    Success = false,
                    Message = "Erreur lors de la création du livre",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookDto>> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
        {
            try
            {
                var existingBook = await _bookRepository.GetByIdAsync(id);
                if (existingBook == null)
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = "Livre non trouvé"
                    };
                }

                // Vérifier si l'ISBN existe déjà (exclure le livre actuel)
                if (await _bookRepository.IsbnExistsAsync(updateBookDto.Isbn, id))
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = "Un autre livre avec cet ISBN existe déjà"
                    };
                }

                // Rechercher ou créer la catégorie si elle a changé
                if (existingBook.Category.Name != updateBookDto.Category)
                {
                    var category = await _bookRepository.GetCategoryByNameAsync(updateBookDto.Category);
                    if (category == null)
                    {
                        // Créer la catégorie si elle n'existe pas
                        var newCategory = new Category
                        {
                            Name = updateBookDto.Category,
                            Description = $"Catégorie pour {updateBookDto.Category}",
                            Icon = "📚"
                        };
                        category = await _bookRepository.CreateCategoryAsync(newCategory);
                    }
                    existingBook.CategoryId = category.Id;
                    existingBook.Category = category;
                }

                // Mapper les autres propriétés
                existingBook.Title = updateBookDto.Title;
                existingBook.Author = updateBookDto.Author;
                existingBook.Isbn = updateBookDto.Isbn;
                existingBook.PublicationDate = updateBookDto.PublicationDate;
                existingBook.CoverUrl = updateBookDto.CoverUrl;
                existingBook.UpdatedAt = DateTime.UtcNow;
                
                var updatedBook = await _bookRepository.UpdateAsync(existingBook);
                var bookDto = _mapper.Map<BookDto>(updatedBook);
                
                return new ServiceResponse<BookDto>
                {
                    Success = true,
                    Message = "Livre mis à jour avec succès",
                    Data = bookDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour du livre",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteBookAsync(Guid id)
        {
            try
            {
                // Vérifier s'il y a des emprunts ou réservations actifs
                if (await _bookRepository.HasActiveLoansOrReservationsAsync(id))
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Impossible de supprimer le livre : il y a des emprunts ou réservations actifs" 
                    };
                }

                var result = await _bookRepository.DeleteAsync(id);
                if (!result)
                {
                    return new ServiceResponse<bool> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                return new ServiceResponse<bool> 
                { 
                    Success = true, 
                    Message = "Livre supprimé avec succès",
                    Data = true 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la suppression du livre",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookDto>> AddCopiesAsync(Guid bookId, int quantity, string reason)
        {
            try
            {
                var result = await _bookRepository.AddCopiesAsync(bookId, quantity);
                if (!result)
                {
                    return new ServiceResponse<BookDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                var updatedBook = await _bookRepository.GetByIdAsync(bookId);
                var bookDto = _mapper.Map<BookDto>(updatedBook);
                
                return new ServiceResponse<BookDto> 
                { 
                    Success = true, 
                    Message = $"{quantity} exemplaire(s) ajouté(s) avec succès",
                    Data = bookDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de l'ajout d'exemplaires",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookDto>> UpdateAvailabilityAsync(Guid bookId, bool available, string? reason = null)
        {
            try
            {
                var result = await _bookRepository.UpdateAvailabilityAsync(bookId, available, reason);
                if (!result)
                {
                    return new ServiceResponse<BookDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                var updatedBook = await _bookRepository.GetByIdAsync(bookId);
                var bookDto = _mapper.Map<BookDto>(updatedBook);
                
                return new ServiceResponse<BookDto> 
                { 
                    Success = true, 
                    Message = $"Disponibilité mise à jour : {(available ? "Disponible" : "Indisponible")}",
                    Data = bookDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la mise à jour de la disponibilité",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BookStatsDto>> GetBookStatsAsync(Guid bookId)
        {
            try
            {
                var stats = await _bookRepository.GetStatsAsync(bookId);
                if (stats == null)
                {
                    return new ServiceResponse<BookStatsDto> 
                    { 
                        Success = false, 
                        Message = "Livre non trouvé" 
                    };
                }

                return new ServiceResponse<BookStatsDto> 
                { 
                    Success = true, 
                    Data = _mapper.Map<BookStatsDto>(stats)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BookStatsDto> 
                { 
                    Success = false, 
                    Message = "Erreur lors de la récupération des statistiques",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null)
        {
            return await _bookRepository.IsbnExistsAsync(isbn, excludeBookId);
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _bookRepository.GetAllCategoriesAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                
                return new ServiceResponse<IEnumerable<CategoryDto>>
                {
                    Success = true,
                    Data = categoryDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<CategoryDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des catégories",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var category = await _bookRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return new ServiceResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Catégorie non trouvée"
                    };
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return new ServiceResponse<CategoryDto>
                {
                    Success = true,
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération de la catégorie",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                // Vérifier si une catégorie avec le même nom existe déjà
                var existingCategory = await _bookRepository.GetCategoryByNameAsync(createCategoryDto.Name);
                if (existingCategory != null)
                {
                    return new ServiceResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Une catégorie avec ce nom existe déjà"
                    };
                }

                var category = _mapper.Map<Category>(createCategoryDto);
                var createdCategory = await _bookRepository.CreateCategoryAsync(category);
                var categoryDto = _mapper.Map<CategoryDto>(createdCategory);
                
                return new ServiceResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Catégorie créée avec succès",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la création de la catégorie",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var existingCategory = await _bookRepository.GetCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    return new ServiceResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Catégorie non trouvée"
                    };
                }

                // Vérifier si une autre catégorie avec le même nom existe déjà
                var existingCategoryWithSameName = await _bookRepository.GetCategoryByNameAsync(updateCategoryDto.Name);
                if (existingCategoryWithSameName != null && existingCategoryWithSameName.Id != id)
                {
                    return new ServiceResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Une autre catégorie avec ce nom existe déjà"
                    };
                }

                _mapper.Map(updateCategoryDto, existingCategory);
                var updatedCategory = await _bookRepository.UpdateCategoryAsync(existingCategory);
                var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);
                
                return new ServiceResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Catégorie mise à jour avec succès",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de la catégorie",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            try
            {
                // Vérifier si la catégorie a des livres associés
                if (await _bookRepository.CategoryHasBooksAsync(id))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Impossible de supprimer la catégorie : elle contient des livres"
                    };
                }

                var result = await _bookRepository.DeleteCategoryAsync(id);
                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Catégorie non trouvée"
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Catégorie supprimée avec succès",
                    Data = true
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression de la catégorie",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}