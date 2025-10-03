using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Helpers
{
    /// <summary>
    /// Classe générique pour représenter un résultat paginé.
    /// </summary>
    /// <typeparam name="T">Type des éléments dans la collection</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Collection d'éléments pour la page courante.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Numéro de la page courante (commence à 1).
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Nombre d'éléments par page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Nombre total d'éléments dans la collection complète.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Nombre total de pages disponibles.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Indique s'il y a une page précédente.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indique s'il y a une page suivante.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Numéro de la page précédente.
        /// </summary>
        public int? PreviousPage => HasPrevious ? CurrentPage - 1 : null;

        /// <summary>
        /// Numéro de la page suivante.
        /// </summary>
        public int? NextPage => HasNext ? CurrentPage + 1 : null;

        /// <summary>
        /// Index du premier élément de la page courante.
        /// </summary>
        public int FirstItemIndex => (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// Index du dernier élément de la page courante.
        /// </summary>
        public int LastItemIndex => Math.Min(CurrentPage * PageSize, TotalCount);

        /// <summary>
        /// Initialise une nouvelle instance de PagedResult.
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de PagedResult avec des paramètres.
        /// </summary>
        /// <param name="items">Collection d'éléments</param>
        /// <param name="totalCount">Nombre total d'éléments</param>
        /// <param name="currentPage">Numéro de page courant</param>
        /// <param name="pageSize">Taille de la page</param>
        public PagedResult(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// Helper pour gérer la pagination des collections.
    /// </summary>
    public static class PaginationHelper
    {
        /// <summary>
        /// Crée un résultat paginé à partir d'une requête IQueryable.
        /// </summary>
        /// <typeparam name="T">Type des éléments</typeparam>
        /// <param name="query">Requête IQueryable à paginer</param>
        /// <param name="pageNumber">Numéro de page (commence à 1)</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Résultat paginé</returns>
        public static PagedResult<T> CreatePagedResult<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Crée un résultat paginé à partir d'une collection IEnumerable.
        /// </summary>
        /// <typeparam name="T">Type des éléments</typeparam>
        /// <param name="collection">Collection à paginer</param>
        /// <param name="pageNumber">Numéro de page (commence à 1)</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Résultat paginé</returns>
        public static PagedResult<T> CreatePagedResult<T>(IEnumerable<T> collection, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = collection.Count();
            var items = collection
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Valide les paramètres de pagination.
        /// </summary>
        /// <param name="pageNumber">Numéro de page à valider</param>
        /// <param name="pageSize">Taille de page à valider</param>
        /// <param name="maxPageSize">Taille de page maximale autorisée</param>
        /// <returns>True si les paramètres sont valides, sinon false</returns>
        public static bool ValidatePaginationParameters(int pageNumber, int pageSize, int maxPageSize = 100)
        {
            return pageNumber >= 1 && pageSize >= 1 && pageSize <= maxPageSize;
        }

        /// <summary>
        /// Calcule le nombre total de pages.
        /// </summary>
        /// <param name="totalCount">Nombre total d'éléments</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Nombre total de pages</returns>
        public static int CalculateTotalPages(int totalCount, int pageSize)
        {
            if (pageSize <= 0) return 0;
            return (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// Calcule l'index de départ pour la pagination.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Index de départ</returns>
        public static int CalculateStartIndex(int pageNumber, int pageSize)
        {
            return (pageNumber - 1) * pageSize;
        }
    }
}