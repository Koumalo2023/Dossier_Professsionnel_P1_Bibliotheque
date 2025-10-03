using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace api.Helpers
{
    /// <summary>
    /// Helper pour gérer l'upload et la gestion des fichiers, notamment les images de couverture des livres.
    /// </summary>
    public static class FileUploadHelper
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

        /// <summary>
        /// Répertoire de base pour le stockage des images de couverture.
        /// </summary>
        public static string BaseUploadPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "covers");

        /// <summary>
        /// URL de base pour accéder aux images uploadées.
        /// </summary>
        public static string BaseUrlPath => "/uploads/covers/";

        /// <summary>
        /// Valide un fichier image avant l'upload.
        /// </summary>
        /// <param name="file">Fichier à valider</param>
        /// <returns>Résultat de la validation avec message d'erreur si applicable</returns>
        public static (bool IsValid, string ErrorMessage) ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Aucun fichier fourni.");
            }

            if (file.Length > MaxFileSize)
            {
                return (false, $"Le fichier est trop volumineux. Taille maximale autorisée : {MaxFileSize / (1024 * 1024)}MB.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !AllowedImageExtensions.Contains(fileExtension))
            {
                return (false, $"Type de fichier non autorisé. Types autorisés : {string.Join(", ", AllowedImageExtensions)}");
            }

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return (false, "Type MIME du fichier non autorisé.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Génère un nom de fichier unique pour éviter les collisions.
        /// </summary>
        /// <param name="originalFileName">Nom de fichier original</param>
        /// <returns>Nom de fichier unique</returns>
        public static string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var random = Guid.NewGuid().ToString("N").Substring(0, 8);
            
            // Nettoyer le nom de fichier pour enlever les caractères spéciaux
            var cleanName = CleanFileName(fileNameWithoutExtension);
            
            return $"{cleanName}_{timestamp}_{random}{extension}";
        }

        /// <summary>
        /// Sauvegarde un fichier image sur le serveur.
        /// </summary>
        /// <param name="file">Fichier à sauvegarder</param>
        /// <param name="customFileName">Nom de fichier personnalisé (optionnel)</param>
        /// <returns>Chemin relatif du fichier sauvegardé</returns>
        public static async Task<string> SaveImageFileAsync(IFormFile file, string customFileName = null)
        {
            // Valider le fichier
            var validationResult = ValidateImageFile(file);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(validationResult.ErrorMessage);
            }

            // S'assurer que le répertoire existe
            if (!Directory.Exists(BaseUploadPath))
            {
                Directory.CreateDirectory(BaseUploadPath);
            }

            // Générer le nom de fichier
            var fileName = customFileName ?? GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(BaseUploadPath, fileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(BaseUrlPath, fileName).Replace("\\", "/");
        }

        /// <summary>
        /// Supprime un fichier image du serveur.
        /// </summary>
        /// <param name="fileUrl">URL ou chemin relatif du fichier</param>
        /// <returns>True si le fichier a été supprimé, false s'il n'existait pas</returns>
        public static bool DeleteImageFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return false;
            }

            try
            {
                // Extraire le nom de fichier de l'URL
                var fileName = Path.GetFileName(fileUrl);
                if (string.IsNullOrEmpty(fileName))
                {
                    return false;
                }

                var filePath = Path.Combine(BaseUploadPath, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                
                return false;
            }
            catch
            {
                // Loguer l'erreur en production
                return false;
            }
        }

        /// <summary>
        /// Vérifie si un fichier image existe sur le serveur.
        /// </summary>
        /// <param name="fileUrl">URL ou chemin relatif du fichier</param>
        /// <returns>True si le fichier existe</returns>
        public static bool ImageFileExists(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return false;
            }

            var fileName = Path.GetFileName(fileUrl);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var filePath = Path.Combine(BaseUploadPath, fileName);
            return File.Exists(filePath);
        }

        /// <summary>
        /// Récupère le chemin physique complet d'un fichier à partir de son URL.
        /// </summary>
        /// <param name="fileUrl">URL ou chemin relatif du fichier</param>
        /// <returns>Chemin physique complet</returns>
        public static string GetPhysicalPath(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return string.Empty;
            }

            var fileName = Path.GetFileName(fileUrl);
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            return Path.Combine(BaseUploadPath, fileName);
        }

        /// <summary>
        /// Nettoie un nom de fichier en supprimant les caractères spéciaux.
        /// </summary>
        /// <param name="fileName">Nom de fichier à nettoyer</param>
        /// <returns>Nom de fichier nettoyé</returns>
        private static string CleanFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            // Remplacer les caractères non autorisés par des underscores
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanName = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                .Trim()
                .Replace(" ", "_")
                .ToLowerInvariant();

            // Limiter la longueur du nom
            return cleanName.Length > 50 ? cleanName.Substring(0, 50) : cleanName;
        }

        /// <summary>
        /// Obtient la taille d'un fichier en octets.
        /// </summary>
        /// <param name="fileUrl">URL ou chemin relatif du fichier</param>
        /// <returns>Taille du fichier en octets, ou -1 si le fichier n'existe pas</returns>
        public static long GetFileSize(string fileUrl)
        {
            var physicalPath = GetPhysicalPath(fileUrl);
            if (File.Exists(physicalPath))
            {
                return new FileInfo(physicalPath).Length;
            }
            return -1;
        }

        /// <summary>
        /// Obtient les informations de base sur un fichier image.
        /// </summary>
        /// <param name="fileUrl">URL ou chemin relatif du fichier</param>
        /// <returns>Informations sur le fichier</returns>
        public static (bool Exists, long Size, DateTime? LastModified) GetFileInfo(string fileUrl)
        {
            var physicalPath = GetPhysicalPath(fileUrl);
            if (File.Exists(physicalPath))
            {
                var fileInfo = new FileInfo(physicalPath);
                return (true, fileInfo.Length, fileInfo.LastWriteTime);
            }
            return (false, -1, null);
        }
    }
}