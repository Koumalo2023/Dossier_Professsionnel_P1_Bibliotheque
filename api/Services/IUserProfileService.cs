using api.Helpers;
using api.Models;

namespace api.Services
{
    /// <summary>
    /// Interface pour le service de gestion des profils utilisateur
    /// </summary>
    public interface IUserProfileService
    {
        // Profil utilisateur
        Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(Guid userId);
        Task<ServiceResponse<UserProfileDto>> CreateUserProfileAsync(Guid userId, CreateUserProfileDto createDto);
        Task<ServiceResponse<UserProfileDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateDto);
        Task<ServiceResponse<bool>> DeleteUserProfileAsync(Guid userId);

        // Préférences de catégorie
        Task<ServiceResponse<UserCategoryPreferenceDto>> AddOrUpdateCategoryPreferenceAsync(Guid userId, CreateUserCategoryPreferenceDto createDto);
        Task<ServiceResponse<List<UserCategoryPreferenceDto>>> GetUserCategoryPreferencesAsync(Guid userId);
        Task<ServiceResponse<bool>> RemoveCategoryPreferenceAsync(Guid userId, Guid categoryId);

        // Objectifs de lecture
        Task<ServiceResponse<UserReadingGoalDto>> CreateReadingGoalAsync(Guid userId, CreateUserReadingGoalDto createDto);
        Task<ServiceResponse<UserReadingGoalDto>> UpdateReadingGoalAsync(Guid userId, Guid goalId, UpdateUserReadingGoalDto updateDto);
        Task<ServiceResponse<List<UserReadingGoalDto>>> GetUserReadingGoalsAsync(Guid userId, bool? activeOnly = null);
        Task<ServiceResponse<UserReadingGoalDto>> GetReadingGoalByIdAsync(Guid userId, Guid goalId);
        Task<ServiceResponse<bool>> DeleteReadingGoalAsync(Guid userId, Guid goalId);
        Task<ServiceResponse<UserReadingGoalDto>> UpdateReadingGoalProgressAsync(Guid userId, Guid goalId, int booksRead);

        // Historique de lecture
        Task<ServiceResponse<UserReadingHistoryDto>> CreateReadingHistoryAsync(Guid userId, CreateUserReadingHistoryDto createDto);
        Task<ServiceResponse<UserReadingHistoryDto>> UpdateReadingHistoryAsync(Guid userId, Guid historyId, UpdateUserReadingHistoryDto updateDto);
        Task<ServiceResponse<List<UserReadingHistoryDto>>> GetUserReadingHistoryAsync(Guid userId, int limit = 50);
        Task<ServiceResponse<UserReadingHistoryDto>> GetReadingHistoryByLoanAsync(Guid userId, Guid loanId);
        Task<ServiceResponse<bool>> DeleteReadingHistoryAsync(Guid userId, Guid historyId);

        // Statistiques et utilitaires
        Task<ServiceResponse<UserReadingStatsDto>> GetUserReadingStatsAsync(Guid userId);
        Task<ServiceResponse<bool>> InitializeUserProfileAsync(Guid userId);
        Task<ServiceResponse<List<UserProfileDto>>> GetProfilesWithActiveGoalsAsync();
    }
}