// Configuration des thèmes

export interface ThemeColors {
  primary: {
    main: string;
    light: string;
    dark: string;
    contrast: string;
  };
  secondary: {
    main: string;
    light: string;
    dark: string;
    contrast: string;
  };
  accent: {
    main: string;
    light: string;
    dark: string;
    contrast: string;
  };
  background: {
    default: string;
    paper: string;
    card: string;
    sidebar: string;
    header: string;
  };
  text: {
    primary: string;
    secondary: string;
    disabled: string;
    hint: string;
    inverse: string;
  };
  status: {
    success: string;
    warning: string;
    error: string;
    info: string;
  };
  border: {
    light: string;
    main: string;
    dark: string;
  };
  common: {
    white: string;
    black: string;
    gray: {
      50: string;
      100: string;
      200: string;
      300: string;
      400: string;
      500: string;
      600: string;
      700: string;
      800: string;
      900: string;
    };
  };
}

export interface ThemeTypography {
  fontFamily: {
    primary: string;
    secondary: string;
    monospace: string;
  };
  fontSize: {
    xs: string;
    sm: string;
    base: string;
    lg: string;
    xl: string;
    '2xl': string;
    '3xl': string;
    '4xl': string;
  };
  fontWeight: {
    light: number;
    normal: number;
    medium: number;
    semibold: number;
    bold: number;
  };
  lineHeight: {
    tight: number;
    normal: number;
    relaxed: number;
  };
}

export interface ThemeSpacing {
  xs: string;
  sm: string;
  md: string;
  lg: string;
  xl: string;
  '2xl': string;
  '3xl': string;
}

export interface ThemeShadows {
  sm: string;
  md: string;
  lg: string;
  xl: string;
  '2xl': string;
  inner: string;
  none: string;
}

export interface ThemeBreakpoints {
  xs: string;
  sm: string;
  md: string;
  lg: string;
  xl: string;
  '2xl': string;
}

export interface ThemeConfig {
  name: string;
  type: 'light' | 'dark';
  colors: ThemeColors;
  typography: ThemeTypography;
  spacing: ThemeSpacing;
  shadows: ThemeShadows;
  breakpoints: ThemeBreakpoints;
  borderRadius: {
    none: string;
    sm: string;
    md: string;
    lg: string;
    xl: string;
    full: string;
  };
  zIndex: {
    dropdown: number;
    sticky: number;
    fixed: number;
    modal: number;
    popover: number;
    tooltip: number;
    toast: number;
  };
}

// Thème clair
export const lightTheme: ThemeConfig = {
  name: 'Light',
  type: 'light',
  colors: {
    primary: {
      main: '#3B82F6', // Blue-500
      light: '#60A5FA', // Blue-400
      dark: '#2563EB', // Blue-600
      contrast: '#FFFFFF'
    },
    secondary: {
      main: '#6B7280', // Gray-500
      light: '#9CA3AF', // Gray-400
      dark: '#4B5563', // Gray-600
      contrast: '#FFFFFF'
    },
    accent: {
      main: '#8B5CF6', // Violet-500
      light: '#A78BFA', // Violet-400
      dark: '#7C3AED', // Violet-600
      contrast: '#FFFFFF'
    },
    background: {
      default: '#F9FAFB', // Gray-50
      paper: '#FFFFFF',
      card: '#FFFFFF',
      sidebar: '#F3F4F6', // Gray-100
      header: '#FFFFFF'
    },
    text: {
      primary: '#111827', // Gray-900
      secondary: '#6B7280', // Gray-500
      disabled: '#9CA3AF', // Gray-400
      hint: '#6B7280', // Gray-500
      inverse: '#FFFFFF'
    },
    status: {
      success: '#10B981', // Emerald-500
      warning: '#F59E0B', // Amber-500
      error: '#EF4444', // Red-500
      info: '#3B82F6' // Blue-500
    },
    border: {
      light: '#E5E7EB', // Gray-200
      main: '#D1D5DB', // Gray-300
      dark: '#9CA3AF' // Gray-400
    },
    common: {
      white: '#FFFFFF',
      black: '#000000',
      gray: {
        50: '#F9FAFB',
        100: '#F3F4F6',
        200: '#E5E7EB',
        300: '#D1D5DB',
        400: '#9CA3AF',
        500: '#6B7280',
        600: '#4B5563',
        700: '#374151',
        800: '#1F2937',
        900: '#111827'
      }
    }
  },
  typography: {
    fontFamily: {
      primary: "'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif",
      secondary: "'Georgia', 'Times New Roman', serif",
      monospace: "'Fira Code', 'Courier New', monospace"
    },
    fontSize: {
      xs: '0.75rem',    // 12px
      sm: '0.875rem',   // 14px
      base: '1rem',     // 16px
      lg: '1.125rem',   // 18px
      xl: '1.25rem',    // 20px
      '2xl': '1.5rem',  // 24px
      '3xl': '1.875rem', // 30px
      '4xl': '2.25rem'  // 36px
    },
    fontWeight: {
      light: 300,
      normal: 400,
      medium: 500,
      semibold: 600,
      bold: 700
    },
    lineHeight: {
      tight: 1.25,
      normal: 1.5,
      relaxed: 1.75
    }
  },
  spacing: {
    xs: '0.25rem',   // 4px
    sm: '0.5rem',    // 8px
    md: '1rem',      // 16px
    lg: '1.5rem',    // 24px
    xl: '2rem',      // 32px
    '2xl': '3rem',   // 48px
    '3xl': '4rem'    // 64px
  },
  shadows: {
    sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
    md: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
    lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
    xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
    '2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
    inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.06)',
    none: 'none'
  },
  breakpoints: {
    xs: '0px',
    sm: '640px',
    md: '768px',
    lg: '1024px',
    xl: '1280px',
    '2xl': '1536px'
  },
  borderRadius: {
    none: '0',
    sm: '0.125rem', // 2px
    md: '0.375rem', // 6px
    lg: '0.5rem',   // 8px
    xl: '0.75rem',  // 12px
    full: '9999px'
  },
  zIndex: {
    dropdown: 1000,
    sticky: 1020,
    fixed: 1030,
    modal: 1040,
    popover: 1050,
    tooltip: 1060,
    toast: 1070
  }
};

// Thème sombre
export const darkTheme: ThemeConfig = {
  name: 'Dark',
  type: 'dark',
  colors: {
    primary: {
      main: '#60A5FA', // Blue-400
      light: '#93C5FD', // Blue-300
      dark: '#3B82F6', // Blue-500
      contrast: '#FFFFFF'
    },
    secondary: {
      main: '#9CA3AF', // Gray-400
      light: '#D1D5DB', // Gray-300
      dark: '#6B7280', // Gray-500
      contrast: '#FFFFFF'
    },
    accent: {
      main: '#A78BFA', // Violet-400
      light: '#C4B5FD', // Violet-300
      dark: '#8B5CF6', // Violet-500
      contrast: '#FFFFFF'
    },
    background: {
      default: '#111827', // Gray-900
      paper: '#1F2937',   // Gray-800
      card: '#374151',    // Gray-700
      sidebar: '#1F2937', // Gray-800
      header: '#1F2937'   // Gray-800
    },
    text: {
      primary: '#F9FAFB', // Gray-50
      secondary: '#D1D5DB', // Gray-300
      disabled: '#6B7280', // Gray-500
      hint: '#9CA3AF',     // Gray-400
      inverse: '#111827'   // Gray-900
    },
    status: {
      success: '#34D399', // Emerald-400
      warning: '#FBBF24', // Amber-400
      error: '#F87171',   // Red-400
      info: '#60A5FA'     // Blue-400
    },
    border: {
      light: '#374151', // Gray-700
      main: '#4B5563',  // Gray-600
      dark: '#6B7280'   // Gray-500
    },
    common: {
      white: '#FFFFFF',
      black: '#000000',
      gray: {
        50: '#F9FAFB',
        100: '#F3F4F6',
        200: '#E5E7EB',
        300: '#D1D5DB',
        400: '#9CA3AF',
        500: '#6B7280',
        600: '#4B5563',
        700: '#374151',
        800: '#1F2937',
        900: '#111827'
      }
    }
  },
  typography: {
    ...lightTheme.typography
  },
  spacing: {
    ...lightTheme.spacing
  },
  shadows: {
    sm: '0 1px 2px 0 rgba(0, 0, 0, 0.3)',
    md: '0 4px 6px -1px rgba(0, 0, 0, 0.4), 0 2px 4px -1px rgba(0, 0, 0, 0.2)',
    lg: '0 10px 15px -3px rgba(0, 0, 0, 0.4), 0 4px 6px -2px rgba(0, 0, 0, 0.2)',
    xl: '0 20px 25px -5px rgba(0, 0, 0, 0.4), 0 10px 10px -5px rgba(0, 0, 0, 0.2)',
    '2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.5)',
    inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.2)',
    none: 'none'
  },
  breakpoints: {
    ...lightTheme.breakpoints
  },
  borderRadius: {
    ...lightTheme.borderRadius
  },
  zIndex: {
    ...lightTheme.zIndex
  }
};

// Configuration des thèmes disponibles
export const availableThemes = {
  light: lightTheme,
  dark: darkTheme
};

export type ThemeName = keyof typeof availableThemes;

// Service de gestion des thèmes
export class ThemeService {
  private static instance: ThemeService;
  private currentTheme: ThemeName = 'light';
  private themeChangeCallbacks: Array<(theme: ThemeName) => void> = [];

  private constructor() {
    // Récupérer le thème sauvegardé ou détecter la préférence système
    this.loadSavedTheme();
  }

  static getInstance(): ThemeService {
    if (!ThemeService.instance) {
      ThemeService.instance = new ThemeService();
    }
    return ThemeService.instance;
  }

  // Charger le thème sauvegardé
  private loadSavedTheme(): void {
    const savedTheme = localStorage.getItem('bibliotheque_theme') as ThemeName;
    const systemPreference = this.getSystemThemePreference();
    
    if (savedTheme && availableThemes[savedTheme]) {
      this.currentTheme = savedTheme;
    } else {
      this.currentTheme = systemPreference;
    }
    
    this.applyTheme(this.currentTheme);
  }

  // Obtenir la préférence de thème du système
  private getSystemThemePreference(): ThemeName {
    if (typeof window !== 'undefined' && window.matchMedia) {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    return 'light';
  }

  // Appliquer un thème
  private applyTheme(themeName: ThemeName): void {
    const theme = availableThemes[themeName];
    
    // Appliquer les variables CSS
    if (typeof document !== 'undefined') {
      const root = document.documentElement;
      
      // Couleurs
      Object.entries(theme.colors).forEach(([category, colors]) => {
        if (typeof colors === 'object' && colors !== null) {
          Object.entries(colors).forEach(([key, value]) => {
            if (typeof value === 'string') {
              root.style.setProperty(`--color-${category}-${key}`, value);
            } else if (typeof value === 'object' && value !== null) {
              Object.entries(value).forEach(([shade, shadeValue]) => {
                root.style.setProperty(`--color-${category}-${key}-${shade}`, shadeValue as string);
              });
            }
          });
        }
      });
      
      // Typographie
      Object.entries(theme.typography).forEach(([category, values]) => {
        if (typeof values === 'object') {
          Object.entries(values).forEach(([key, value]) => {
            root.style.setProperty(`--font-${category}-${key}`, value as string);
          });
        }
      });
      
      // Espacements
      Object.entries(theme.spacing).forEach(([key, value]) => {
        root.style.setProperty(`--spacing-${key}`, value);
      });
      
      // Bordures
      Object.entries(theme.borderRadius).forEach(([key, value]) => {
        root.style.setProperty(`--radius-${key}`, value);
      });
      
      // Appliquer la classe de thème
      root.classList.remove('theme-light', 'theme-dark');
      root.classList.add(`theme-${themeName}`);
    }
    
    // Sauvegarder la préférence
    localStorage.setItem('bibliotheque_theme', themeName);
  }

  // Obtenir le thème actuel
  getCurrentTheme(): ThemeName {
    return this.currentTheme;
  }

  // Obtenir la configuration du thème actuel
  getCurrentThemeConfig(): ThemeConfig {
    return availableThemes[this.currentTheme];
  }

  // Changer de thème
  setTheme(themeName: ThemeName): void {
    if (!availableThemes[themeName]) {
      console.warn(`Thème "${themeName}" non disponible`);
      return;
    }
    
    this.currentTheme = themeName;
    this.applyTheme(themeName);
    
    // Notifier les callbacks
    this.themeChangeCallbacks.forEach(callback => callback(themeName));
  }

  // Basculer entre les thèmes
  toggleTheme(): void {
    const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.setTheme(newTheme);
  }

  // S'abonner aux changements de thème
  onThemeChange(callback: (theme: ThemeName) => void): () => void {
    this.themeChangeCallbacks.push(callback);
    
    // Retourner une fonction de désabonnement
    return () => {
      const index = this.themeChangeCallbacks.indexOf(callback);
      if (index > -1) {
        this.themeChangeCallbacks.splice(index, 1);
      }
    };
  }

  // Vérifier si le thème est sombre
  isDarkTheme(): boolean {
    return this.currentTheme === 'dark';
  }

  // Vérifier si le thème est clair
  isLightTheme(): boolean {
    return this.currentTheme === 'light';
  }

  // Obtenir la liste des thèmes disponibles
  getAvailableThemes(): ThemeName[] {
    return Object.keys(availableThemes) as ThemeName[];
  }

  // Obtenir la configuration d'un thème spécifique
  getThemeConfig(themeName: ThemeName): ThemeConfig | null {
    return availableThemes[themeName] || null;
  }
}