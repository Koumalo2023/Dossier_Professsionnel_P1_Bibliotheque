// Utilitaires de pagination

export interface PaginationConfig {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  pagination: PaginationConfig;
}

export class PaginationUtils {
  // Calculer le nombre total de pages
  static calculateTotalPages(totalItems: number, pageSize: number): number {
    if (totalItems <= 0 || pageSize <= 0) return 0;
    return Math.ceil(totalItems / pageSize);
  }

  // Calculer l'offset pour la requête SQL
  static calculateOffset(page: number, pageSize: number): number {
    return (page - 1) * pageSize;
  }

  // Vérifier si une page est valide
  static isValidPage(page: number, totalPages: number): boolean {
    return page >= 1 && page <= totalPages;
  }

  // Obtenir la plage de pages à afficher (pour la pagination UI)
  static getPageRange(
    currentPage: number, 
    totalPages: number, 
    maxVisiblePages: number = 5
  ): number[] {
    if (totalPages <= 1) return [1];
    
    const halfRange = Math.floor(maxVisiblePages / 2);
    let startPage = Math.max(1, currentPage - halfRange);
    let endPage = Math.min(totalPages, currentPage + halfRange);
    
    // Ajuster si on est proche du début
    if (currentPage <= halfRange) {
      endPage = Math.min(totalPages, maxVisiblePages);
    }
    
    // Ajuster si on est proche de la fin
    if (currentPage > totalPages - halfRange) {
      startPage = Math.max(1, totalPages - maxVisiblePages + 1);
    }
    
    const pages: number[] = [];
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  // Générer la configuration de pagination
  static createPaginationConfig(
    page: number,
    pageSize: number,
    totalItems: number
  ): PaginationConfig {
    const totalPages = this.calculateTotalPages(totalItems, pageSize);
    
    return {
      page: Math.max(1, Math.min(page, totalPages)),
      pageSize: Math.max(1, pageSize),
      totalItems: Math.max(0, totalItems),
      totalPages: totalPages
    };
  }

  // Paginer un tableau de données
  static paginateArray<T>(
    items: T[],
    page: number,
    pageSize: number
  ): PaginatedResponse<T> {
    const totalItems = items.length;
    const pagination = this.createPaginationConfig(page, pageSize, totalItems);
    
    const startIndex = this.calculateOffset(pagination.page, pagination.pageSize);
    const endIndex = startIndex + pagination.pageSize;
    const paginatedItems = items.slice(startIndex, endIndex);
    
    return {
      items: paginatedItems,
      pagination
    };
  }

  // Vérifier s'il y a une page précédente
  static hasPreviousPage(currentPage: number): boolean {
    return currentPage > 1;
  }

  // Vérifier s'il y a une page suivante
  static hasNextPage(currentPage: number, totalPages: number): boolean {
    return currentPage < totalPages;
  }

  // Obtenir le numéro de la page précédente
  static getPreviousPage(currentPage: number): number {
    return Math.max(1, currentPage - 1);
  }

  // Obtenir le numéro de la page suivante
  static getNextPage(currentPage: number, totalPages: number): number {
    return Math.min(totalPages, currentPage + 1);
  }

  // Obtenir le premier élément de la page actuelle
  static getFirstItemOnPage(currentPage: number, pageSize: number): number {
    return this.calculateOffset(currentPage, pageSize) + 1;
  }

  // Obtenir le dernier élément de la page actuelle
  static getLastItemOnPage(
    currentPage: number, 
    pageSize: number, 
    totalItems: number
  ): number {
    const lastItem = currentPage * pageSize;
    return Math.min(lastItem, totalItems);
  }

  // Formater les informations de pagination pour l'affichage
  static formatPaginationInfo(
    currentPage: number,
    pageSize: number,
    totalItems: number
  ): string {
    if (totalItems === 0) {
      return 'Aucun élément';
    }
    
    const firstItem = this.getFirstItemOnPage(currentPage, pageSize);
    const lastItem = this.getLastItemOnPage(currentPage, pageSize, totalItems);
    const totalPages = this.calculateTotalPages(totalItems, pageSize);
    
    return `${firstItem}-${lastItem} sur ${totalItems} élément${totalItems > 1 ? 's' : ''} (Page ${currentPage}/${totalPages})`;
  }

  // Valider et corriger les paramètres de pagination
  static validateAndFixPaginationParams(
    page: number,
    pageSize: number,
    maxPageSize: number = 100
  ): { page: number; pageSize: number } {
    const validPageSize = Math.max(1, Math.min(pageSize, maxPageSize));
    const validPage = Math.max(1, page);
    
    return {
      page: validPage,
      pageSize: validPageSize
    };
  }

  // Générer les options de taille de page
  static getPageSizeOptions(
    defaultSizes: number[] = [10, 25, 50, 100]
  ): number[] {
    return [...defaultSizes];
  }

  // Calculer le nombre d'éléments à sauter pour une requête
  static getSkipCount(page: number, pageSize: number): number {
    return (page - 1) * pageSize;
  }

  // Créer un objet de métadonnées de pagination pour les réponses API
  static createPaginationMetadata(
    page: number,
    pageSize: number,
    totalItems: number
  ) {
    const totalPages = this.calculateTotalPages(totalItems, pageSize);
    
    return {
      currentPage: page,
      pageSize,
      totalItems,
      totalPages,
      hasNext: this.hasNextPage(page, totalPages),
      hasPrevious: this.hasPreviousPage(page),
      nextPage: this.getNextPage(page, totalPages),
      previousPage: this.getPreviousPage(page)
    };
  }

  // Merger plusieurs réponses paginées
  static mergePaginatedResponses<T>(
    responses: PaginatedResponse<T>[]
  ): PaginatedResponse<T> {
    if (responses.length === 0) {
      return {
        items: [],
        pagination: this.createPaginationConfig(1, 10, 0)
      };
    }
    
    const allItems: T[] = [];
    let totalItems = 0;
    
    responses.forEach(response => {
      allItems.push(...response.items);
      totalItems += response.pagination.totalItems;
    });
    
    // On utilise la pagination de la première réponse comme référence
    const firstPagination = responses[0].pagination;
    
    return {
      items: allItems,
      pagination: {
        ...firstPagination,
        totalItems,
        totalPages: this.calculateTotalPages(totalItems, firstPagination.pageSize)
      }
    };
  }

  // Filtrer et paginer un tableau avec des critères
  static filterAndPaginate<T>(
    items: T[],
    filters: Partial<T>,
    page: number,
    pageSize: number
  ): PaginatedResponse<T> {
    // Appliquer les filtres
    const filteredItems = items.filter(item => {
      return Object.entries(filters).every(([key, value]) => {
        if (value === undefined || value === null || value === '') return true;
        
        const itemValue = (item as any)[key];
        if (typeof value === 'string') {
          return itemValue?.toString().toLowerCase().includes(value.toLowerCase());
        }
        
        return itemValue === value;
      });
    });
    
    // Paginer les résultats filtrés
    return this.paginateArray(filteredItems, page, pageSize);
  }

  // Trier et paginer un tableau
  static sortAndPaginate<T>(
    items: T[],
    sortBy: keyof T,
    sortOrder: 'asc' | 'desc',
    page: number,
    pageSize: number
  ): PaginatedResponse<T> {
    const sortedItems = [...items].sort((a, b) => {
      const aValue = a[sortBy];
      const bValue = b[sortBy];
      
      if (aValue === bValue) return 0;
      
      let comparison = 0;
      if (aValue < bValue) comparison = -1;
      if (aValue > bValue) comparison = 1;
      
      return sortOrder === 'desc' ? -comparison : comparison;
    });
    
    return this.paginateArray(sortedItems, page, pageSize);
  }

  // Générer les paramètres de query pour la pagination
  static generateQueryParams(
    page: number,
    pageSize: number,
    additionalParams: Record<string, any> = {}
  ): URLSearchParams {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      ...additionalParams
    });
    
    return params;
  }

  // Parser les paramètres de query pour la pagination
  static parseQueryParams(
    params: URLSearchParams,
    defaultPage: number = 1,
    defaultPageSize: number = 10
  ): { page: number; pageSize: number } {
    const page = parseInt(params.get('page') || defaultPage.toString());
    const pageSize = parseInt(params.get('pageSize') || defaultPageSize.toString());
    
    return this.validateAndFixPaginationParams(page, pageSize);
  }

  // Calculer le pourcentage de progression dans la pagination
  static getProgressPercentage(
    currentPage: number,
    totalPages: number
  ): number {
    if (totalPages <= 1) return 100;
    return Math.round((currentPage / totalPages) * 100);
  }

  // Vérifier si on doit afficher la pagination (trop d'éléments pour une seule page)
  static shouldShowPagination(totalItems: number, pageSize: number): boolean {
    return totalItems > pageSize;
  }

  // Obtenir le nombre d'éléments sur la page actuelle
  static getCurrentPageItemCount<T>(
    paginatedResponse: PaginatedResponse<T>
  ): number {
    return paginatedResponse.items.length;
  }

  // Réinitialiser la pagination à la première page
  static resetToFirstPage<T>(
    paginatedResponse: PaginatedResponse<T>
  ): PaginatedResponse<T> {
    return {
      ...paginatedResponse,
      pagination: {
        ...paginatedResponse.pagination,
        page: 1
      }
    };
  }
}