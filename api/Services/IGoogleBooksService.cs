using api.Helpers;
using api.Models;

namespace api.Services
{
    /// <summary>
    /// Service pour l'interaction avec l'API Google Books
    /// </summary>
    public interface IGoogleBooksService
    {
        /// <summary>
        /// Importe un livre depuis Google Books API par son ISBN
        /// </summary>
        /// <param name="importDto">DTO d'importation contenant l'ISBN et la catégorie par défaut</param>
        /// <returns>Résultat de l'importation avec le livre créé</returns>
        Task<ServiceResponse<BookDto>> ImportBookFromGoogleAsync(ImportBookFromGoogleDto importDto);

        /// <summary>
        /// Recherche des livres dans Google Books API
        /// </summary>
        /// <param name="query">Terme de recherche</param>
        /// <param name="maxResults">Nombre maximum de résultats (défaut: 10)</param>
        /// <returns>Liste des livres trouvés</returns>
        Task<ServiceResponse<List<GoogleBookPreviewDto>>> SearchBooksAsync(string query, int maxResults = 10);
    }

    /// <summary>
    /// DTO pour l'aperçu d'un livre Google Books
    /// </summary>
    public class GoogleBookPreviewDto
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new();
        public string PublishedDate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public int PageCount { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string GoogleBooksId { get; set; } = string.Empty;
    }
}