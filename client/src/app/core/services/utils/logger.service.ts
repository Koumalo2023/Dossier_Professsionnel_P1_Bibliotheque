import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { EventBusService } from '../event-bus/event-bus.service';
import { StorageService } from '../storage/storage.service';

export enum LogLevel {
  ERROR = 0,
  WARN = 1,
  INFO = 2,
  DEBUG = 3,
  TRACE = 4
}

export interface LogEntry {
  timestamp: Date;
  level: LogLevel;
  message: string;
  context?: string;
  data?: any;
  stackTrace?: string;
  userAgent?: string;
  url?: string;
}

export interface LoggerConfig {
  level: LogLevel;
  maxEntries: number;
  enableConsole: boolean;
  enableStorage: boolean;
  enableRemote: boolean;
  remoteEndpoint?: string;
  enablePerformance: boolean;
  enableUserTracking: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class LoggerService {
  private config!: LoggerConfig;
  private logHistory: LogEntry[] = [];
  private isBrowser: boolean;
  private performanceMarkers: Map<string, number> = new Map();

  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private eventBus: EventBusService,
    private storageService: StorageService
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.initializeConfig();
    this.loadLogHistory();
    this.setupGlobalErrorHandling();
  }

  /**
   * Initialise la configuration du logger
   */
  private initializeConfig(): void {
    const defaultConfig: LoggerConfig = {
      level: this.isBrowser && !environment.production ? LogLevel.DEBUG : LogLevel.WARN,
      maxEntries: 1000,
      enableConsole: true,
      enableStorage: true,
      enableRemote: false,
      enablePerformance: false,
      enableUserTracking: false
    };

    const savedConfig = this.storageService.get<LoggerConfig>('logger-config');
    this.config = { ...defaultConfig, ...savedConfig };
  }

  /**
   * Configure la gestion globale des erreurs
   */
  private setupGlobalErrorHandling(): void {
    if (!this.isBrowser) return;

    // Gestion des erreurs JavaScript non capturées
    window.addEventListener('error', (event) => {
      this.error('Uncaught JavaScript Error', event.error?.message || event.message, {
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno,
        error: event.error
      });
    });

    // Gestion des promesses rejetées non capturées
    window.addEventListener('unhandledrejection', (event) => {
      this.error('Unhandled Promise Rejection', event.reason?.message || 'Unknown reason', {
        reason: event.reason
      });
    });

    // Surveillance des performances
    if (this.config.enablePerformance && 'performance' in window) {
      this.setupPerformanceMonitoring();
    }
  }

  /**
   * Configure la surveillance des performances
   */
  private setupPerformanceMonitoring(): void {
    if (!this.isBrowser) return;

    // Surveillance de la mémoire (si disponible)
    if ('memory' in performance) {
      setInterval(() => {
        const memory = (performance as any).memory;
        this.debug('Memory Usage', 'Performance monitoring', {
          usedJSHeapSize: this.formatBytes(memory.usedJSHeapSize),
          totalJSHeapSize: this.formatBytes(memory.totalJSHeapSize),
          jsHeapSizeLimit: this.formatBytes(memory.jsHeapSizeLimit)
        });
      }, 60000); // Toutes les minutes
    }

    // Surveillance des entrées de performance
    const observer = new PerformanceObserver((list) => {
      list.getEntries().forEach((entry) => {
        this.trace('Performance Entry', entry.name, {
          entryType: entry.entryType,
          duration: entry.duration,
          startTime: entry.startTime
        });
      });
    });

    try {
      observer.observe({ entryTypes: ['navigation', 'resource', 'paint'] });
    } catch (e) {
      this.warn('Performance Observer not supported', String(e));
    }
  }

  // ──────────────────────────────────────────────────
  // 📝 MÉTHODES DE LOGGING PRINCIPALES
  // ──────────────────────────────────────────────────

  /**
   * Log d'erreur critique
   */
  error(message: string, context?: string, data?: any): void {
    this.log(LogLevel.ERROR, message, context, data);
  }

  /**
   * Log d'avertissement
   */
  warn(message: string, context?: string, data?: any): void {
    this.log(LogLevel.WARN, message, context, data);
  }

  /**
   * Log d'information
   */
  info(message: string, context?: string, data?: any): void {
    this.log(LogLevel.INFO, message, context, data);
  }

  /**
   * Log de débogage
   */
  debug(message: string, context?: string, data?: any): void {
    this.log(LogLevel.DEBUG, message, context, data);
  }

  /**
   * Log de trace détaillée
   */
  trace(message: string, context?: string, data?: any): void {
    this.log(LogLevel.TRACE, message, context, data);
  }

  /**
   * Méthode de logging principale
   */
  private log(level: LogLevel, message: string, context?: string, data?: any): void {
    if (level > this.config.level) return;

    const entry: LogEntry = {
      timestamp: new Date(),
      level,
      message,
      context,
      data: this.sanitizeData(data),
      stackTrace: level <= LogLevel.WARN ? this.getStackTrace() : undefined,
      userAgent: this.isBrowser ? navigator.userAgent : undefined,
      url: this.isBrowser ? window.location.href : undefined
    };

    this.addToHistory(entry);
    this.outputToConsole(entry);
    this.emitLogEvent(entry);

    // Envoi vers serveur distant si configuré
    if (this.config.enableRemote && this.config.remoteEndpoint) {
      this.sendToRemote(entry);
    }
  }

  /**
   * Ajoute une entrée à l'historique
   */
  private addToHistory(entry: LogEntry): void {
    this.logHistory.unshift(entry);

    // Limite la taille de l'historique
    if (this.logHistory.length > this.config.maxEntries) {
      this.logHistory = this.logHistory.slice(0, this.config.maxEntries);
    }

    // Sauvegarde dans le stockage si configuré
    if (this.config.enableStorage) {
      this.saveLogHistory();
    }
  }

  /**
   * Affiche le log dans la console
   */
  private outputToConsole(entry: LogEntry): void {
    if (!this.config.enableConsole || !this.isBrowser) return;

    const timestamp = entry.timestamp.toISOString();
    const levelName = LogLevel[entry.level];
    const context = entry.context ? `[${entry.context}]` : '';
    const message = `${timestamp} ${levelName} ${context} ${entry.message}`;

    const styles = this.getConsoleStyles(entry.level);

    switch (entry.level) {
      case LogLevel.ERROR:
        console.error(`%c${message}`, styles, entry.data || '');
        break;
      case LogLevel.WARN:
        console.warn(`%c${message}`, styles, entry.data || '');
        break;
      case LogLevel.INFO:
        console.info(`%c${message}`, styles, entry.data || '');
        break;
      case LogLevel.DEBUG:
        console.debug(`%c${message}`, styles, entry.data || '');
        break;
      case LogLevel.TRACE:
        console.trace(`%c${message}`, styles, entry.data || '');
        break;
    }
  }

  /**
   * Retourne les styles CSS pour la console
   */
  private getConsoleStyles(level: LogLevel): string {
    const baseStyle = 'font-weight: bold; padding: 2px 4px; border-radius: 3px;';

    switch (level) {
      case LogLevel.ERROR:
        return `${baseStyle} background: #ffebee; color: #c62828;`;
      case LogLevel.WARN:
        return `${baseStyle} background: #fff3e0; color: #ef6c00;`;
      case LogLevel.INFO:
        return `${baseStyle} background: #e3f2fd; color: #1565c0;`;
      case LogLevel.DEBUG:
        return `${baseStyle} background: #e8f5e8; color: #2e7d32;`;
      case LogLevel.TRACE:
        return `${baseStyle} background: #f3e5f5; color: #7b1fa2;`;
      default:
        return baseStyle;
    }
  }

  /**
   * Émet un événement pour le log
   */
  private emitLogEvent(entry: LogEntry): void {
    this.eventBus.emit('LOG_ENTRY_CREATED', { entry });
  }

  /**
   * Envoie le log vers un serveur distant
   */
  private sendToRemote(entry: LogEntry): void {
    if (!this.isBrowser) return;

    // Utilisation de sendBeacon pour les envois asynchrones
    const blob = new Blob([JSON.stringify(entry)], { type: 'application/json' });
    
    if (navigator.sendBeacon) {
      navigator.sendBeacon(this.config.remoteEndpoint!, blob);
    } else {
      // Fallback avec fetch
      fetch(this.config.remoteEndpoint!, {
        method: 'POST',
        body: blob,
        keepalive: true
      }).catch(error => {
        this.warn('Failed to send log to remote', 'LoggerService', { error });
      });
    }
  }

  // ──────────────────────────────────────────────────
  // ⚡ MÉTHODES DE PERFORMANCE
  // ──────────────────────────────────────────────────

  /**
   * Démarre un marqueur de performance
   */
  startTimer(marker: string): void {
    if (!this.config.enablePerformance || !this.isBrowser) return;
    
    this.performanceMarkers.set(marker, performance.now());
    this.trace(`Timer started: ${marker}`, 'Performance');
  }

  /**
   * Arrête un marqueur de performance et log le temps écoulé
   */
  stopTimer(marker: string, context?: string, data?: any): number {
    if (!this.config.enablePerformance || !this.isBrowser) return 0;

    const startTime = this.performanceMarkers.get(marker);
    if (!startTime) {
      this.warn(`Timer not found: ${marker}`, 'Performance');
      return 0;
    }

    const endTime = performance.now();
    const duration = endTime - startTime;

    this.performanceMarkers.delete(marker);

    this.info(`Timer stopped: ${marker}`, context || 'Performance', {
      ...data,
      duration: `${duration.toFixed(2)}ms`,
      marker
    });

    return duration;
  }

  /**
   * Mesure le temps d'exécution d'une fonction
   */
  measure<T>(name: string, fn: () => T, context?: string): T {
    this.startTimer(name);
    try {
      const result = fn();
      this.stopTimer(name, context);
      return result;
    } catch (error) {
      this.stopTimer(name, context, { error });
      throw error;
    }
  }

  /**
   * Mesure le temps d'exécution d'une fonction asynchrone
   */
  async measureAsync<T>(name: string, fn: () => Promise<T>, context?: string): Promise<T> {
    this.startTimer(name);
    try {
      const result = await fn();
      this.stopTimer(name, context);
      return result;
    } catch (error) {
      this.stopTimer(name, context, { error });
      throw error;
    }
  }

  // ──────────────────────────────────────────────────
  // 📊 MÉTHODES UTILITAIRES
  // ──────────────────────────────────────────────────

  /**
   * Retourne l'historique des logs
   */
  getLogHistory(): LogEntry[] {
    return [...this.logHistory];
  }

  /**
   * Filtre les logs par niveau
   */
  filterByLevel(level: LogLevel): LogEntry[] {
    return this.logHistory.filter(entry => entry.level === level);
  }

  /**
   * Filtre les logs par contexte
   */
  filterByContext(context: string): LogEntry[] {
    return this.logHistory.filter(entry => entry.context === context);
  }

  /**
   * Recherche dans les logs
   */
  searchLogs(query: string): LogEntry[] {
    const lowerQuery = query.toLowerCase();
    return this.logHistory.filter(entry =>
      entry.message.toLowerCase().includes(lowerQuery) ||
      (entry.context && entry.context.toLowerCase().includes(lowerQuery))
    );
  }

  /**
   * Exporte les logs au format JSON
   */
  exportLogs(): string {
    return JSON.stringify(this.logHistory, null, 2);
  }

  /**
   * Efface l'historique des logs
   */
  clearLogs(): void {
    this.logHistory = [];
    this.storageService.remove('log-history');
    this.eventBus.emit('LOGS_CLEARED');
  }

  /**
   * Met à jour la configuration
   */
  updateConfig(newConfig: Partial<LoggerConfig>): void {
    this.config = { ...this.config, ...newConfig };
    this.storageService.set('logger-config', this.config);
    this.eventBus.emit('LOGGER_CONFIG_UPDATED', { config: this.config });
  }

  /**
   * Retourne la configuration actuelle
   */
  getConfig(): LoggerConfig {
    return { ...this.config };
  }

  /**
   * Réinitialise la configuration par défaut
   */
  resetConfig(): void {
    this.storageService.remove('logger-config');
    this.initializeConfig();
    this.eventBus.emit('LOGGER_CONFIG_RESET', { config: this.config });
  }

  // ──────────────────────────────────────────────────
  // 🔧 MÉTHODES PRIVÉES
  // ──────────────────────────────────────────────────

  /**
   * Nettoie les données pour éviter les références circulaires
   */
  private sanitizeData(data: any): any {
    if (!data) return data;

    try {
      return JSON.parse(JSON.stringify(data, (key, value) => {
        // Évite les données trop volumineuses
        if (typeof value === 'string' && value.length > 1000) {
          return value.substring(0, 1000) + '... [TRUNCATED]';
        }
        return value;
      }));
    } catch (error) {
      return '[Unserializable data]';
    }
  }

  /**
   * Retourne la stack trace (sans les frames du logger)
   */
  private getStackTrace(): string {
    try {
      throw new Error();
    } catch (error) {
      const stack = (error as Error).stack;
      if (!stack) return '';

      // Filtre les frames du logger
      return stack.split('\n')
        .slice(3) // Supprime les frames de getStackTrace et log
        .filter(line => !line.includes('LoggerService'))
        .join('\n');
    }
  }

  /**
   * Charge l'historique depuis le stockage
   */
  private loadLogHistory(): void {
    if (!this.config.enableStorage) return;

    const savedHistory = this.storageService.get<LogEntry[]>('log-history');
    if (savedHistory) {
      // Convertit les timestamps string en Date
      this.logHistory = savedHistory.map(entry => ({
        ...entry,
        timestamp: new Date(entry.timestamp)
      }));
    }
  }

  /**
   * Sauvegarde l'historique dans le stockage
   */
  private saveLogHistory(): void {
    if (!this.config.enableStorage) return;
    this.storageService.set('log-history', this.logHistory);
  }

  /**
   * Formate les octets en unités lisibles
   */
  private formatBytes(bytes: number): string {
    if (bytes === 0) return '0 Bytes';

    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));

    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  /**
   * Vérifie si le logger est actif
   */
  isEnabled(): boolean {
    return this.config.level > LogLevel.ERROR;
  }

  /**
   * Vérifie si un niveau de log est activé
   */
  isLevelEnabled(level: LogLevel): boolean {
    return level <= this.config.level;
  }
}

// Configuration d'environnement
const environment = {
  production: false
};