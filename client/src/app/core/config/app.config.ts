// Configuration de l'application

export interface AppConfig {
  app: {
    name: string;
    version: string;
    environment: 'development' | 'staging' | 'production';
    defaultLanguage: string;
    supportedLanguages: string[];
    defaultPageSize: number;
    maxPageSize: number;
    enableDebug: boolean;
    enableAnalytics: boolean;
  };
  features: {
    enableNotifications: boolean;
    enableReservations: boolean;
    enableAnalytics: boolean;
    enableUserPreferences: boolean;
    enableReadingGoals: boolean;
    enableDarkMode: boolean;
    enableOfflineMode: boolean;
  };
  ui: {
    defaultTheme: 'light' | 'dark' | 'auto';
    animations: {
      enable: boolean;
      duration: number;
    };
    toast: {
      duration: number;
      position: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
    };
    pagination: {
      defaultPageSize: number;
      pageSizeOptions: number[];
    };
  };
  security: {
    tokenRefreshInterval: number; // en minutes
    sessionTimeout: number; // en minutes
    maxLoginAttempts: number;
    passwordPolicy: {
      minLength: number;
      requireUppercase: boolean;
      requireLowercase: boolean;
      requireNumbers: boolean;
      requireSpecialChars: boolean;
    };
  };
  storage: {
    prefix: string;
    encryptionKey: string;
    defaultStorage: 'localStorage' | 'sessionStorage';
  };
}

export const appConfig: AppConfig = {
  app: {
    name: 'Bibliothèque Numérique',
    version: '1.0.0',
    environment: 'development',
    defaultLanguage: 'fr',
    supportedLanguages: ['fr', 'en'],
    defaultPageSize: 10,
    maxPageSize: 100,
    enableDebug: true,
    enableAnalytics: true
  },
  features: {
    enableNotifications: true,
    enableReservations: true,
    enableAnalytics: true,
    enableUserPreferences: true,
    enableReadingGoals: true,
    enableDarkMode: true,
    enableOfflineMode: false
  },
  ui: {
    defaultTheme: 'light',
    animations: {
      enable: true,
      duration: 300
    },
    toast: {
      duration: 5000,
      position: 'top-right'
    },
    pagination: {
      defaultPageSize: 10,
      pageSizeOptions: [5, 10, 25, 50, 100]
    }
  },
  security: {
    tokenRefreshInterval: 15, // 15 minutes
    sessionTimeout: 60, // 1 heure
    maxLoginAttempts: 5,
    passwordPolicy: {
      minLength: 8,
      requireUppercase: true,
      requireLowercase: true,
      requireNumbers: true,
      requireSpecialChars: true
    }
  },
  storage: {
    prefix: 'bibliotheque_',
    encryptionKey: 'bibliotheque_app_key',
    defaultStorage: 'localStorage'
  }
};

// Configuration pour l'environnement de développement
export const devConfig: Partial<AppConfig> = {
  app: {
    ...appConfig.app,
    environment: 'development',
    enableDebug: true
  }
};

// Configuration pour l'environnement de production
export const prodConfig: Partial<AppConfig> = {
  app: {
    ...appConfig.app,
    environment: 'production',
    enableDebug: false
  },
  features: {
    ...appConfig.features,
    enableOfflineMode: true
  }
};

// Configuration pour l'environnement de staging
export const stagingConfig: Partial<AppConfig> = {
  app: {
    ...appConfig.app,
    environment: 'staging',
    enableDebug: false
  }
};

// Fonction pour obtenir la configuration en fonction de l'environnement
export function getAppConfig(): AppConfig {
  // Utiliser les environnements Angular au lieu de process.env
  const environment = (window as any).environment || 'development';
  
  let environmentConfig: Partial<AppConfig> = {};
  
  switch (environment) {
    case 'production':
      environmentConfig = prodConfig;
      break;
    case 'staging':
      environmentConfig = stagingConfig;
      break;
    default:
      environmentConfig = devConfig;
  }
  
  // Fusionner la configuration de base avec la configuration d'environnement
  return {
    ...appConfig,
    ...environmentConfig
  };
}

// Types utilitaires pour la configuration
export type AppEnvironment = AppConfig['app']['environment'];
export type AppTheme = AppConfig['ui']['defaultTheme'];
export type ToastPosition = AppConfig['ui']['toast']['position'];

// Service de configuration
export class ConfigService {
  private static instance: ConfigService;
  private config: AppConfig;

  private constructor() {
    this.config = getAppConfig();
  }

  static getInstance(): ConfigService {
    if (!ConfigService.instance) {
      ConfigService.instance = new ConfigService();
    }
    return ConfigService.instance;
  }

  getConfig(): AppConfig {
    return this.config;
  }

  getAppConfig() {
    return this.config.app;
  }

  getFeaturesConfig() {
    return this.config.features;
  }

  getUiConfig() {
    return this.config.ui;
  }

  getSecurityConfig() {
    return this.config.security;
  }

  getStorageConfig() {
    return this.config.storage;
  }

  isDevelopment(): boolean {
    return this.config.app.environment === 'development';
  }

  isProduction(): boolean {
    return this.config.app.environment === 'production';
  }

  isStaging(): boolean {
    return this.config.app.environment === 'staging';
  }

  // Méthodes pour vérifier l'activation des fonctionnalités
  isFeatureEnabled(feature: keyof AppConfig['features']): boolean {
    return this.config.features[feature];
  }

  // Méthodes pour obtenir des valeurs spécifiques
  getDefaultPageSize(): number {
    return this.config.ui.pagination.defaultPageSize;
  }

  getPageSizeOptions(): number[] {
    return this.config.ui.pagination.pageSizeOptions;
  }

  getDefaultLanguage(): string {
    return this.config.app.defaultLanguage;
  }

  getSupportedLanguages(): string[] {
    return this.config.app.supportedLanguages;
  }
}