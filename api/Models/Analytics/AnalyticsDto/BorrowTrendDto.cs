namespace api.Models
{
    /// <summary>
    /// DTO pour les tendances d'emprunts
    /// </summary>
    public class BorrowTrendDto
    {
        public string Period { get; set; } = null!;
        public int BorrowCount { get; set; }
        public double Growth { get; set; }
    }
}