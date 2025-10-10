import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { EventBusService } from '../event-bus/event-bus.service';
import { StorageService } from '../storage/storage.service';

export interface ToastNotification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
  action?: {
    label: string;
    callback: () => void;
  };
  timestamp: Date;
}

export interface SnackbarNotification {
  id: string;
  message: string;
  action?: {
    label: string;
    callback: () => void;
  };
  duration?: number;
  type?: 'default' | 'success' | 'error';
}

export interface DialogConfig {
  id: string;
  title: string;
  message: string;
  type?: 'alert' | 'confirm' | 'prompt';
  confirmText?: string;
  cancelText?: string;
  inputPlaceholder?: string;
  inputValue?: string;
  onConfirm?: (inputValue?: string) => void;
  onCancel?: () => void;
  showCloseButton?: boolean;
}

export interface NotificationSettings {
  toastDuration: number;
  snackbarDuration: number;
  maxToasts: number;
  position: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
  soundEnabled: boolean;
  vibrationEnabled: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UINotificationService {
  private toastNotifications = new BehaviorSubject<ToastNotification[]>([]);
  private snackbarNotifications = new BehaviorSubject<SnackbarNotification | null>(null);
  private activeDialog = new BehaviorSubject<DialogConfig | null>(null);
  private settings!: NotificationSettings;
  private isBrowser: boolean;

  public toastNotifications$: Observable<ToastNotification[]> = this.toastNotifications.asObservable();
  public snackbarNotifications$: Observable<SnackbarNotification | null> = this.snackbarNotifications.asObservable();
  public activeDialog$: Observable<DialogConfig | null> = this.activeDialog.asObservable();

  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private eventBus: EventBusService,
    private storageService: StorageService
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.initializeSettings();
    this.setupEventListeners();
  }

  /**
   * Initialise les paramètres de notification
   */
  private initializeSettings(): void {
    const defaultSettings: NotificationSettings = {
      toastDuration: 5000,
      snackbarDuration: 4000,
      maxToasts: 5,
      position: 'top-right',
      soundEnabled: false,
      vibrationEnabled: false
    };

    const savedSettings = this.storageService.get<NotificationSettings>('notification-settings');
    this.settings = { ...defaultSettings, ...savedSettings };
  }

  /**
   * Configure les écouteurs d'événements
   */
  private setupEventListeners(): void {
    // Écoute les événements de notification du système
    this.eventBus.onEvent('SHOW_TOAST').subscribe((event) => {
      const data = event.payload;
      this.showToast(data.type, data.title, data.message, data.duration, data.action);
    });

    this.eventBus.onEvent('SHOW_SNACKBAR').subscribe((event) => {
      const data = event.payload;
      this.showSnackbar(data.message, data.action, data.duration, data.type);
    });

    this.eventBus.onEvent('SHOW_DIALOG').subscribe((event) => {
      const data = event.payload;
      this.showDialog(data);
    });
  }

  // ──────────────────────────────────────────────────
  // 🍞 TOAST NOTIFICATIONS
  // ──────────────────────────────────────────────────

  /**
   * Affiche une notification toast
   */
  showToast(
    type: ToastNotification['type'],
    title: string,
    message: string,
    duration: number = this.settings.toastDuration,
    action?: ToastNotification['action']
  ): string {
    const id = this.generateId();
    const toast: ToastNotification = {
      id,
      type,
      title,
      message,
      duration,
      action,
      timestamp: new Date()
    };

    const currentToasts = this.toastNotifications.value;
    const updatedToasts = [...currentToasts, toast];

    // Limite le nombre de toasts affichés
    if (updatedToasts.length > this.settings.maxToasts) {
      updatedToasts.shift();
    }

    this.toastNotifications.next(updatedToasts);

    // Auto-dismiss après la durée spécifiée
    if (duration > 0) {
      setTimeout(() => {
        this.dismissToast(id);
      }, duration);
    }

    // Effets sonores et haptiques
    this.playNotificationSound(type);
    this.vibrate();

    // Émission d'événement
    this.eventBus.emit('TOAST_SHOWN', { toast });

    return id;
  }

  /**
   * Affiche un toast de succès
   */
  success(title: string, message: string, duration?: number, action?: ToastNotification['action']): string {
    return this.showToast('success', title, message, duration, action);
  }

  /**
   * Affiche un toast d'erreur
   */
  error(title: string, message: string, duration?: number, action?: ToastNotification['action']): string {
    return this.showToast('error', title, message, duration, action);
  }

  /**
   * Affiche un toast d'avertissement
   */
  warning(title: string, message: string, duration?: number, action?: ToastNotification['action']): string {
    return this.showToast('warning', title, message, duration, action);
  }

  /**
   * Affiche un toast d'information
   */
  info(title: string, message: string, duration?: number, action?: ToastNotification['action']): string {
    return this.showToast('info', title, message, duration, action);
  }

  /**
   * Ferme une notification toast
   */
  dismissToast(id: string): void {
    const currentToasts = this.toastNotifications.value;
    const updatedToasts = currentToasts.filter(toast => toast.id !== id);
    this.toastNotifications.next(updatedToasts);
    
    this.eventBus.emit('TOAST_DISMISSED', { id });
  }

  /**
   * Ferme toutes les notifications toast
   */
  dismissAllToasts(): void {
    this.toastNotifications.next([]);
    this.eventBus.emit('ALL_TOASTS_DISMISSED');
  }

  // ──────────────────────────────────────────────────
  // 🍫 SNACKBAR NOTIFICATIONS
  // ──────────────────────────────────────────────────

  /**
   * Affiche une snackbar
   */
  showSnackbar(
    message: string,
    action?: SnackbarNotification['action'],
    duration: number = this.settings.snackbarDuration,
    type: SnackbarNotification['type'] = 'default'
  ): string {
    const id = this.generateId();
    const snackbar: SnackbarNotification = {
      id,
      message,
      action,
      duration,
      type
    };

    this.snackbarNotifications.next(snackbar);

    // Auto-dismiss après la durée spécifiée
    if (duration > 0) {
      setTimeout(() => {
        this.dismissSnackbar();
      }, duration);
    }

    this.eventBus.emit('SNACKBAR_SHOWN', { snackbar });
    return id;
  }

  /**
   * Ferme la snackbar active
   */
  dismissSnackbar(): void {
    const currentSnackbar = this.snackbarNotifications.value;
    if (currentSnackbar) {
      this.eventBus.emit('SNACKBAR_DISMISSED', { id: currentSnackbar.id });
    }
    this.snackbarNotifications.next(null);
  }

  // ──────────────────────────────────────────────────
  // 🗨️ DIALOG MODALS
  // ──────────────────────────────────────────────────

  /**
   * Affiche une boîte de dialogue
   */
  showDialog(config: Omit<DialogConfig, 'id'>): string {
    const id = this.generateId();
    const dialog: DialogConfig = {
      id,
      ...config
    };

    this.activeDialog.next(dialog);
    this.eventBus.emit('DIALOG_SHOWN', { dialog });

    return id;
  }

  /**
   * Affiche une boîte d'alerte
   */
  alert(title: string, message: string, confirmText: string = 'OK'): Promise<void> {
    return new Promise((resolve) => {
      this.showDialog({
        title,
        message,
        type: 'alert',
        confirmText,
        onConfirm: () => resolve()
      });
    });
  }

  /**
   * Affiche une boîte de confirmation
   */
  confirm(title: string, message: string, confirmText: string = 'Confirmer', cancelText: string = 'Annuler'): Promise<boolean> {
    return new Promise((resolve) => {
      this.showDialog({
        title,
        message,
        type: 'confirm',
        confirmText,
        cancelText,
        onConfirm: () => resolve(true),
        onCancel: () => resolve(false)
      });
    });
  }

  /**
   * Affiche une boîte de saisie
   */
  prompt(
    title: string,
    message: string,
    placeholder: string = '',
    defaultValue: string = '',
    confirmText: string = 'OK',
    cancelText: string = 'Annuler'
  ): Promise<string | null> {
    return new Promise((resolve) => {
      this.showDialog({
        title,
        message,
        type: 'prompt',
        inputPlaceholder: placeholder,
        inputValue: defaultValue,
        confirmText,
        cancelText,
        onConfirm: (inputValue) => resolve(inputValue || ''),
        onCancel: () => resolve(null)
      });
    });
  }

  /**
   * Ferme la boîte de dialogue active
   */
  closeDialog(): void {
    const currentDialog = this.activeDialog.value;
    if (currentDialog) {
      this.eventBus.emit('DIALOG_CLOSED', { id: currentDialog.id });
    }
    this.activeDialog.next(null);
  }

  // ──────────────────────────────────────────────────
  // ⚙️ SETTINGS MANAGEMENT
  // ──────────────────────────────────────────────────

  /**
   * Met à jour les paramètres de notification
   */
  updateSettings(newSettings: Partial<NotificationSettings>): void {
    this.settings = { ...this.settings, ...newSettings };
    this.storageService.set('notification-settings', this.settings);
    this.eventBus.emit('NOTIFICATION_SETTINGS_UPDATED', { settings: this.settings });
  }

  /**
   * Retourne les paramètres actuels
   */
  getSettings(): NotificationSettings {
    return { ...this.settings };
  }

  /**
   * Réinitialise les paramètres par défaut
   */
  resetSettings(): void {
    this.storageService.remove('notification-settings');
    this.initializeSettings();
    this.eventBus.emit('NOTIFICATION_SETTINGS_RESET', { settings: this.settings });
  }

  // ──────────────────────────────────────────────────
  // 🔊 EFFECTS & UTILITIES
  // ──────────────────────────────────────────────────

  /**
   * Joue un son de notification
   */
  private playNotificationSound(type: ToastNotification['type']): void {
    if (!this.isBrowser || !this.settings.soundEnabled) return;

    // Implémentation basique - peut être étendue avec des fichiers audio
    const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    // Fréquences différentes selon le type
    const frequencies = {
      success: 523.25, // Do
      error: 392.00,   // Sol
      warning: 440.00, // La
      info: 349.23     // Fa
    };

    oscillator.frequency.value = frequencies[type];
    oscillator.type = 'sine';

    gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

    oscillator.start(audioContext.currentTime);
    oscillator.stop(audioContext.currentTime + 0.5);
  }

  /**
   * Déclenche une vibration (si supportée)
   */
  private vibrate(): void {
    if (!this.isBrowser || !this.settings.vibrationEnabled || !navigator.vibrate) return;

    navigator.vibrate(200);
  }

  /**
   * Génère un ID unique
   */
  private generateId(): string {
    return `notification_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Vérifie si les notifications sont supportées
   */
  isSupported(): boolean {
    return this.isBrowser;
  }

  /**
   * Demande la permission pour les notifications push (future implémentation)
   */
  async requestPermission(): Promise<NotificationPermission> {
    if (!this.isBrowser || !('Notification' in window)) {
      return 'denied';
    }

    if (Notification.permission === 'default') {
      return await Notification.requestPermission();
    }

    return Notification.permission;
  }

  /**
   * Envoie une notification push (future implémentation)
   */
  sendPushNotification(title: string, options?: NotificationOptions): void {
    if (!this.isBrowser || !('Notification' in window) || Notification.permission !== 'granted') {
      return;
    }

    new Notification(title, options);
  }
}