using api.Helpers;
using api.Models;
using api.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /// <summary>
    /// Service pour la gestion des profils utilisateur
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            IBookRepository bookRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            _userProfileRepository = userProfileRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(Guid userId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var userProfileDto = _mapper.Map<UserProfileDto>(userProfile);
                return new ServiceResponse<UserProfileDto>
                {
                    Success = true,
                    Data = userProfileDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du profil utilisateur pour l'ID: {UserId}", userId);
                return new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération du profil utilisateur"
                };
            }
        }

        public async Task<ServiceResponse<UserProfileDto>> CreateUserProfileAsync(Guid userId, CreateUserProfileDto createDto)
        {
            try
            {
                // Vérifier si le profil existe déjà
                if (await _userProfileRepository.UserProfileExistsAsync(userId))
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Un profil existe déjà pour cet utilisateur"
                    };
                }

                var userProfile = _mapper.Map<UserProfile>(createDto);
                userProfile.UserId = userId;
                userProfile.CreatedAt = DateTime.UtcNow;
                userProfile.UpdatedAt = DateTime.UtcNow;

                var createdProfile = await _userProfileRepository.CreateAsync(userProfile);
                var userProfileDto = _mapper.Map<UserProfileDto>(createdProfile);

                return new ServiceResponse<UserProfileDto>
                {
                    Success = true,
                    Message = "Profil utilisateur créé avec succès",
                    Data = userProfileDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du profil utilisateur pour l'ID: {UserId}", userId);
                return new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Erreur lors de la création du profil utilisateur"
                };
            }
        }

        public async Task<ServiceResponse<UserProfileDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                // Mettre à jour les propriétés non nulles
                if (updateDto.Bio != null) userProfile.Bio = updateDto.Bio;
                if (updateDto.FavoriteAuthor != null) userProfile.FavoriteAuthor = updateDto.FavoriteAuthor;
                if (updateDto.FavoriteGenre != null) userProfile.FavoriteGenre = updateDto.FavoriteGenre;
                if (updateDto.EmailNotifications.HasValue) userProfile.EmailNotifications = updateDto.EmailNotifications.Value;
                if (updateDto.PushNotifications.HasValue) userProfile.PushNotifications = updateDto.PushNotifications.Value;
                if (updateDto.NewBookAlerts.HasValue) userProfile.NewBookAlerts = updateDto.NewBookAlerts.Value;
                if (updateDto.ReturnReminders.HasValue) userProfile.ReturnReminders = updateDto.ReturnReminders.Value;
                if (updateDto.ReviewReminders.HasValue) userProfile.ReviewReminders = updateDto.ReviewReminders.Value;
                if (updateDto.Theme != null) userProfile.Theme = updateDto.Theme;
                if (updateDto.ItemsPerPage.HasValue) userProfile.ItemsPerPage = updateDto.ItemsPerPage.Value;
                if (updateDto.Language != null) userProfile.Language = updateDto.Language;

                userProfile.UpdatedAt = DateTime.UtcNow;

                var updatedProfile = await _userProfileRepository.UpdateAsync(userProfile);
                var userProfileDto = _mapper.Map<UserProfileDto>(updatedProfile);

                return new ServiceResponse<UserProfileDto>
                {
                    Success = true,
                    Message = "Profil utilisateur mis à jour avec succès",
                    Data = userProfileDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du profil utilisateur pour l'ID: {UserId}", userId);
                return new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour du profil utilisateur"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteUserProfileAsync(Guid userId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var result = await _userProfileRepository.DeleteAsync(userProfile.Id);
                return new ServiceResponse<bool>
                {
                    Success = result,
                    Message = result ? "Profil utilisateur supprimé avec succès" : "Erreur lors de la suppression du profil"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du profil utilisateur pour l'ID: {UserId}", userId);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression du profil utilisateur"
                };
            }
        }

        public async Task<ServiceResponse<UserCategoryPreferenceDto>> AddOrUpdateCategoryPreferenceAsync(Guid userId, CreateUserCategoryPreferenceDto createDto)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserCategoryPreferenceDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var preference = new UserCategoryPreference
                {
                    UserProfileId = userProfile.Id,
                    CategoryId = createDto.CategoryId,
                    PreferenceScore = createDto.PreferenceScore,
                    LastInteracted = DateTime.UtcNow,
                    InteractionCount = 1
                };

                var updatedPreference = await _userProfileRepository.AddOrUpdateCategoryPreferenceAsync(preference);
                var preferenceDto = _mapper.Map<UserCategoryPreferenceDto>(updatedPreference);

                return new ServiceResponse<UserCategoryPreferenceDto>
                {
                    Success = true,
                    Message = "Préférence de catégorie mise à jour avec succès",
                    Data = preferenceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout/mise à jour de la préférence de catégorie pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<UserCategoryPreferenceDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de la préférence de catégorie"
                };
            }
        }

        public async Task<ServiceResponse<List<UserCategoryPreferenceDto>>> GetUserCategoryPreferencesAsync(Guid userId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<List<UserCategoryPreferenceDto>>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var preferences = await _userProfileRepository.GetUserCategoryPreferencesAsync(userProfile.Id);
                var preferenceDtos = _mapper.Map<List<UserCategoryPreferenceDto>>(preferences);

                return new ServiceResponse<List<UserCategoryPreferenceDto>>
                {
                    Success = true,
                    Data = preferenceDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des préférences de catégorie pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<List<UserCategoryPreferenceDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des préférences de catégorie"
                };
            }
        }

        public async Task<ServiceResponse<bool>> RemoveCategoryPreferenceAsync(Guid userId, Guid categoryId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var preference = await _userProfileRepository.GetCategoryPreferenceAsync(userProfile.Id, categoryId);
                if (preference == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Préférence de catégorie non trouvée"
                    };
                }

                var result = await _userProfileRepository.RemoveCategoryPreferenceAsync(preference.Id);
                return new ServiceResponse<bool>
                {
                    Success = result,
                    Message = result ? "Préférence de catégorie supprimée avec succès" : "Erreur lors de la suppression de la préférence"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la préférence de catégorie pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression de la préférence de catégorie"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingGoalDto>> CreateReadingGoalAsync(Guid userId, CreateUserReadingGoalDto createDto)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var goal = _mapper.Map<UserReadingGoal>(createDto);
                goal.UserProfileId = userProfile.Id;
                goal.CreatedAt = DateTime.UtcNow;
                goal.UpdatedAt = DateTime.UtcNow;

                var createdGoal = await _userProfileRepository.CreateReadingGoalAsync(goal);
                var goalDto = _mapper.Map<UserReadingGoalDto>(createdGoal);

                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = true,
                    Message = "Objectif de lecture créé avec succès",
                    Data = goalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'objectif de lecture pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = false,
                    Message = "Erreur lors de la création de l'objectif de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingGoalDto>> UpdateReadingGoalAsync(Guid userId, Guid goalId, UpdateUserReadingGoalDto updateDto)
        {
            try
            {
                var goal = await _userProfileRepository.GetReadingGoalByIdAsync(goalId);
                if (goal == null)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Objectif de lecture non trouvé"
                    };
                }

                // Vérifier que l'objectif appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || goal.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet objectif"
                    };
                }

                // Mettre à jour les propriétés non nulles
                if (updateDto.Title != null) goal.Title = updateDto.Title;
                if (updateDto.Description != null) goal.Description = updateDto.Description;
                if (updateDto.TargetBooks.HasValue) goal.TargetBooks = updateDto.TargetBooks.Value;
                if (updateDto.StartDate.HasValue) goal.StartDate = updateDto.StartDate.Value;
                if (updateDto.EndDate.HasValue) goal.EndDate = updateDto.EndDate.Value;
                if (updateDto.IsActive.HasValue) goal.IsActive = updateDto.IsActive.Value;

                goal.UpdatedAt = DateTime.UtcNow;

                var updatedGoal = await _userProfileRepository.UpdateReadingGoalAsync(goal);
                var goalDto = _mapper.Map<UserReadingGoalDto>(updatedGoal);

                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = true,
                    Message = "Objectif de lecture mis à jour avec succès",
                    Data = goalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'objectif de lecture {GoalId} pour l'utilisateur: {UserId}", goalId, userId);
                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de l'objectif de lecture"
                };
            }
        }

        public async Task<ServiceResponse<List<UserReadingGoalDto>>> GetUserReadingGoalsAsync(Guid userId, bool? activeOnly = null)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<List<UserReadingGoalDto>>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var goals = await _userProfileRepository.GetUserReadingGoalsAsync(userProfile.Id, activeOnly);
                var goalDtos = _mapper.Map<List<UserReadingGoalDto>>(goals);

                return new ServiceResponse<List<UserReadingGoalDto>>
                {
                    Success = true,
                    Data = goalDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des objectifs de lecture pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<List<UserReadingGoalDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des objectifs de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingGoalDto>> GetReadingGoalByIdAsync(Guid userId, Guid goalId)
        {
            try
            {
                var goal = await _userProfileRepository.GetReadingGoalByIdAsync(goalId);
                if (goal == null)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Objectif de lecture non trouvé"
                    };
                }

                // Vérifier que l'objectif appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || goal.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet objectif"
                    };
                }

                var goalDto = _mapper.Map<UserReadingGoalDto>(goal);
                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = true,
                    Data = goalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'objectif de lecture {GoalId} pour l'utilisateur: {UserId}", goalId, userId);
                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération de l'objectif de lecture"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteReadingGoalAsync(Guid userId, Guid goalId)
        {
            try
            {
                var goal = await _userProfileRepository.GetReadingGoalByIdAsync(goalId);
                if (goal == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Objectif de lecture non trouvé"
                    };
                }

                // Vérifier que l'objectif appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || goal.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet objectif"
                    };
                }

                var result = await _userProfileRepository.DeleteReadingGoalAsync(goalId);
                return new ServiceResponse<bool>
                {
                    Success = result,
                    Message = result ? "Objectif de lecture supprimé avec succès" : "Erreur lors de la suppression de l'objectif"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'objectif de lecture {GoalId} pour l'utilisateur: {UserId}", goalId, userId);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression de l'objectif de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingGoalDto>> UpdateReadingGoalProgressAsync(Guid userId, Guid goalId, int booksRead)
        {
            try
            {
                var goal = await _userProfileRepository.GetReadingGoalByIdAsync(goalId);
                if (goal == null)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Objectif de lecture non trouvé"
                    };
                }

                // Vérifier que l'objectif appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || goal.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<UserReadingGoalDto>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet objectif"
                    };
                }

                goal.CurrentProgress = booksRead;
                goal.IsCompleted = goal.CurrentProgress >= goal.TargetBooks;
                goal.UpdatedAt = DateTime.UtcNow;

                var updatedGoal = await _userProfileRepository.UpdateReadingGoalAsync(goal);
                var goalDto = _mapper.Map<UserReadingGoalDto>(updatedGoal);

                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = true,
                    Message = goal.IsCompleted ? "Félicitations ! Objectif atteint !" : "Progression de l'objectif mise à jour",
                    Data = goalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la progression de l'objectif {GoalId} pour l'utilisateur: {UserId}", goalId, userId);
                return new ServiceResponse<UserReadingGoalDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de la progression de l'objectif"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingHistoryDto>> CreateReadingHistoryAsync(Guid userId, CreateUserReadingHistoryDto createDto)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                // Vérifier que le livre existe
                var book = await _bookRepository.GetByIdAsync(createDto.BookId);
                if (book == null)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Livre non trouvé"
                    };
                }

                var history = _mapper.Map<UserReadingHistory>(createDto);
                history.UserProfileId = userProfile.Id;
                history.CreatedAt = DateTime.UtcNow;
                history.UpdatedAt = DateTime.UtcNow;

                var createdHistory = await _userProfileRepository.CreateReadingHistoryAsync(history);
                var historyDto = _mapper.Map<UserReadingHistoryDto>(createdHistory);

                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = true,
                    Message = "Historique de lecture créé avec succès",
                    Data = historyDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'historique de lecture pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la création de l'historique de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingHistoryDto>> UpdateReadingHistoryAsync(Guid userId, Guid historyId, UpdateUserReadingHistoryDto updateDto)
        {
            try
            {
                var history = await _userProfileRepository.GetReadingHistoryByIdAsync(historyId);
                if (history == null)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Historique de lecture non trouvé"
                    };
                }

                // Vérifier que l'historique appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || history.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet historique"
                    };
                }

                // Mettre à jour les propriétés non nulles
                if (updateDto.FinishedReading.HasValue) history.FinishedReading = updateDto.FinishedReading.Value;
                if (updateDto.ReadingTimeMinutes.HasValue) history.ReadingTimeMinutes = updateDto.ReadingTimeMinutes.Value;
                if (updateDto.Rating.HasValue) history.Rating = updateDto.Rating.Value;
                if (updateDto.Review != null) history.Review = updateDto.Review;
                if (updateDto.IsFavorite.HasValue) history.IsFavorite = updateDto.IsFavorite.Value;

                history.UpdatedAt = DateTime.UtcNow;

                var updatedHistory = await _userProfileRepository.UpdateReadingHistoryAsync(history);
                var historyDto = _mapper.Map<UserReadingHistoryDto>(updatedHistory);

                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = true,
                    Message = "Historique de lecture mis à jour avec succès",
                    Data = historyDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'historique de lecture {HistoryId} pour l'utilisateur: {UserId}", historyId, userId);
                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de l'historique de lecture"
                };
            }
        }

        public async Task<ServiceResponse<List<UserReadingHistoryDto>>> GetUserReadingHistoryAsync(Guid userId, int limit = 50)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<List<UserReadingHistoryDto>>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var history = await _userProfileRepository.GetUserReadingHistoryAsync(userProfile.Id, limit);
                var historyDtos = _mapper.Map<List<UserReadingHistoryDto>>(history);

                return new ServiceResponse<List<UserReadingHistoryDto>>
                {
                    Success = true,
                    Data = historyDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de lecture pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<List<UserReadingHistoryDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération de l'historique de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingHistoryDto>> GetReadingHistoryByLoanAsync(Guid userId, Guid loanId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var history = await _userProfileRepository.GetReadingHistoryByLoanAsync(userProfile.Id, loanId);
                if (history == null)
                {
                    return new ServiceResponse<UserReadingHistoryDto>
                    {
                        Success = false,
                        Message = "Historique de lecture non trouvé pour cet emprunt"
                    };
                }

                var historyDto = _mapper.Map<UserReadingHistoryDto>(history);
                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = true,
                    Data = historyDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de lecture par emprunt {LoanId} pour l'utilisateur: {UserId}", loanId, userId);
                return new ServiceResponse<UserReadingHistoryDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération de l'historique de lecture"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteReadingHistoryAsync(Guid userId, Guid historyId)
        {
            try
            {
                var history = await _userProfileRepository.GetReadingHistoryByIdAsync(historyId);
                if (history == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Historique de lecture non trouvé"
                    };
                }

                // Vérifier que l'historique appartient à l'utilisateur
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null || history.UserProfileId != userProfile.Id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Accès non autorisé à cet historique"
                    };
                }

                var result = await _userProfileRepository.DeleteReadingHistoryAsync(historyId);
                return new ServiceResponse<bool>
                {
                    Success = result,
                    Message = result ? "Historique de lecture supprimé avec succès" : "Erreur lors de la suppression de l'historique"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'historique de lecture {HistoryId} pour l'utilisateur: {UserId}", historyId, userId);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression de l'historique de lecture"
                };
            }
        }

        public async Task<ServiceResponse<UserReadingStatsDto>> GetUserReadingStatsAsync(Guid userId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
                if (userProfile == null)
                {
                    return new ServiceResponse<UserReadingStatsDto>
                    {
                        Success = false,
                        Message = "Profil utilisateur non trouvé"
                    };
                }

                var history = await _userProfileRepository.GetUserReadingHistoryAsync(userProfile.Id, int.MaxValue);
                var goals = await _userProfileRepository.GetUserReadingGoalsAsync(userProfile.Id, true);

                var stats = new UserReadingStatsDto
                {
                    TotalBooksRead = history.Count(h => h.FinishedReading.HasValue),
                    TotalPagesRead = history.Where(h => h.FinishedReading.HasValue && h.Book.PageCount.HasValue)
                                           .Sum(h => h.Book.PageCount.Value),
                    AverageReadingTime = history.Where(h => h.ReadingTimeMinutes.HasValue)
                                               .Average(h => h.ReadingTimeMinutes.Value),
                    ActiveGoals = goals.Count(g => g.IsActive && !g.IsCompleted),
                    CompletedGoals = goals.Count(g => g.IsCompleted),
                    FavoriteCategory = userProfile.FavoriteGenre,
                    FavoriteAuthor = userProfile.FavoriteAuthor,
                    TotalReadingTimeMinutes = history.Where(h => h.ReadingTimeMinutes.HasValue)
                                                    .Sum(h => h.ReadingTimeMinutes.Value),
                    BooksPerMonth = CalculateBooksPerMonth(history),
                    BooksByCategory = CalculateBooksByCategory(history),
                    RatingDistribution = CalculateRatingDistribution(history)
                };

                return new ServiceResponse<UserReadingStatsDto>
                {
                    Success = true,
                    Data = stats
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques de lecture pour l'utilisateur: {UserId}", userId);
                return new ServiceResponse<UserReadingStatsDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des statistiques de lecture"
                };
            }
        }

        public async Task<ServiceResponse<bool>> InitializeUserProfileAsync(Guid userId)
        {
            try
            {
                if (await _userProfileRepository.UserProfileExistsAsync(userId))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Le profil existe déjà"
                    };
                }

                var createDto = new CreateUserProfileDto();
                var result = await CreateUserProfileAsync(userId, createDto);

                return new ServiceResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation du profil utilisateur pour l'ID: {UserId}", userId);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Erreur lors de l'initialisation du profil utilisateur"
                };
            }
        }

        public async Task<ServiceResponse<List<UserProfileDto>>> GetProfilesWithActiveGoalsAsync()
        {
            try
            {
                var profiles = await _userProfileRepository.GetProfilesWithActiveGoalsAsync();
                var profileDtos = _mapper.Map<List<UserProfileDto>>(profiles);

                return new ServiceResponse<List<UserProfileDto>>
                {
                    Success = true,
                    Data = profileDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des profils avec objectifs actifs");
                return new ServiceResponse<List<UserProfileDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des profils"
                };
            }
        }

        // Méthodes utilitaires privées
        private double CalculateBooksPerMonth(List<UserReadingHistory> history)
        {
            var finishedBooks = history.Where(h => h.FinishedReading.HasValue).ToList();
            if (!finishedBooks.Any()) return 0;

            var firstBook = finishedBooks.Min(h => h.FinishedReading.Value);
            var lastBook = finishedBooks.Max(h => h.FinishedReading.Value);
            var months = (lastBook - firstBook).TotalDays / 30.0;

            return months > 0 ? finishedBooks.Count / months : finishedBooks.Count;
        }

        private Dictionary<string, int> CalculateBooksByCategory(List<UserReadingHistory> history)
        {
            return history
                .Where(h => h.FinishedReading.HasValue)
                .GroupBy(h => h.Book.Category.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<int, int> CalculateRatingDistribution(List<UserReadingHistory> history)
        {
            return history
                .Where(h => h.Rating.HasValue)
                .GroupBy(h => h.Rating.Value)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}