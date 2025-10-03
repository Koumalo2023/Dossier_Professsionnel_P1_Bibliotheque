using api.Models; 
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère le profil de l'utilisateur connecté
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetUserProfileAsync(userId);
                
                if (!result.Success)
                    return NotFound(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du profil utilisateur");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Crée un profil pour l'utilisateur connecté
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromBody] CreateUserProfileDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.CreateUserProfileAsync(userId, createDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return CreatedAtAction(nameof(GetUserProfile), result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du profil utilisateur");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Met à jour le profil de l'utilisateur connecté
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.UpdateUserProfileAsync(userId, updateDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du profil utilisateur");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Supprime le profil de l'utilisateur connecté
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.DeleteUserProfileAsync(userId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du profil utilisateur");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Initialise le profil utilisateur (création avec valeurs par défaut)
        /// </summary>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.InitializeUserProfileAsync(userId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = "Profil utilisateur initialisé avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation du profil utilisateur");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère les préférences de catégorie de l'utilisateur
        /// </summary>
        [HttpGet("preferences/categories")]
        public async Task<IActionResult> GetCategoryPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetUserCategoryPreferencesAsync(userId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des préférences de catégorie");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Ajoute ou met à jour une préférence de catégorie
        /// </summary>
        [HttpPost("preferences/categories")]
        public async Task<IActionResult> AddOrUpdateCategoryPreference([FromBody] CreateUserCategoryPreferenceDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.AddOrUpdateCategoryPreferenceAsync(userId, createDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout/mise à jour de la préférence de catégorie");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Supprime une préférence de catégorie
        /// </summary>
        [HttpDelete("preferences/categories/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryPreference(Guid categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.RemoveCategoryPreferenceAsync(userId, categoryId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la préférence de catégorie");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère les objectifs de lecture de l'utilisateur
        /// </summary>
        [HttpGet("goals")]
        public async Task<IActionResult> GetReadingGoals([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetUserReadingGoalsAsync(userId, activeOnly);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des objectifs de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Crée un nouvel objectif de lecture
        /// </summary>
        [HttpPost("goals")]
        public async Task<IActionResult> CreateReadingGoal([FromBody] CreateUserReadingGoalDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.CreateReadingGoalAsync(userId, createDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return CreatedAtAction(nameof(GetReadingGoal), new { goalId = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'objectif de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère un objectif de lecture spécifique
        /// </summary>
        [HttpGet("goals/{goalId}")]
        public async Task<IActionResult> GetReadingGoal(Guid goalId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetReadingGoalByIdAsync(userId, goalId);
                
                if (!result.Success)
                    return NotFound(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'objectif de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Met à jour un objectif de lecture
        /// </summary>
        [HttpPut("goals/{goalId}")]
        public async Task<IActionResult> UpdateReadingGoal(Guid goalId, [FromBody] UpdateUserReadingGoalDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.UpdateReadingGoalAsync(userId, goalId, updateDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'objectif de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Supprime un objectif de lecture
        /// </summary>
        [HttpDelete("goals/{goalId}")]
        public async Task<IActionResult> DeleteReadingGoal(Guid goalId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.DeleteReadingGoalAsync(userId, goalId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'objectif de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Met à jour la progression d'un objectif de lecture
        /// </summary>
        [HttpPut("goals/{goalId}/progress")]
        public async Task<IActionResult> UpdateReadingGoalProgress(Guid goalId, [FromBody] UpdateReadingGoalProgressRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.UpdateReadingGoalProgressAsync(userId, goalId, request.BooksRead);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la progression de l'objectif");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère l'historique de lecture de l'utilisateur
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetReadingHistory([FromQuery] int limit = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetUserReadingHistoryAsync(userId, limit);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère l'historique de lecture par emprunt
        /// </summary>
        [HttpGet("history/loan/{loanId}")]
        public async Task<IActionResult> GetReadingHistoryByLoan(Guid loanId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetReadingHistoryByLoanAsync(userId, loanId);
                
                if (!result.Success)
                    return NotFound(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de lecture par emprunt");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Crée un nouvel historique de lecture
        /// </summary>
        [HttpPost("history")]
        public async Task<IActionResult> CreateReadingHistory([FromBody] CreateUserReadingHistoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.CreateReadingHistoryAsync(userId, createDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return CreatedAtAction(nameof(GetReadingHistory), result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'historique de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Met à jour un historique de lecture
        /// </summary>
        [HttpPut("history/{historyId}")]
        public async Task<IActionResult> UpdateReadingHistory(Guid historyId, [FromBody] UpdateUserReadingHistoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.UpdateReadingHistoryAsync(userId, historyId, updateDto);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'historique de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Supprime un historique de lecture
        /// </summary>
        [HttpDelete("history/{historyId}")]
        public async Task<IActionResult> DeleteReadingHistory(Guid historyId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.DeleteReadingHistoryAsync(userId, historyId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'historique de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère les statistiques de lecture de l'utilisateur
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetReadingStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Utilisateur non authentifié" });

                var result = await _userProfileService.GetUserReadingStatsAsync(userId);
                
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques de lecture");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Extrait l'ID de l'utilisateur connecté
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Requête pour mettre à jour la progression d'un objectif
    /// </summary>
    public class UpdateReadingGoalProgressRequest
    {
        public int BooksRead { get; set; }
    }
}