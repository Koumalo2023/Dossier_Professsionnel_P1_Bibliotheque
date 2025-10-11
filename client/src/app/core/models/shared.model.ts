// Modèles partagés

// Pagination
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

export interface ServiceResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

// Filtres de recherche
export interface BookSearchFilters {
  category?: string;
  author?: string;
  availableOnly?: boolean;
  page?: number;
  pageSize?: number;
}

export interface LoanSearchFilters {
  status?: string;
  userId?: string;
  overdueOnly?: boolean;
  page?: number;
  pageSize?: number;
}

export interface ReservationSearchFilters {
  status?: string;
  userId?: string;
  page?: number;
  pageSize?: number;
}

export interface NotificationSearchFilters {
  isRead?: boolean;
  type?: string;
  page?: number;
  pageSize?: number;
}