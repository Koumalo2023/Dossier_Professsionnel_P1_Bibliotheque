using System.Text.Json.Serialization;

namespace api.Models.GoogleBooks
{
    /// <summary>
    /// Réponse de l'API Google Books pour une recherche par ISBN
    /// </summary>
    public class GoogleBookResponse
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleBookItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Item de livre dans la réponse Google Books
    /// </summary>
    public class GoogleBookItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("volumeInfo")]
        public GoogleBookVolumeInfo VolumeInfo { get; set; } = new();
    }

    /// <summary>
    /// Informations sur le volume (livre)
    /// </summary>
    public class GoogleBookVolumeInfo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; } = new();

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; } = string.Empty;

        [JsonPropertyName("publishedDate")]
        public string PublishedDate { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("industryIdentifiers")]
        public List<GoogleBookIndustryIdentifier> IndustryIdentifiers { get; set; } = new();

        [JsonPropertyName("pageCount")]
        public int PageCount { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; } = new();

        [JsonPropertyName("imageLinks")]
        public GoogleBookImageLinks ImageLinks { get; set; } = new();

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
    }

    /// <summary>
    /// Identifiant industriel (ISBN, etc.)
    /// </summary>
    public class GoogleBookIndustryIdentifier
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; } = string.Empty;
    }

    /// <summary>
    /// Liens d'images pour la couverture
    /// </summary>
    public class GoogleBookImageLinks
    {
        [JsonPropertyName("smallThumbnail")]
        public string SmallThumbnail { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [JsonPropertyName("small")]
        public string Small { get; set; } = string.Empty;

        [JsonPropertyName("medium")]
        public string Medium { get; set; } = string.Empty;

        [JsonPropertyName("large")]
        public string Large { get; set; } = string.Empty;

        [JsonPropertyName("extraLarge")]
        public string ExtraLarge { get; set; } = string.Empty;
    }
}