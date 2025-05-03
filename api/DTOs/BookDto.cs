namespace api.DTOs
{
    /// <summary>
    /// DTO pour transférer les données d'un livre.
    /// </summary>
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Isbn { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string CoverUrl { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
