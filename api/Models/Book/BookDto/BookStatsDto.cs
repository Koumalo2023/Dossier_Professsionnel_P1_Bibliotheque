
namespace api.Models
{ 

    /// <summary>
    /// DTO pour les statistiques d'un livre
    /// </summary>
    public class BookStatsDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public int TotalBorrows { get; set; }
        public int CurrentActiveLoans { get; set; }
        public int TotalReservations { get; set; }
        public double AverageLoanDurationDays { get; set; }
        public int PopularityRank { get; set; }
        public DateTime? LastBorrowed { get; set; }
    }
}