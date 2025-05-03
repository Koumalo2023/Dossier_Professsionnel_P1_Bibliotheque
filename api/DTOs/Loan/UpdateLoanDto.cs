namespace api.DTOs
{
    /// <summary>
    /// DTO pour la mise à jour d'un emprunt.
    /// </summary>
    public class UpdateLoanDto
    {
        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; }
    }
}
