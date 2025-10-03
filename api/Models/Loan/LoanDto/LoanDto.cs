namespace api.Models
{
    /// <summary>
    /// DTO pour transférer les données d'un emprunt.
    /// </summary>
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
    }
}
