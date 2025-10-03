using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des profils utilisateur
    /// </summary>
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        // Profil utilisateur
        public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles
                .Include(up => up.CategoryPreferences)
                    .ThenInclude(cp => cp.Category)
                .Include(up => up.ReadingGoals)
                .Include(up => up.ReadingHistory)
                    .ThenInclude(rh => rh.Book)
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task<UserProfile> CreateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
        {
            userProfile.UpdatedAt = DateTime.UtcNow;
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<bool> DeleteAsync(Guid userProfileId)
        {
            var userProfile = await _context.UserProfiles.FindAsync(userProfileId);
            if (userProfile == null) return false;

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            return true;
        }

        // Préférences de catégorie
        public async Task<UserCategoryPreference?> GetCategoryPreferenceAsync(Guid userProfileId, Guid categoryId)
        {
            return await _context.UserCategoryPreferences
                .Include(cp => cp.Category)
                .FirstOrDefaultAsync(cp => cp.UserProfileId == userProfileId && cp.CategoryId == categoryId);
        }

        public async Task<UserCategoryPreference> AddOrUpdateCategoryPreferenceAsync(UserCategoryPreference preference)
        {
            var existing = await _context.UserCategoryPreferences
                .FirstOrDefaultAsync(cp => cp.UserProfileId == preference.UserProfileId && cp.CategoryId == preference.CategoryId);

            if (existing != null)
            {
                existing.PreferenceScore = preference.PreferenceScore;
                existing.LastInteracted = DateTime.UtcNow;
                existing.InteractionCount++;
                _context.UserCategoryPreferences.Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }
            else
            {
                _context.UserCategoryPreferences.Add(preference);
                await _context.SaveChangesAsync();
                return preference;
            }
        }

        public async Task<List<UserCategoryPreference>> GetUserCategoryPreferencesAsync(Guid userProfileId)
        {
            return await _context.UserCategoryPreferences
                .Include(cp => cp.Category)
                .Where(cp => cp.UserProfileId == userProfileId)
                .OrderByDescending(cp => cp.PreferenceScore)
                .ThenByDescending(cp => cp.LastInteracted)
                .ToListAsync();
        }

        public async Task<bool> RemoveCategoryPreferenceAsync(Guid preferenceId)
        {
            var preference = await _context.UserCategoryPreferences.FindAsync(preferenceId);
            if (preference == null) return false;

            _context.UserCategoryPreferences.Remove(preference);
            await _context.SaveChangesAsync();
            return true;
        }

        // Objectifs de lecture
        public async Task<UserReadingGoal?> GetReadingGoalByIdAsync(Guid goalId)
        {
            return await _context.UserReadingGoals.FindAsync(goalId);
        }

        public async Task<List<UserReadingGoal>> GetUserReadingGoalsAsync(Guid userProfileId, bool? activeOnly = null)
        {
            var query = _context.UserReadingGoals
                .Where(g => g.UserProfileId == userProfileId);

            if (activeOnly.HasValue)
            {
                query = query.Where(g => g.IsActive == activeOnly.Value);
            }

            return await query
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserReadingGoal> CreateReadingGoalAsync(UserReadingGoal goal)
        {
            _context.UserReadingGoals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<UserReadingGoal> UpdateReadingGoalAsync(UserReadingGoal goal)
        {
            goal.UpdatedAt = DateTime.UtcNow;
            _context.UserReadingGoals.Update(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<bool> DeleteReadingGoalAsync(Guid goalId)
        {
            var goal = await _context.UserReadingGoals.FindAsync(goalId);
            if (goal == null) return false;

            _context.UserReadingGoals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

        // Historique de lecture
        public async Task<UserReadingHistory?> GetReadingHistoryByIdAsync(Guid historyId)
        {
            return await _context.UserReadingHistories
                .Include(rh => rh.Book)
                .FirstOrDefaultAsync(rh => rh.Id == historyId);
        }

        public async Task<List<UserReadingHistory>> GetUserReadingHistoryAsync(Guid userProfileId, int limit = 50)
        {
            return await _context.UserReadingHistories
                .Include(rh => rh.Book)
                .Where(rh => rh.UserProfileId == userProfileId)
                .OrderByDescending(rh => rh.StartedReading)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<UserReadingHistory?> GetReadingHistoryByLoanAsync(Guid userProfileId, Guid loanId)
        {
            return await _context.UserReadingHistories
                .Include(rh => rh.Book)
                .FirstOrDefaultAsync(rh => rh.UserProfileId == userProfileId && rh.LoanId == loanId);
        }

        public async Task<UserReadingHistory> CreateReadingHistoryAsync(UserReadingHistory history)
        {
            _context.UserReadingHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<UserReadingHistory> UpdateReadingHistoryAsync(UserReadingHistory history)
        {
            history.UpdatedAt = DateTime.UtcNow;
            _context.UserReadingHistories.Update(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<bool> DeleteReadingHistoryAsync(Guid historyId)
        {
            var history = await _context.UserReadingHistories.FindAsync(historyId);
            if (history == null) return false;

            _context.UserReadingHistories.Remove(history);
            await _context.SaveChangesAsync();
            return true;
        }

        // Méthodes utilitaires
        public async Task<bool> UserProfileExistsAsync(Guid userId)
        {
            return await _context.UserProfiles.AnyAsync(up => up.UserId == userId);
        }

        public async Task<List<UserProfile>> GetProfilesWithActiveGoalsAsync()
        {
            return await _context.UserProfiles
                .Include(up => up.ReadingGoals)
                .Where(up => up.ReadingGoals.Any(g => g.IsActive && !g.IsCompleted))
                .ToListAsync();
        }

        public async Task<List<UserProfile>> GetProfilesByCategoryPreferenceAsync(Guid categoryId)
        {
            return await _context.UserProfiles
                .Include(up => up.CategoryPreferences)
                .Where(up => up.CategoryPreferences.Any(cp => cp.CategoryId == categoryId && cp.PreferenceScore >= 7))
                .ToListAsync();
        }
    }
}