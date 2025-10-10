import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'current_user';
  private readonly THEME_KEY = 'user_theme';
  private readonly LANGUAGE_KEY = 'user_language';

  // Méthodes génériques pour le stockage (alias pour compatibilité)
  set(key: string, value: any): void {
    this.setItem(key, value);
  }

  get<T>(key: string): T | null {
    return this.getItem<T>(key);
  }

  remove(key: string): void {
    this.removeItem(key);
  }

  // Gestion du token d'authentification
  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  // Gestion des données utilisateur
  setUser(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getUser(): any {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  // Gestion du thème
  setTheme(theme: string): void {
    localStorage.setItem(this.THEME_KEY, theme);
  }

  getTheme(): string | null {
    return localStorage.getItem(this.THEME_KEY);
  }

  // Gestion de la langue
  setLanguage(language: string): void {
    localStorage.setItem(this.LANGUAGE_KEY, language);
  }

  getLanguage(): string | null {
    return localStorage.getItem(this.LANGUAGE_KEY);
  }

  // Méthodes génériques pour le stockage
  setItem(key: string, value: any): void {
    if (typeof value === 'object') {
      localStorage.setItem(key, JSON.stringify(value));
    } else {
      localStorage.setItem(key, value);
    }
  }

  getItem<T>(key: string): T | null {
    const item = localStorage.getItem(key);
    if (!item) return null;

    try {
      return JSON.parse(item) as T;
    } catch {
      return item as T;
    }
  }

  removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  // Nettoyage complet du stockage
  clear(): void {
    localStorage.clear();
  }

  // Nettoyage sélectif (conserve certaines clés)
  clearExcept(keys: string[]): void {
    const itemsToKeep: { [key: string]: string } = {};
    
    keys.forEach(key => {
      const value = localStorage.getItem(key);
      if (value) {
        itemsToKeep[key] = value;
      }
    });

    localStorage.clear();

    Object.keys(itemsToKeep).forEach(key => {
      localStorage.setItem(key, itemsToKeep[key]);
    });
  }

  // Vérification de la disponibilité du localStorage
  isLocalStorageAvailable(): boolean {
    try {
      const test = 'test';
      localStorage.setItem(test, test);
      localStorage.removeItem(test);
      return true;
    } catch (e) {
      return false;
    }
  }

  // Gestion des données de session
  setSessionItem(key: string, value: any): void {
    if (typeof value === 'object') {
      sessionStorage.setItem(key, JSON.stringify(value));
    } else {
      sessionStorage.setItem(key, value);
    }
  }

  getSessionItem<T>(key: string): T | null {
    const item = sessionStorage.getItem(key);
    if (!item) return null;

    try {
      return JSON.parse(item) as T;
    } catch {
      return item as T;
    }
  }

  removeSessionItem(key: string): void {
    sessionStorage.removeItem(key);
  }

  clearSession(): void {
    sessionStorage.clear();
  }
}