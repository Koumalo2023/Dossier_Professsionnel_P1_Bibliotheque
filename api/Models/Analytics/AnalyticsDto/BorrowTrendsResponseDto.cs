using System.Collections.Generic;

namespace api.Models
{
    /// <summary>
    /// DTO pour la réponse des tendances d'emprunts
    /// </summary>
    public class BorrowTrendsResponseDto
    {
        public string Period { get; set; } = null!;
        public List<BorrowTrendDto> Data { get; set; } = new List<BorrowTrendDto>();
        public double AverageGrowth { get; set; }
    }
}