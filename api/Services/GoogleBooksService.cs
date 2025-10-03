using System.Text.Json;
using api.Helpers;
using api.Models;
using api.Models.GoogleBooks;
using api.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /// <summary>
    /// Service pour l'interaction avec l'API Google Books
    /// </summary>
    public class GoogleBooksService : IGoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly IBookService _bookService;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GoogleBooksService> _logger;

        private const string GoogleBooksApiBaseUrl = "https://www.googleapis.com/books/v1/volumes";

        public GoogleBooksService(
            HttpClient httpClient,
            IBookService bookService,
            IBookRepository bookRepository,
            IMapper mapper,
            ILogger<GoogleBooksService> logger)
        {
            _httpClient = httpClient;
            _bookService = bookService;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<BookDto>> ImportBookFromGoogleAsync(ImportBookFromGoogleDto importDto)
        {
            try
            {
                _logger.LogInformation("Tentative d'importation du livre avec ISBN: {Isbn}", importDto.Isbn);

                // Vérifier si le livre existe déjà dans notre base
                if (await _bookService.IsbnExistsAsync(importDto.Isbn))
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = "Un livre avec cet ISBN existe déjà dans la base de données"
                    };
                }

                // Rechercher le livre dans Google Books API
                var googleBook = await GetBookFromGoogleBooksAsync(importDto.Isbn);
                if (googleBook == null)
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = "Livre non trouvé dans Google Books API"
                    };
                }

                // Convertir le livre Google en CreateBookDto
                var createBookDto = ConvertGoogleBookToCreateBookDto(googleBook, importDto);

                // Créer le livre dans notre base de données
                var createResult = await _bookService.CreateBookAsync(createBookDto);
                if (!createResult.Success)
                {
                    return new ServiceResponse<BookDto>
                    {
                        Success = false,
                        Message = createResult.Message,
                        Errors = createResult.Errors
                    };
                }

                _logger.LogInformation("Livre importé avec succès: {Title} (ID: {Id})", 
                    createResult.Data.Title, createResult.Data.Id);

                return new ServiceResponse<BookDto>
                {
                    Success = true,
                    Message = "Livre importé avec succès depuis Google Books",
                    Data = createResult.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'importation du livre avec ISBN: {Isbn}", importDto.Isbn);
                return new ServiceResponse<BookDto>
                {
                    Success = false,
                    Message = "Erreur lors de l'importation du livre",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<List<GoogleBookPreviewDto>>> SearchBooksAsync(string query, int maxResults = 10)
        {
            try
            {
                var url = $"{GoogleBooksApiBaseUrl}?q={Uri.EscapeDataString(query)}&maxResults={maxResults}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<List<GoogleBookPreviewDto>>
                    {
                        Success = false,
                        Message = "Erreur lors de la recherche dans Google Books API"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonSerializer.Deserialize<GoogleBookResponse>(content);

                if (googleResponse?.Items == null || !googleResponse.Items.Any())
                {
                    return new ServiceResponse<List<GoogleBookPreviewDto>>
                    {
                        Success = true,
                        Message = "Aucun livre trouvé",
                        Data = new List<GoogleBookPreviewDto>()
                    };
                }

                var previews = googleResponse.Items
                    .Select(item => ConvertToGoogleBookPreviewDto(item))
                    .Where(preview => preview != null)
                    .ToList();

                return new ServiceResponse<List<GoogleBookPreviewDto>>
                {
                    Success = true,
                    Data = previews!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche de livres avec la requête: {Query}", query);
                return new ServiceResponse<List<GoogleBookPreviewDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la recherche de livres",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private async Task<GoogleBookVolumeInfo?> GetBookFromGoogleBooksAsync(string isbn)
        {
            try
            {
                var url = $"{GoogleBooksApiBaseUrl}?q=isbn:{isbn}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonSerializer.Deserialize<GoogleBookResponse>(content);

                return googleResponse?.Items?.FirstOrDefault()?.VolumeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du livre depuis Google Books avec ISBN: {Isbn}", isbn);
                return null;
            }
        }

        private CreateBookDto ConvertGoogleBookToCreateBookDto(GoogleBookVolumeInfo googleBook, ImportBookFromGoogleDto importDto)
        {
            var isbn = ExtractIsbn(googleBook) ?? importDto.Isbn;
            var category = googleBook.Categories?.FirstOrDefault() ?? importDto.DefaultCategory;
            var coverUrl = googleBook.ImageLinks?.Thumbnail?.Replace("http://", "https://") ?? string.Empty;

            // Nettoyer l'URL de la couverture
            if (!string.IsNullOrEmpty(coverUrl) && coverUrl.Contains("&edge=curl"))
            {
                coverUrl = coverUrl.Replace("&edge=curl", "");
            }

            return new CreateBookDto
            {
                Title = googleBook.Title ?? "Titre inconnu",
                Author = googleBook.Authors?.FirstOrDefault() ?? "Auteur inconnu",
                Category = category,
                Isbn = isbn,
                PublicationDate = ParsePublicationDate(googleBook.PublishedDate),
                CoverUrl = coverUrl,
                TotalCopies = importDto.TotalCopies
            };
        }

        private GoogleBookPreviewDto? ConvertToGoogleBookPreviewDto(GoogleBookItem item)
        {
            try
            {
                var volumeInfo = item.VolumeInfo;
                var isbn = ExtractIsbn(volumeInfo);

                if (string.IsNullOrEmpty(isbn) || string.IsNullOrEmpty(volumeInfo.Title))
                    return null;

                var coverUrl = volumeInfo.ImageLinks?.Thumbnail?.Replace("http://", "https://") ?? string.Empty;
                if (!string.IsNullOrEmpty(coverUrl) && coverUrl.Contains("&edge=curl"))
                {
                    coverUrl = coverUrl.Replace("&edge=curl", "");
                }

                return new GoogleBookPreviewDto
                {
                    Title = volumeInfo.Title,
                    Authors = volumeInfo.Authors ?? new List<string>(),
                    PublishedDate = volumeInfo.PublishedDate ?? string.Empty,
                    Description = volumeInfo.Description ?? string.Empty,
                    Isbn = isbn,
                    CoverUrl = coverUrl,
                    Categories = volumeInfo.Categories ?? new List<string>(),
                    PageCount = volumeInfo.PageCount,
                    Publisher = volumeInfo.Publisher ?? string.Empty,
                    GoogleBooksId = item.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la conversion du livre Google en aperçu");
                return null;
            }
        }

        private string? ExtractIsbn(GoogleBookVolumeInfo volumeInfo)
        {
            return volumeInfo.IndustryIdentifiers?
                .FirstOrDefault(id => id.Type == "ISBN_13" || id.Type == "ISBN_10")?
                .Identifier;
        }

        private DateTime? ParsePublicationDate(string? publishedDate)
        {
            if (string.IsNullOrEmpty(publishedDate))
                return null;

            // Google Books peut retourner des dates au format "YYYY", "YYYY-MM", ou "YYYY-MM-DD"
            if (DateTime.TryParse(publishedDate, out var date))
                return date;

            if (int.TryParse(publishedDate, out var year) && year >= 1000 && year <= 9999)
                return new DateTime(year, 1, 1);

            return null;
        }
    }
}