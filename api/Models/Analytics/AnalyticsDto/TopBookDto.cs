using System;

namespace api.Models
{
    /// <summary>
    /// DTO pour les livres les plus empruntés
    /// </summary>
    public class TopBookDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public int BorrowCount { get; set; }
    }
}