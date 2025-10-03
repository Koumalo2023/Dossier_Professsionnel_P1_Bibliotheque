using api.Models;

namespace api.Repositories
{
    /// <summary>
    /// Interface pour le repository de gestion des profils utilisateur
    /// </summary>
    public interface IUserProfileRepository
    {
        // Profil utilisateur
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task<UserProfile> UpdateAsync(UserProfile userProfile);
        Task<bool> DeleteAsync(Guid userProfileId);

        // Préférences de catégorie
        Task<UserCategoryPreference?> GetCategoryPreferenceAsync(Guid userProfileId, Guid categoryId);
        Task<UserCategoryPreference> AddOrUpdateCategoryPreferenceAsync(UserCategoryPreference preference);
        Task<List<UserCategoryPreference>> GetUserCategoryPreferencesAsync(Guid userProfileId);
        Task<bool> RemoveCategoryPreferenceAsync(Guid preferenceId);

        // Objectifs de lecture
        Task<UserReadingGoal?> GetReadingGoalByIdAsync(Guid goalId);
        Task<List<UserReadingGoal>> GetUserReadingGoalsAsync(Guid userProfileId, bool? activeOnly = null);
        Task<UserReadingGoal> CreateReadingGoalAsync(UserReadingGoal goal);
        Task<UserReadingGoal> UpdateReadingGoalAsync(UserReadingGoal goal);
        Task<bool> DeleteReadingGoalAsync(Guid goalId);

        // Historique de lecture
        Task<UserReadingHistory?> GetReadingHistoryByIdAsync(Guid historyId);
        Task<List<UserReadingHistory>> GetUserReadingHistoryAsync(Guid userProfileId, int limit = 50);
        Task<UserReadingHistory?> GetReadingHistoryByLoanAsync(Guid userProfileId, Guid loanId);
        Task<UserReadingHistory> CreateReadingHistoryAsync(UserReadingHistory history);
        Task<UserReadingHistory> UpdateReadingHistoryAsync(UserReadingHistory history);
        Task<bool> DeleteReadingHistoryAsync(Guid historyId);

        // Méthodes utilitaires
        Task<bool> UserProfileExistsAsync(Guid userId);
        Task<List<UserProfile>> GetProfilesWithActiveGoalsAsync();
        Task<List<UserProfile>> GetProfilesByCategoryPreferenceAsync(Guid categoryId);
    }
}