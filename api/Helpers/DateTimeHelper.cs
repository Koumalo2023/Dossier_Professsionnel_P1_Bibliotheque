using System;
using System.Globalization;

namespace api.Helpers
{
    /// <summary>
    /// Helper pour gérer les opérations liées aux dates et heures.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Culture française pour le formatage des dates.
        /// </summary>
        public static readonly CultureInfo FrenchCulture = new CultureInfo("fr-FR");

        /// <summary>
        /// Calcule la date d'échéance d'un emprunt.
        /// </summary>
        /// <param name="loanDate">Date de l'emprunt</param>
        /// <param name="loanDurationInDays">Durée du prêt en jours</param>
        /// <returns>Date d'échéance calculée</returns>
        public static DateTime CalculateDueDate(DateTime loanDate, int loanDurationInDays = 30)
        {
            return loanDate.AddDays(loanDurationInDays);
        }

        /// <summary>
        /// Calcule le nombre de jours de retard pour un emprunt.
        /// </summary>
        /// <param name="dueDate">Date d'échéance</param>
        /// <param name="returnDate">Date de retour (optionnelle, utilise la date actuelle si null)</param>
        /// <returns>Nombre de jours de retard (0 si pas de retard)</returns>
        public static int CalculateDaysOverdue(DateTime dueDate, DateTime? returnDate = null)
        {
            var comparisonDate = returnDate ?? DateTime.UtcNow;
            if (comparisonDate <= dueDate)
                return 0;

            return (int)(comparisonDate - dueDate).TotalDays;
        }

        /// <summary>
        /// Vérifie si un emprunt est en retard.
        /// </summary>
        /// <param name="dueDate">Date d'échéance</param>
        /// <param name="returnDate">Date de retour (optionnelle)</param>
        /// <returns>True si l'emprunt est en retard</returns>
        public static bool IsOverdue(DateTime dueDate, DateTime? returnDate = null)
        {
            var comparisonDate = returnDate ?? DateTime.UtcNow;
            return comparisonDate > dueDate;
        }

        /// <summary>
        /// Formate une date selon le format français court (dd/MM/yyyy).
        /// </summary>
        /// <param name="date">Date à formater</param>
        /// <returns>Date formatée</returns>
        public static string FormatShortDate(DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString("dd/MM/yyyy", FrenchCulture);
        }

        /// <summary>
        /// Formate une date selon le format français long (dd MMMM yyyy).
        /// </summary>
        /// <param name="date">Date à formater</param>
        /// <returns>Date formatée</returns>
        public static string FormatLongDate(DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString("dd MMMM yyyy", FrenchCulture);
        }

        /// <summary>
        /// Formate une date et heure selon le format français (dd/MM/yyyy HH:mm).
        /// </summary>
        /// <param name="date">Date et heure à formater</param>
        /// <returns>Date et heure formatées</returns>
        public static string FormatDateTime(DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString("dd/MM/yyyy HH:mm", FrenchCulture);
        }

        /// <summary>
        /// Formate une durée en jours, heures, minutes.
        /// </summary>
        /// <param name="duration">Durée en TimeSpan</param>
        /// <returns>Durée formatée</returns>
        public static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
                return $"{(int)duration.TotalDays} jour{(duration.TotalDays >= 2 ? "s" : "")}";

            if (duration.TotalHours >= 1)
                return $"{(int)duration.TotalHours} heure{(duration.TotalHours >= 2 ? "s" : "")}";

            if (duration.TotalMinutes >= 1)
                return $"{(int)duration.TotalMinutes} minute{(duration.TotalMinutes >= 2 ? "s" : "")}";

            return "moins d'une minute";
        }

        /// <summary>
        /// Formate une date relative (ex: "il y a 2 jours", "demain", etc.).
        /// </summary>
        /// <param name="date">Date à formater</param>
        /// <returns>Date relative formatée</returns>
        public static string FormatRelativeDate(DateTime date)
        {
            var now = DateTime.UtcNow;
            var difference = date - now;

            if (Math.Abs(difference.TotalDays) < 1)
            {
                if (difference.TotalHours >= 0)
                {
                    if (difference.TotalHours < 1)
                        return "dans moins d'une heure";
                    return $"dans {(int)difference.TotalHours} heure{(difference.TotalHours >= 2 ? "s" : "")}";
                }
                else
                {
                    if (Math.Abs(difference.TotalHours) < 1)
                        return "il y a moins d'une heure";
                    return $"il y a {(int)Math.Abs(difference.TotalHours)} heure{(Math.Abs(difference.TotalHours) >= 2 ? "s" : "")}";
                }
            }

            if (difference.TotalDays >= 0)
            {
                if (difference.TotalDays < 1)
                    return "demain";
                if (difference.TotalDays < 2)
                    return "après-demain";
                return $"dans {(int)difference.TotalDays} jour{(difference.TotalDays >= 2 ? "s" : "")}";
            }
            else
            {
                if (Math.Abs(difference.TotalDays) < 1)
                    return "hier";
                if (Math.Abs(difference.TotalDays) < 2)
                    return "avant-hier";
                return $"il y a {(int)Math.Abs(difference.TotalDays)} jour{(Math.Abs(difference.TotalDays) >= 2 ? "s" : "")}";
            }
        }

        /// <summary>
        /// Calcule la date d'expiration d'une réservation.
        /// </summary>
        /// <param name="reservationDate">Date de la réservation</param>
        /// <param name="expirationDays">Nombre de jours avant expiration</param>
        /// <returns>Date d'expiration</returns>
        public static DateTime CalculateReservationExpiry(DateTime reservationDate, int expirationDays = 7)
        {
            return reservationDate.AddDays(expirationDays);
        }

        /// <summary>
        /// Vérifie si une réservation a expiré.
        /// </summary>
        /// <param name="expiryDate">Date d'expiration</param>
        /// <returns>True si la réservation a expiré</returns>
        public static bool IsReservationExpired(DateTime expiryDate)
        {
            return DateTime.UtcNow > expiryDate;
        }

        /// <summary>
        /// Calcule l'âge à partir d'une date de naissance.
        /// </summary>
        /// <param name="birthDate">Date de naissance</param>
        /// <returns>Âge calculé</returns>
        public static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Vérifie si une date est dans le futur.
        /// </summary>
        /// <param name="date">Date à vérifier</param>
        /// <returns>True si la date est dans le futur</returns>
        public static bool IsFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }

        /// <summary>
        /// Vérifie si une date est dans le passé.
        /// </summary>
        /// <param name="date">Date à vérifier</param>
        /// <returns>True si la date est dans le passé</returns>
        public static bool IsPastDate(DateTime date)
        {
            return date < DateTime.UtcNow;
        }

        /// <summary>
        /// Arrondit une date à l'heure la plus proche.
        /// </summary>
        /// <param name="date">Date à arrondir</param>
        /// <param name="roundToMinutes">Intervalle d'arrondi en minutes</param>
        /// <returns>Date arrondie</returns>
        public static DateTime RoundToNearest(DateTime date, int roundToMinutes = 15)
        {
            var ticks = date.Ticks;
            var roundToTicks = TimeSpan.FromMinutes(roundToMinutes).Ticks;
            var roundedTicks = Math.Round((double)ticks / roundToTicks) * roundToTicks;
            return new DateTime((long)roundedTicks, date.Kind);
        }

        /// <summary>
        /// Obtient le premier jour du mois pour une date donnée.
        /// </summary>
        /// <param name="date">Date de référence</param>
        /// <returns>Premier jour du mois</returns>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Obtient le dernier jour du mois pour une date donnée.
        /// </summary>
        /// <param name="date">Date de référence</param>
        /// <returns>Dernier jour du mois</returns>
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        /// <summary>
        /// Calcule la différence entre deux dates en jours ouvrables (exclut les week-ends).
        /// </summary>
        /// <param name="startDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Nombre de jours ouvrables</returns>
        public static int CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            int businessDays = 0;
            DateTime currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return businessDays;
        }

        /// <summary>
        /// Vérifie si une date est un jour ouvrable (du lundi au vendredi).
        /// </summary>
        /// <param name="date">Date à vérifier</param>
        /// <returns>True si c'est un jour ouvrable</returns>
        public static bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        /// <summary>
        /// Convertit un timestamp Unix en DateTime.
        /// </summary>
        /// <param name="unixTimeStamp">Timestamp Unix</param>
        /// <returns>DateTime correspondant</returns>
        public static DateTime FromUnixTimeStamp(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// Convertit une DateTime en timestamp Unix.
        /// </summary>
        /// <param name="date">Date à convertir</param>
        /// <returns>Timestamp Unix</returns>
        public static long ToUnixTimeStamp(DateTime date)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)(date.ToUniversalTime() - unixStart).TotalSeconds;
        }

        /// <summary>
        /// Obtient le nom du mois en français.
        /// </summary>
        /// <param name="month">Numéro du mois (1-12)</param>
        /// <returns>Nom du mois</returns>
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Le mois doit être entre 1 et 12");

            return FrenchCulture.DateTimeFormat.GetMonthName(month);
        }

        /// <summary>
        /// Obtient le nom abrégé du mois en français.
        /// </summary>
        /// <param name="month">Numéro du mois (1-12)</param>
        /// <returns>Nom abrégé du mois</returns>
        public static string GetAbbreviatedMonthName(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Le mois doit être entre 1 et 12");

            return FrenchCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
        }

        /// <summary>
        /// Obtient le nom du jour de la semaine en français.
        /// </summary>
        /// <param name="dayOfWeek">Jour de la semaine</param>
        /// <returns>Nom du jour</returns>
        public static string GetDayName(DayOfWeek dayOfWeek)
        {
            return FrenchCulture.DateTimeFormat.GetDayName(dayOfWeek);
        }

        /// <summary>
        /// Obtient le nom abrégé du jour de la semaine en français.
        /// </summary>
        /// <param name="dayOfWeek">Jour de la semaine</param>
        /// <returns>Nom abrégé du jour</returns>
        public static string GetAbbreviatedDayName(DayOfWeek dayOfWeek)
        {
            return FrenchCulture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
        }
    }
}