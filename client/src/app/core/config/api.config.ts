// Configuration API

export interface ApiConfig {
  baseUrl: string;
  endpoints: {
    auth: {
      login: string;
      register: string;
      refresh: string;
      logout: string;
      forgotPassword: string;
      resetPassword: string;
    };
    books: {
      base: string;
      search: string;
      byId: string;
      create: string;
      update: string;
      delete: string;
      import: string;
      categories: string;
      popular: string;
      recent: string;
    };
    loans: {
      base: string;
      userLoans: string;
      borrow: string;
      return: string;
      renew: string;
      overdue: string;
      history: string;
    };
    reservations: {
      base: string;
      userReservations: string;
      create: string;
      cancel: string;
      available: string;
    };
    notifications: {
      base: string;
      userNotifications: string;
      markRead: string;
      markAllRead: string;
      settings: string;
    };
    analytics: {
      base: string;
      dashboard: string;
      books: string;
      users: string;
      loans: string;
      popularBooks: string;
      readingTrends: string;
    };
    users: {
      base: string;
      profile: string;
      updateProfile: string;
      changePassword: string;
      preferences: string;
      readingGoals: string;
      readingHistory: string;
    };
    preferences: {
      base: string;
      categories: string;
      notifications: string;
      privacy: string;
    };
  };
  timeout: number;
  retryAttempts: number;
  retryDelay: number;
}

// Configuration par défaut
export const apiConfig: ApiConfig = {
  baseUrl: '/api',
  endpoints: {
    auth: {
      login: '/auth/login',
      register: '/auth/register',
      refresh: '/auth/refresh',
      logout: '/auth/logout',
      forgotPassword: '/auth/forgot-password',
      resetPassword: '/auth/reset-password'
    },
    books: {
      base: '/books',
      search: '/books/search',
      byId: '/books',
      create: '/books',
      update: '/books',
      delete: '/books',
      import: '/books/import',
      categories: '/books/categories',
      popular: '/books/popular',
      recent: '/books/recent'
    },
    loans: {
      base: '/loans',
      userLoans: '/loans/user',
      borrow: '/loans/borrow',
      return: '/loans/return',
      renew: '/loans/renew',
      overdue: '/loans/overdue',
      history: '/loans/history'
    },
    reservations: {
      base: '/reservations',
      userReservations: '/reservations/user',
      create: '/reservations',
      cancel: '/reservations/cancel',
      available: '/reservations/available'
    },
    notifications: {
      base: '/notifications',
      userNotifications: '/notifications/user',
      markRead: '/notifications/mark-read',
      markAllRead: '/notifications/mark-all-read',
      settings: '/notifications/settings'
    },
    analytics: {
      base: '/analytics',
      dashboard: '/analytics/dashboard',
      books: '/analytics/books',
      users: '/analytics/users',
      loans: '/analytics/loans',
      popularBooks: '/analytics/popular-books',
      readingTrends: '/analytics/reading-trends'
    },
    users: {
      base: '/users',
      profile: '/users/profile',
      updateProfile: '/users/profile',
      changePassword: '/users/change-password',
      preferences: '/users/preferences',
      readingGoals: '/users/reading-goals',
      readingHistory: '/users/reading-history'
    },
    preferences: {
      base: '/preferences',
      categories: '/preferences/categories',
      notifications: '/preferences/notifications',
      privacy: '/preferences/privacy'
    }
  },
  timeout: 30000, // 30 secondes
  retryAttempts: 3,
  retryDelay: 1000 // 1 seconde
};

// Configuration pour l'environnement de développement
export const devApiConfig: Partial<ApiConfig> = {
  baseUrl: '/api',
  timeout: 30000
};

// Configuration pour l'environnement de production
export const prodApiConfig: Partial<ApiConfig> = {
  baseUrl: '/api',
  timeout: 15000
};

// Configuration pour l'environnement de staging
export const stagingApiConfig: Partial<ApiConfig> = {
  baseUrl: '/api',
  timeout: 20000
};

// Fonction pour obtenir la configuration API en fonction de l'environnement
export function getApiConfig(): ApiConfig {
  // Utiliser les environnements Angular au lieu de process.env
  const environment = (window as any).environment || 'development';
  
  let environmentConfig: Partial<ApiConfig> = {};
  
  switch (environment) {
    case 'production':
      environmentConfig = prodApiConfig;
      break;
    case 'staging':
      environmentConfig = stagingApiConfig;
      break;
    default:
      environmentConfig = devApiConfig;
  }
  
  // Fusionner la configuration de base avec la configuration d'environnement
  return {
    ...apiConfig,
    ...environmentConfig
  };
}

import { Injectable } from '@angular/core';

// Service de configuration API
@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private config: ApiConfig;

  constructor() {
    this.config = getApiConfig();
  }

  getConfig(): ApiConfig {
    return this.config;
  }

  getBaseUrl(): string {
    return this.config.baseUrl;
  }

  getTimeout(): number {
    return this.config.timeout;
  }

  getRetryAttempts(): number {
    return this.config.retryAttempts;
  }

  getRetryDelay(): number {
    return this.config.retryDelay;
  }

  // Méthodes pour construire des URLs complètes
  buildUrl(endpoint: string): string {
    return `${this.config.baseUrl}${endpoint}`;
  }

  buildAuthUrl(endpoint: keyof ApiConfig['endpoints']['auth']): string {
    return this.buildUrl(this.config.endpoints.auth[endpoint]);
  }

  buildBooksUrl(endpoint: keyof ApiConfig['endpoints']['books']): string {
    return this.buildUrl(this.config.endpoints.books[endpoint]);
  }

  buildLoansUrl(endpoint: keyof ApiConfig['endpoints']['loans']): string {
    return this.buildUrl(this.config.endpoints.loans[endpoint]);
  }

  buildReservationsUrl(endpoint: keyof ApiConfig['endpoints']['reservations']): string {
    return this.buildUrl(this.config.endpoints.reservations[endpoint]);
  }

  buildNotificationsUrl(endpoint: keyof ApiConfig['endpoints']['notifications']): string {
    return this.buildUrl(this.config.endpoints.notifications[endpoint]);
  }

  buildAnalyticsUrl(endpoint: keyof ApiConfig['endpoints']['analytics']): string {
    return this.buildUrl(this.config.endpoints.analytics[endpoint]);
  }

  buildUsersUrl(endpoint: keyof ApiConfig['endpoints']['users']): string {
    return this.buildUrl(this.config.endpoints.users[endpoint]);
  }

  buildPreferencesUrl(endpoint: keyof ApiConfig['endpoints']['preferences']): string {
    return this.buildUrl(this.config.endpoints.preferences[endpoint]);
  }

  // Méthodes pour construire des URLs avec des paramètres
  buildBookByIdUrl(bookId: string | number): string {
    return `${this.buildBooksUrl('byId')}/${bookId}`;
  }

  buildLoanByIdUrl(loanId: string | number): string {
    return `${this.buildLoansUrl('base')}/${loanId}`;
  }

  buildReservationByIdUrl(reservationId: string | number): string {
    return `${this.buildReservationsUrl('base')}/${reservationId}`;
  }

  buildNotificationByIdUrl(notificationId: string | number): string {
    return `${this.buildNotificationsUrl('base')}/${notificationId}`;
  }

  buildUserByIdUrl(userId: string | number): string {
    return `${this.buildUsersUrl('base')}/${userId}`;
  }

  // Méthodes pour construire des URLs avec des query parameters
  buildSearchUrl(query: string, filters?: Record<string, any>): string {
    const searchParams = new URLSearchParams();
    searchParams.set('query', query);
    
    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          searchParams.set(key, value.toString());
        }
      });
    }
    
    return `${this.buildBooksUrl('search')}?${searchParams.toString()}`;
  }

  buildBooksWithPaginationUrl(page: number, pageSize: number, filters?: Record<string, any>): string {
    const searchParams = new URLSearchParams();
    searchParams.set('page', page.toString());
    searchParams.set('pageSize', pageSize.toString());
    
    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          searchParams.set(key, value.toString());
        }
      });
    }
    
    return `${this.buildBooksUrl('base')}?${searchParams.toString()}`;
  }
}

// Types utilitaires pour les endpoints
export type AuthEndpoint = keyof ApiConfig['endpoints']['auth'];
export type BooksEndpoint = keyof ApiConfig['endpoints']['books'];
export type LoansEndpoint = keyof ApiConfig['endpoints']['loans'];
export type ReservationsEndpoint = keyof ApiConfig['endpoints']['reservations'];
export type NotificationsEndpoint = keyof ApiConfig['endpoints']['notifications'];
export type AnalyticsEndpoint = keyof ApiConfig['endpoints']['analytics'];
export type UsersEndpoint = keyof ApiConfig['endpoints']['users'];
export type PreferencesEndpoint = keyof ApiConfig['endpoints']['preferences'];