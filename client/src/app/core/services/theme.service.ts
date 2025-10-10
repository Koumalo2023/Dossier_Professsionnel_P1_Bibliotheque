import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { StorageService } from './storage/storage.service';
import { EventBusService } from './event-bus/event-bus.service';

export type ThemeType = 'light' | 'dark' | 'auto';
export interface CustomTheme {
  id: string;
  name: string;
  colors: {
    primary: string;
    secondary: string;
    accent: string;
    background: string;
    card: string;
    text: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentThemeSubject = new BehaviorSubject<ThemeType>('light');
  private availableThemes: ThemeType[] = ['light', 'dark', 'auto'];
  private customThemes: CustomTheme[] = [];
  private isBrowser: boolean;

  public currentTheme$: Observable<ThemeType> = this.currentThemeSubject.asObservable();

  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private storageService: StorageService,
    private eventBus: EventBusService
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.initializeTheme();
  }

  /**
   * Initialise le thème au démarrage de l'application
   */
  private initializeTheme(): void {
    if (!this.isBrowser) return;

    // Récupère le thème sauvegardé ou utilise 'auto' par défaut
    const savedTheme = this.storageService.get<string>('user-theme') as ThemeType;
    const theme = savedTheme || 'auto';
    
    this.applyTheme(theme, false); // Pas de transition au démarrage
  }

  /**
   * Applique un thème avec transition fluide
   */
  setTheme(theme: ThemeType): void {
    if (!this.availableThemes.includes(theme)) {
      console.warn(`Thème non supporté: ${theme}`);
      return;
    }

    this.applyTheme(theme, true);
    this.saveTheme(theme);
    this.emitThemeChange(theme);
  }

  /**
   * Applique le thème au DOM avec transition optionnelle
   */
  private applyTheme(theme: ThemeType, withTransition: boolean): void {
    if (!this.isBrowser) return;

    const htmlElement = document.documentElement;
    const bodyElement = document.body;

    // Active les transitions CSS si demandé
    if (withTransition) {
      bodyElement.style.transition = 'all 0.3s ease-in-out';
    }

    // Supprime les classes de thème existantes
    this.availableThemes.forEach(t => {
      htmlElement.classList.remove(`${t}-theme`);
      bodyElement.classList.remove(`${t}-theme`);
    });

    // Détermine le thème effectif (pour le mode auto)
    const effectiveTheme = this.getEffectiveTheme(theme);

    // Applique le thème
    htmlElement.classList.add(`${effectiveTheme}-theme`);
    bodyElement.classList.add(`${effectiveTheme}-theme`);
    
    // Met à jour l'attribut data-theme pour les styles CSS
    htmlElement.setAttribute('data-theme', effectiveTheme);

    // Met à jour le sujet
    this.currentThemeSubject.next(theme);

    // Réinitialise la transition après un délai
    if (withTransition) {
      setTimeout(() => {
        bodyElement.style.transition = '';
      }, 300);
    }
  }

  /**
   * Détermine le thème effectif (gestion du mode auto)
   */
  private getEffectiveTheme(theme: ThemeType): 'light' | 'dark' {
    if (theme !== 'auto') {
      return theme;
    }

    // Détection automatique basée sur les préférences système
    if (this.isBrowser && window.matchMedia) {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    return 'light'; // Fallback
  }

  /**
   * Sauvegarde le thème dans le stockage local
   */
  private saveTheme(theme: ThemeType): void {
    this.storageService.set('user-theme', theme);
  }

  /**
   * Émet un événement de changement de thème
   */
  private emitThemeChange(theme: ThemeType): void {
    this.eventBus.emit('THEME_CHANGED', {
      theme,
      effectiveTheme: this.getEffectiveTheme(theme),
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Retourne le thème actuel
   */
  getCurrentTheme(): ThemeType {
    return this.currentThemeSubject.value;
  }

  /**
   * Retourne le thème effectif (résolu)
   */
  getEffectiveCurrentTheme(): 'light' | 'dark' {
    return this.getEffectiveTheme(this.currentThemeSubject.value);
  }

  /**
   * Retourne la liste des thèmes disponibles
   */
  getAvailableThemes(): ThemeType[] {
    return [...this.availableThemes];
  }

  /**
   * Ajoute un thème personnalisé
   */
  addCustomTheme(theme: CustomTheme): void {
    const existingIndex = this.customThemes.findIndex(t => t.id === theme.id);
    
    if (existingIndex >= 0) {
      this.customThemes[existingIndex] = theme;
    } else {
      this.customThemes.push(theme);
    }

    this.saveCustomThemes();
    this.eventBus.emit('CUSTOM_THEME_ADDED', { theme });
  }

  /**
   * Supprime un thème personnalisé
   */
  removeCustomTheme(themeId: string): void {
    this.customThemes = this.customThemes.filter(t => t.id !== themeId);
    this.saveCustomThemes();
    this.eventBus.emit('CUSTOM_THEME_REMOVED', { themeId });
  }

  /**
   * Retourne les thèmes personnalisés
   */
  getCustomThemes(): CustomTheme[] {
    return [...this.customThemes];
  }

  /**
   * Applique un thème personnalisé
   */
  applyCustomTheme(themeId: string): void {
    const theme = this.customThemes.find(t => t.id === themeId);
    if (!theme) {
      console.warn(`Thème personnalisé non trouvé: ${themeId}`);
      return;
    }

    this.applyCustomThemeStyles(theme);
    this.eventBus.emit('CUSTOM_THEME_APPLIED', { theme });
  }

  /**
   * Applique les styles CSS d'un thème personnalisé
   */
  private applyCustomThemeStyles(theme: CustomTheme): void {
    if (!this.isBrowser) return;

    const styleElement = document.getElementById('custom-theme-styles') || 
      this.createCustomThemeStyleElement();

    const css = `
      .custom-theme-${theme.id} {
        --color-primary: ${theme.colors.primary};
        --color-secondary: ${theme.colors.secondary};
        --color-accent: ${theme.colors.accent};
        --color-background: ${theme.colors.background};
        --color-card-bg: ${theme.colors.card};
        --color-text-primary: ${theme.colors.text};
      }
    `;

    styleElement.textContent = css;
    
    // Applique la classe du thème personnalisé
    document.documentElement.classList.add(`custom-theme-${theme.id}`);
    document.body.classList.add(`custom-theme-${theme.id}`);
  }

  /**
   * Crée l'élément style pour les thèmes personnalisés
   */
  private createCustomThemeStyleElement(): HTMLStyleElement {
    const styleElement = document.createElement('style');
    styleElement.id = 'custom-theme-styles';
    document.head.appendChild(styleElement);
    return styleElement;
  }

  /**
   * Supprime les thèmes personnalisés appliqués
   */
  removeCustomThemeStyles(): void {
    if (!this.isBrowser) return;

    const styleElement = document.getElementById('custom-theme-styles');
    if (styleElement) {
      styleElement.remove();
    }

    // Supprime toutes les classes de thème personnalisé
    const classes = Array.from(document.documentElement.classList);
    classes.forEach(className => {
      if (className.startsWith('custom-theme-')) {
        document.documentElement.classList.remove(className);
        document.body.classList.remove(className);
      }
    });
  }

  /**
   * Sauvegarde les thèmes personnalisés
   */
  private saveCustomThemes(): void {
    this.storageService.set('custom-themes', this.customThemes);
  }

  /**
   * Charge les thèmes personnalisés sauvegardés
   */
  loadCustomThemes(): void {
    const savedThemes = this.storageService.get<CustomTheme[]>('custom-themes');
    if (savedThemes) {
      this.customThemes = savedThemes;
    }
  }

  /**
   * Écoute les changements de préférence système (pour le mode auto)
   */
  watchSystemPreference(): void {
    if (!this.isBrowser || !window.matchMedia) return;

    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    
    const handleChange = (e: MediaQueryListEvent) => {
      if (this.getCurrentTheme() === 'auto') {
        this.applyTheme('auto', true);
      }
    };

    // Support ancien et nouveau navigateurs
    if (mediaQuery.addEventListener) {
      mediaQuery.addEventListener('change', handleChange);
    } else {
      mediaQuery.addListener(handleChange);
    }
  }

  /**
   * Cycle entre les thèmes disponibles
   */
  cycleThemes(): ThemeType {
    const currentIndex = this.availableThemes.indexOf(this.currentThemeSubject.value);
    const nextIndex = (currentIndex + 1) % this.availableThemes.length;
    const nextTheme = this.availableThemes[nextIndex];
    
    this.setTheme(nextTheme);
    return nextTheme;
  }

  /**
   * Vérifie si le thème actuel est sombre
   */
  isDarkTheme(): boolean {
    return this.getEffectiveCurrentTheme() === 'dark';
  }

  /**
   * Vérifie si le thème actuel est clair
   */
  isLightTheme(): boolean {
    return this.getEffectiveCurrentTheme() === 'light';
  }

  /**
   * Réinitialise vers le thème par défaut
   */
  resetToDefault(): void {
    this.setTheme('auto');
  }
}