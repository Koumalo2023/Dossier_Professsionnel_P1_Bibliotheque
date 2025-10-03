using AutoMapper;
using api.Models;

namespace api.Models
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            // UserProfile mappings
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.CategoryPreferences, opt => opt.MapFrom(src => src.CategoryPreferences))
                .ForMember(dest => dest.ReadingGoals, opt => opt.MapFrom(src => src.ReadingGoals))
                .ForMember(dest => dest.ReadingHistory, opt => opt.MapFrom(src => src.ReadingHistory));

            CreateMap<CreateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryPreferences, opt => opt.Ignore())
                .ForMember(dest => dest.ReadingGoals, opt => opt.Ignore())
                .ForMember(dest => dest.ReadingHistory, opt => opt.Ignore());

            CreateMap<UpdateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CategoryPreferences, opt => opt.Ignore())
                .ForMember(dest => dest.ReadingGoals, opt => opt.Ignore())
                .ForMember(dest => dest.ReadingHistory, opt => opt.Ignore());

            // UserCategoryPreference mappings
            CreateMap<UserCategoryPreference, UserCategoryPreferenceDto>();

            CreateMap<CreateUserCategoryPreferenceDto, UserCategoryPreference>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.LastInteracted, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // UserReadingGoal mappings
            CreateMap<UserReadingGoal, UserReadingGoalDto>()
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src =>
                    src.TargetBooks > 0 ? (double)src.CurrentProgress / src.TargetBooks * 100 : 0));

            CreateMap<CreateUserReadingGoalDto, UserReadingGoal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentProgress, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateUserReadingGoalDto, UserReadingGoal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CurrentProgress, opt => opt.Ignore());

            // UserReadingHistory mappings
            CreateMap<UserReadingHistory, UserReadingHistoryDto>();

            CreateMap<CreateUserReadingHistoryDto, UserReadingHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.Loan, opt => opt.Ignore());

            CreateMap<UpdateUserReadingHistoryDto, UserReadingHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.LoanId, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.Loan, opt => opt.Ignore());

            // Statistics mappings
            CreateMap<UserProfile, UserReadingStatsDto>()
                .ForMember(dest => dest.TotalBooksRead, opt => opt.MapFrom(src => src.ReadingHistory.Count))
                .ForMember(dest => dest.TotalReadingTimeMinutes, opt => opt.MapFrom(src => src.ReadingHistory.Sum(h => h.ReadingTimeMinutes ?? 0)))
                .ForMember(dest => dest.AverageReadingTime, opt => opt.MapFrom(src =>
                    src.ReadingHistory.Count > 0 ? src.ReadingHistory.Average(h => h.ReadingTimeMinutes ?? 0) : 0))
                .ForMember(dest => dest.FavoriteCategory, opt => opt.MapFrom(src =>
                    src.CategoryPreferences.OrderByDescending(cp => cp.PreferenceScore).Select(cp => cp.Category.Name).FirstOrDefault()))
                .ForMember(dest => dest.ActiveGoals, opt => opt.MapFrom(src =>
                    src.ReadingGoals.Count(g => g.IsActive)))
                .ForMember(dest => dest.CompletedGoals, opt => opt.MapFrom(src =>
                    src.ReadingGoals.Count(g => g.CurrentProgress >= g.TargetBooks)));

            // Helper methods for statistics
        }

        private static int CalculateReadingStreak(ICollection<UserReadingHistory> readingHistory)
        {
            if (readingHistory == null || !readingHistory.Any())
                return 0;

            var today = DateTime.UtcNow.Date;
            var readingDates = readingHistory
                .Where(h => h.StartedReading != null)
                .Select(h => h.StartedReading.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (!readingDates.Any())
                return 0;

            var streak = 0;
            var currentDate = today;

            while (readingDates.Contains(currentDate))
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }

            return streak;
        }

        private static int CalculateLongestReadingStreak(ICollection<UserReadingHistory> readingHistory)
        {
            if (readingHistory == null || !readingHistory.Any())
                return 0;

            var readingDates = readingHistory
                .Where(h => h.StartedReading != null)
                .Select(h => h.StartedReading.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (!readingDates.Any())
                return 0;

            var longestStreak = 1;
            var currentStreak = 1;

            for (int i = 1; i < readingDates.Count; i++)
            {
                var daysBetween = (readingDates[i] - readingDates[i - 1]).Days;
                if (daysBetween == 1)
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }
    }
}