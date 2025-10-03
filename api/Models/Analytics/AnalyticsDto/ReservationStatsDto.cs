namespace api.Models
{
    /// <summary>
    /// DTO pour les statistiques des réservations
    /// </summary>
    public class ReservationStatsDto
    {
        public int TotalReservations { get; set; }
        public int PendingReservations { get; set; }
        public int FulfilledReservations { get; set; }
        public int ExpiredReservations { get; set; }
        public double AverageFulfillmentTimeHours { get; set; }
        public string MostReservedBook { get; set; } = null!;
    }
}