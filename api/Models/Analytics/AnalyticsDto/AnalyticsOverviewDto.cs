namespace api.Models
{
    /// <summary>
    /// DTO pour la vue d'ensemble des statistiques
    /// </summary>
    public class AnalyticsOverviewDto
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int PendingReservations { get; set; }
        public int ActiveUsersLast30Days { get; set; }
    }
}