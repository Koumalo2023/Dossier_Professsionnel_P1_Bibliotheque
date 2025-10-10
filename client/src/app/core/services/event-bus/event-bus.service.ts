import { Injectable } from '@angular/core';
import { Subject, Observable, filter, map } from 'rxjs';

// Interface pour les événements
export interface AppEvent {
  type: string;
  payload?: any;
  timestamp: number;
}

// Types d'événements prédéfinis
export enum EventTypes {
  // Authentification
  USER_LOGIN = 'USER_LOGIN',
  USER_LOGOUT = 'USER_LOGOUT',
  USER_PROFILE_UPDATED = 'USER_PROFILE_UPDATED',
  USER_REGISTERED = 'USER_REGISTERED',
  USER_UPDATED = 'USER_UPDATED',
  USER_DELETED = 'USER_DELETED',
  USER_ROLE_ADDED = 'USER_ROLE_ADDED',
  USER_ROLE_REMOVED = 'USER_ROLE_REMOVED',
  TOKEN_REFRESHED = 'TOKEN_REFRESHED',
  
  // Livres
  BOOK_CREATED = 'BOOK_CREATED',
  BOOK_UPDATED = 'BOOK_UPDATED',
  BOOK_DELETED = 'BOOK_DELETED',
  BOOK_IMPORTED = 'BOOK_IMPORTED',
  
  // Emprunts
  LOAN_CREATED = 'LOAN_CREATED',
  LOAN_RETURNED = 'LOAN_RETURNED',
  LOAN_EXTENDED = 'LOAN_EXTENDED',
  LOAN_OVERDUE = 'LOAN_OVERDUE',
  
  // Réservations
  RESERVATION_CREATED = 'RESERVATION_CREATED',
  RESERVATION_CANCELLED = 'RESERVATION_CANCELLED',
  RESERVATION_AVAILABLE = 'RESERVATION_AVAILABLE',
  
  // Notifications
  NOTIFICATION_RECEIVED = 'NOTIFICATION_RECEIVED',
  NOTIFICATION_READ = 'NOTIFICATION_READ',
  NOTIFICATION_CLEARED = 'NOTIFICATION_CLEARED',
  
  // UI/UX
  THEME_CHANGED = 'THEME_CHANGED',
  LANGUAGE_CHANGED = 'LANGUAGE_CHANGED',
  LOADING_STARTED = 'LOADING_STARTED',
  LOADING_FINISHED = 'LOADING_FINISHED',
  
  // Navigation
  NAVIGATION_STARTED = 'NAVIGATION_STARTED',
  NAVIGATION_ENDED = 'NAVIGATION_ENDED'
}

@Injectable({
  providedIn: 'root'
})
export class EventBusService {
  private eventSubject = new Subject<AppEvent>();
  private eventHistory: AppEvent[] = [];
  private readonly MAX_HISTORY_SIZE = 100;

  // Émettre un événement
  emit(eventType: string, payload?: any): void {
    const event: AppEvent = {
      type: eventType,
      payload,
      timestamp: Date.now()
    };
    
    this.eventSubject.next(event);
    this.addToHistory(event);
    
    // Log en développement
    if (!environment.production) {
      console.log(`Event emitted: ${eventType}`, payload);
    }
  }

  // Écouter tous les événements
  on(): Observable<AppEvent> {
    return this.eventSubject.asObservable();
  }

  // Écouter un type d'événement spécifique
  onEvent(eventType: string): Observable<AppEvent> {
    return this.eventSubject.pipe(
      filter(event => event.type === eventType)
    );
  }

  // Écouter un type d'événement et extraire le payload
  onEventPayload<T>(eventType: string): Observable<T> {
    return this.onEvent(eventType).pipe(
      map(event => event.payload as T)
    );
  }

  // Écouter plusieurs types d'événements
  onEvents(eventTypes: string[]): Observable<AppEvent> {
    return this.eventSubject.pipe(
      filter(event => eventTypes.includes(event.type))
    );
  }

  // Méthodes utilitaires pour les événements courants

  // Authentification
  emitUserLogin(user: any): void {
    this.emit(EventTypes.USER_LOGIN, user);
  }

  emitUserLogout(): void {
    this.emit(EventTypes.USER_LOGOUT);
  }

  emitUserProfileUpdated(profile: any): void {
    this.emit(EventTypes.USER_PROFILE_UPDATED, profile);
  }

  emitUserRegistered(user: any): void {
    this.emit(EventTypes.USER_REGISTERED, user);
  }

  emitUserUpdated(user: any): void {
    this.emit(EventTypes.USER_UPDATED, user);
  }

  emitUserDeleted(userId: string): void {
    this.emit(EventTypes.USER_DELETED, { userId });
  }

  emitUserRoleAdded(userId: string, role: string): void {
    this.emit(EventTypes.USER_ROLE_ADDED, { userId, role });
  }

  emitUserRoleRemoved(userId: string, role: string): void {
    this.emit(EventTypes.USER_ROLE_REMOVED, { userId, role });
  }

  emitTokenRefreshed(): void {
    this.emit(EventTypes.TOKEN_REFRESHED);
  }

  // Livres
  emitBookCreated(book: any): void {
    this.emit(EventTypes.BOOK_CREATED, book);
  }

  emitBookUpdated(book: any): void {
    this.emit(EventTypes.BOOK_UPDATED, book);
  }

  emitBookDeleted(bookId: string): void {
    this.emit(EventTypes.BOOK_DELETED, { bookId });
  }

  emitBookImported(book: any): void {
    this.emit(EventTypes.BOOK_IMPORTED, book);
  }

  // Emprunts
  emitLoanCreated(loan: any): void {
    this.emit(EventTypes.LOAN_CREATED, loan);
  }

  emitLoanReturned(loan: any): void {
    this.emit(EventTypes.LOAN_RETURNED, loan);
  }

  emitLoanExtended(loan: any): void {
    this.emit(EventTypes.LOAN_EXTENDED, loan);
  }

  emitLoanOverdue(loan: any): void {
    this.emit(EventTypes.LOAN_OVERDUE, loan);
  }

  // Réservations
  emitReservationCreated(reservation: any): void {
    this.emit(EventTypes.RESERVATION_CREATED, reservation);
  }

  emitReservationCancelled(reservation: any): void {
    this.emit(EventTypes.RESERVATION_CANCELLED, reservation);
  }

  emitReservationAvailable(reservation: any): void {
    this.emit(EventTypes.RESERVATION_AVAILABLE, reservation);
  }

  // Notifications
  emitNotificationReceived(notification: any): void {
    this.emit(EventTypes.NOTIFICATION_RECEIVED, notification);
  }

  emitNotificationRead(notificationId: string): void {
    this.emit(EventTypes.NOTIFICATION_READ, { notificationId });
  }

  emitNotificationCleared(): void {
    this.emit(EventTypes.NOTIFICATION_CLEARED);
  }

  // UI/UX
  emitThemeChanged(theme: string): void {
    this.emit(EventTypes.THEME_CHANGED, { theme });
  }

  emitLanguageChanged(language: string): void {
    this.emit(EventTypes.LANGUAGE_CHANGED, { language });
  }

  emitLoadingStarted(operation?: string): void {
    this.emit(EventTypes.LOADING_STARTED, { operation });
  }

  emitLoadingFinished(operation?: string): void {
    this.emit(EventTypes.LOADING_FINISHED, { operation });
  }

  // Navigation
  emitNavigationStarted(route: string): void {
    this.emit(EventTypes.NAVIGATION_STARTED, { route });
  }

  emitNavigationEnded(route: string): void {
    this.emit(EventTypes.NAVIGATION_ENDED, { route });
  }

  // Gestion de l'historique
  private addToHistory(event: AppEvent): void {
    this.eventHistory.unshift(event);
    
    // Limiter la taille de l'historique
    if (this.eventHistory.length > this.MAX_HISTORY_SIZE) {
      this.eventHistory = this.eventHistory.slice(0, this.MAX_HISTORY_SIZE);
    }
  }

  getEventHistory(): AppEvent[] {
    return [...this.eventHistory];
  }

  clearEventHistory(): void {
    this.eventHistory = [];
  }

  // Recherche dans l'historique
  findEventsByType(eventType: string): AppEvent[] {
    return this.eventHistory.filter(event => event.type === eventType);
  }

  findEventsByTimeRange(startTime: number, endTime: number): AppEvent[] {
    return this.eventHistory.filter(
      event => event.timestamp >= startTime && event.timestamp <= endTime
    );
  }

  // Débogage
  printEventHistory(): void {
    console.log('Event History:', this.eventHistory);
  }
}

// Import d'environnement (à créer si nécessaire)
const environment = {
  production: false
};