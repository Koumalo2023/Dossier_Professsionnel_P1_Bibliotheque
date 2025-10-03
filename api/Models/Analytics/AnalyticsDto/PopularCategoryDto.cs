using System;

namespace api.Models
{
    /// <summary>
    /// DTO pour les catégories populaires
    /// </summary>
    public class PopularCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int BorrowCount { get; set; }
        public double Percentage { get; set; }
    }
}