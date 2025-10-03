# Documentation des Endpoints API et Interfaces Angular

## Vue d'ensemble de l'API

L'API de la bibliothèque expose des endpoints RESTful organisés autour des fonctionnalités principales :
- **Authentification** : Gestion des utilisateurs et rôles
- **Livres** : Gestion du catalogue et importation depuis Google Books
- **Emprunts** : Gestion des prêts de livres
- **Réservations** : Système de réservation
- **Notifications** : Communication avec les utilisateurs
- **Analytiques** : Statistiques et rapports
- **Profil Utilisateur** : Gestion du profil, préférences et historique de lecture

---

## Endpoints API

### 🔐 Authentification (`/api/auth`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `POST` | `/api/auth/register` | Public | Inscription d'un nouvel utilisateur |
| `POST` | `/api/auth/login` | Public | Connexion et obtention du token JWT |
| `GET` | `/api/auth/me` | Authentifié | Récupération des informations de l'utilisateur connecté |
| `GET` | `/api/auth/users` | Manager, Admin | Liste de tous les utilisateurs |
| `PUT` | `/api/auth/users/{userId}` | Manager, Admin | Mise à jour d'un utilisateur |
| `DELETE` | `/api/auth/users/{userId}` | Manager, Admin | Suppression d'un utilisateur |
| `POST` | `/api/auth/users/{userId}/roles` | Manager, Admin | Ajout d'un rôle à un utilisateur |
| `DELETE` | `/api/auth/users/{userId}/roles/{role}` | Manager, Admin | Suppression d'un rôle d'un utilisateur |

### 📚 Livres (`/api/books`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `GET` | `/api/books` | Public | Liste paginée des livres avec filtres |
| `GET` | `/api/books/{id}` | Public | Détails d'un livre spécifique |
| `GET` | `/api/books/search` | Public | Recherche avancée de livres |
| `GET` | `/api/books/categories/{categoryId}` | Public | Livres par catégorie |
| `GET` | `/api/books/recent` | Public | Livres récemment ajoutés |
| `GET` | `/api/books/popular` | Public | Livres les plus populaires |
| `POST` | `/api/books` | Manager, Admin | Création d'un nouveau livre |
| `PUT` | `/api/books/{id}` | Manager, Admin | Mise à jour d'un livre |
| `DELETE` | `/api/books/{id}` | Admin | Suppression d'un livre |
| `POST` | `/api/books/{id}/copies` | Manager, Admin | Ajout d'exemplaires |
| `PUT` | `/api/books/{id}/availability` | Manager, Admin | Mise à jour de la disponibilité |
| `GET` | `/api/books/{id}/stats` | Manager, Admin | Statistiques d'un livre |
| `GET` | `/api/books/categories` | Public | Liste des catégories |
| `GET` | `/api/books/categories/{id}` | Public | Détails d'une catégorie |
| `POST` | `/api/books/categories` | Manager, Admin | Création d'une catégorie |
| `PUT` | `/api/books/categories/{id}` | Manager, Admin | Mise à jour d'une catégorie |
| `DELETE` | `/api/books/categories/{id}` | Admin | Suppression d'une catégorie |
| `POST` | `/api/books/import/google` | Manager, Admin | Importation depuis Google Books |
| `GET` | `/api/books/import/google/search` | Manager, Admin | Recherche dans Google Books |

### 📖 Emprunts (`/api/loans`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `GET` | `/api/loans` | Authentifié | Liste des emprunts avec filtres |
| `GET` | `/api/loans/overdue` | Manager, Admin | Emprunts en retard |
| `GET` | `/api/loans/user/{userId}` | Manager, Admin | Historique d'un utilisateur |
| `POST` | `/api/loans` | User, Manager, Admin | Création d'un emprunt |
| `GET` | `/api/loans/{id}` | Authentifié | Détails d'un emprunt |
| `PUT` | `/api/loans/{id}/return` | Authentifié | Retour d'un livre |
| `PUT` | `/api/loans/{id}/extend` | User, Manager, Admin | Prolongation d'un emprunt |
| `PUT` | `/api/loans/{id}/notify-overdue` | Manager, Admin | Notification de retard |
| `GET` | `/api/loans/can-borrow/{bookId}` | User, Manager, Admin | Vérification d'emprunt possible |
| `GET` | `/api/loans/stats` | User, Manager, Admin | Statistiques personnelles |

### 🔖 Réservations (`/api/reservations`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `POST` | `/api/reservations` | User, Manager, Admin | Création d'une réservation |
| `GET` | `/api/reservations` | Authentifié | Liste des réservations |
| `GET` | `/api/reservations/{id}` | Authentifié | Détails d'une réservation |
| `DELETE` | `/api/reservations/{id}` | Authentifié | Annulation d'une réservation |
| `GET` | `/api/reservations/can-reserve/{bookId}` | User, Manager, Admin | Vérification de réservation possible |
| `GET` | `/api/reservations/expired` | Manager, Admin | Réservations expirées |
| `GET` | `/api/reservations/available` | Manager, Admin | Réservations disponibles |
| `PUT` | `/api/reservations/{id}/status` | Manager, Admin | Mise à jour du statut |

### 🔔 Notifications (`/api/notifications`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `GET` | `/api/notifications` | Authentifié | Liste des notifications |
| `PUT` | `/api/notifications/{id}/read` | Authentifié | Marquer comme lue |
| `PUT` | `/api/notifications/mark-all-read` | Authentifié | Tout marquer comme lu |
| `POST` | `/api/notifications` | Manager, Admin | Création d'une notification |
| `GET` | `/api/notifications/unread-count` | Authentifié | Nombre de notifications non lues |

### 👤 Profil Utilisateur (`/api/userprofile`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `GET` | `/api/userprofile` | Authentifié | Récupérer le profil de l'utilisateur connecté |
| `POST` | `/api/userprofile` | Authentifié | Créer un profil utilisateur |
| `PUT` | `/api/userprofile` | Authentifié | Mettre à jour le profil utilisateur |
| `DELETE` | `/api/userprofile` | Authentifié | Supprimer le profil utilisateur |
| `POST` | `/api/userprofile/initialize` | Authentifié | Initialiser le profil avec valeurs par défaut |
| `GET` | `/api/userprofile/preferences/categories` | Authentifié | Préférences de catégorie de l'utilisateur |
| `POST` | `/api/userprofile/preferences/categories` | Authentifié | Ajouter/mettre à jour une préférence de catégorie |
| `DELETE` | `/api/userprofile/preferences/categories/{categoryId}` | Authentifié | Supprimer une préférence de catégorie |
| `GET` | `/api/userprofile/goals` | Authentifié | Objectifs de lecture de l'utilisateur |
| `POST` | `/api/userprofile/goals` | Authentifié | Créer un objectif de lecture |
| `GET` | `/api/userprofile/goals/{goalId}` | Authentifié | Détails d'un objectif de lecture |
| `PUT` | `/api/userprofile/goals/{goalId}` | Authentifié | Mettre à jour un objectif de lecture |
| `DELETE` | `/api/userprofile/goals/{goalId}` | Authentifié | Supprimer un objectif de lecture |
| `PUT` | `/api/userprofile/goals/{goalId}/progress` | Authentifié | Mettre à jour la progression d'un objectif |
| `GET` | `/api/userprofile/history` | Authentifié | Historique de lecture de l'utilisateur |
| `GET` | `/api/userprofile/history/loan/{loanId}` | Authentifié | Historique de lecture par emprunt |
| `POST` | `/api/userprofile/history` | Authentifié | Créer un historique de lecture |
| `PUT` | `/api/userprofile/history/{historyId}` | Authentifié | Mettre à jour un historique de lecture |
| `DELETE` | `/api/userprofile/history/{historyId}` | Authentifié | Supprimer un historique de lecture |
| `GET` | `/api/userprofile/stats` | Authentifié | Statistiques de lecture de l'utilisateur |

### 📊 Analytiques (`/api/analytics`)

| Méthode | Endpoint | Rôles | Description |
|---------|----------|-------|-------------|
| `GET` | `/api/analytics/categories/popular` | Manager, Admin | Catégories populaires |
| `GET` | `/api/analytics/trends/borrows` | Manager, Admin | Tendances des emprunts |
| `GET` | `/api/analytics/reservations/stats` | Manager, Admin | Statistiques des réservations |
| `GET` | `/api/analytics/books/top` | Manager, Admin | Livres les plus empruntés |
| `GET` | `/api/analytics/users/active` | Manager, Admin | Utilisateurs actifs |
| `GET` | `/api/analytics/overview` | Manager, Admin | Vue d'ensemble |
| `GET` | `/api/analytics/audit/logs` | Admin | Logs d'audit |

---

## Interfaces Angular Exportables

### Modèles d'Authentification

```typescript
// User
export interface User {
  id: string;
  name: string;
  email: string;
  roles: string[];
  createdAt: string;
  updatedAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  tokenExpires: string;
  user: User;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface UpdateUserRequest {
  name?: string;
  email?: string;
}
```

### Modèles de Livres

```typescript
// Book
export interface Book {
  id: string;
  title: string;
  author: string;
  category: string;
  isbn: string;
  publicationDate?: string;
  coverUrl: string;
  availableCopies: number;
  totalCopies: number;
  description: string;
  pageCount?: number;
  publisher: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  category: string;
  isbn: string;
  publicationDate?: string;
  coverUrl: string;
  totalCopies: number;
  description: string;
  pageCount?: number;
  publisher: string;
}

export interface UpdateBookRequest {
  title?: string;
  author?: string;
  category?: string;
  isbn?: string;
  publicationDate?: string;
  coverUrl?: string;
  description?: string;
  pageCount?: number;
  publisher?: string;
}

export interface BookStats {
  totalLoans: number;
  activeLoans: number;
  totalReservations: number;
  popularityScore: number;
}

export interface ImportBookFromGoogleRequest {
  isbn: string;
  totalCopies: number;
  defaultCategory: string;
}

export interface GoogleBookPreview {
  title: string;
  authors: string[];
  publishedDate: string;
  description: string;
  isbn: string;
  coverUrl: string;
  categories: string[];
  pageCount?: number;
  publisher: string;
  googleBooksId: string;
}
```

### Modèles d'Emprunts

```typescript
// Loan
export interface Loan {
  id: string;
  bookId: string;
  userId: string;
  bookTitle: string;
  userName: string;
  borrowDate: string;
  dueDate: string;
  returnDate?: string;
  status: 'ACTIVE' | 'RETURNED' | 'OVERDUE';
  canExtend: boolean;
}

export interface CreateLoanRequest {
  bookId: string;
}

export interface ExtendLoanRequest {
  additionalDays: number;
}

export interface LoanStats {
  totalLoans: number;
  activeLoans: number;
  overdueLoans: number;
  favoriteCategory: string;
}
```

### Modèles de Réservations

```typescript
// Reservation
export interface Reservation {
  id: string;
  bookId: string;
  userId: string;
  bookTitle: string;
  userName: string;
  reservationDate: string;
  status: 'PENDING' | 'AVAILABLE' | 'CANCELLED' | 'EXPIRED';
  availableSince?: string;
  expiryDate: string;
}

export interface CreateReservationRequest {
  bookId: string;
}

export interface UpdateReservationStatusRequest {
  status: string;
  availableSince?: string;
}
```

### Modèles de Notifications

```typescript
// Notification
export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'REMINDER' | 'INFO' | 'WARNING' | 'SUCCESS';
  isRead: boolean;
  createdAt: string;
  relatedEntityId?: string;
  relatedEntityType?: string;
}

export interface CreateNotificationRequest {
  title: string;
  message: string;
  type: string;
  userId?: string;
  relatedEntityId?: string;
  relatedEntityType?: string;
}

export interface UnreadNotificationsResponse {
  unreadCount: number;
  unreadByType: { [key: string]: number };
}
```

### Modèles d'Analytiques

```typescript
// Analytics
export interface AnalyticsOverview {
  totalBooks: number;
  totalUsers: number;
  activeLoans: number;
  pendingReservations: number;
  todayLoans: number;
  monthlyGrowth: number;
}

export interface PopularCategory {
  categoryName: string;
  borrowCount: number;
  percentage: number;
}

export interface BorrowTrend {
  period: string;
  borrowCount: number;
  trend: number;
}

export interface TopBook {
  bookId: string;
  title: string;
  author: string;
  borrowCount: number;
}

export interface ActiveUser {
  userId: string;
  userName: string;
  email: string;
  loanCount: number;
  lastActivity: string;
}

export interface ReservationStats {
  totalReservations: number;
  pendingCount: number;
  availableCount: number;
  completionRate: number;
}

export interface AuditLog {
  id: string;
  userId: string;
  userName: string;
  action: string;
  entityType: string;
  entityId: string;
  timestamp: string;
  details?: string;
}
```

### Modèles de Pagination et Réponses

```typescript
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

// Search and Filters
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

export interface UserProfile {
  id: string;
  userId: string;
  bio?: string;
  favoriteAuthor?: string;
  favoriteGenre?: string;
  emailNotifications: boolean;
  pushNotifications: boolean;
  newBookAlerts: boolean;
  returnReminders: boolean;
  reviewReminders: boolean;
  theme: string;
  itemsPerPage: number;
  language: string;
  totalBooksRead: number;
  totalPagesRead: number;
  averageReadingTime: number;
  readingLevel?: string;
  createdAt: string;
  updatedAt: string;
  categoryPreferences: UserCategoryPreference[];
  readingGoals: UserReadingGoal[];
  readingHistory: UserReadingHistory[];
}

export interface CreateUserProfileRequest {
  bio?: string;
  favoriteAuthor?: string;
  favoriteGenre?: string;
  emailNotifications?: boolean;
  pushNotifications?: boolean;
  newBookAlerts?: boolean;
  returnReminders?: boolean;
  reviewReminders?: boolean;
  theme?: string;
  itemsPerPage?: number;
  language?: string;
}

export interface UpdateUserProfileRequest {
  bio?: string;
  favoriteAuthor?: string;
  favoriteGenre?: string;
  emailNotifications?: boolean;
  pushNotifications?: boolean;
  newBookAlerts?: boolean;
  returnReminders?: boolean;
  reviewReminders?: boolean;
  theme?: string;
  itemsPerPage?: number;
  language?: string;
}

export interface UserCategoryPreference {
  id: string;
  categoryId: string;
  categoryName: string;
  preferenceScore: number;
  lastInteracted: string;
  interactionCount: number;
}

export interface CreateUserCategoryPreferenceRequest {
  categoryId: string;
  preferenceScore: number;
}

export interface UserReadingGoal {
  id: string;
  title: string;
  description?: string;
  targetBooks: number;
  currentProgress: number;
  progressPercentage: number;
  startDate: string;
  endDate: string;
  isCompleted: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserReadingGoalRequest {
  title: string;
  description?: string;
  targetBooks: number;
  startDate: string;
  endDate: string;
}

export interface UpdateUserReadingGoalRequest {
  title?: string;
  description?: string;
  targetBooks?: number;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}

export interface UpdateReadingGoalProgressRequest {
  booksRead: number;
}

export interface UserReadingHistory {
  id: string;
  bookId: string;
  loanId: string;
  bookTitle: string;
  bookAuthor: string;
  bookCoverUrl?: string;
  startedReading: string;
  finishedReading?: string;
  readingTimeMinutes?: number;
  rating?: number;
  review?: string;
  isFavorite: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserReadingHistoryRequest {
  bookId: string;
  loanId: string;
  startedReading: string;
  finishedReading?: string;
  readingTimeMinutes?: number;
  rating?: number;
  review?: string;
  isFavorite?: boolean;
}

export interface UpdateUserReadingHistoryRequest {
  finishedReading?: string;
  readingTimeMinutes?: number;
  rating?: number;
  review?: string;
  isFavorite?: boolean;
}

export interface UserReadingStats {
  totalBooksRead: number;
  totalPagesRead: number;
  averageReadingTime: number;
  activeGoals: number;
  completedGoals: number;
  favoriteCategory?: string;
  favoriteAuthor?: string;
  totalReadingTimeMinutes: number;
  booksPerMonth: number;
  booksByCategory: { [key: string]: number };
  ratingDistribution: { [key: number]: number };
}
```

---

## Types d'Enums

```typescript
// Rôles utilisateur
export enum UserRole {
  ADMIN = 'Admin',
  MANAGER = 'Manager',
  USER = 'User'
}

// Statuts d'emprunt
export enum LoanStatus {
  ACTIVE = 'ACTIVE',
  RETURNED = 'RETURNED',
  OVERDUE = 'OVERDUE'
}

// Statuts de réservation
export enum ReservationStatus {
  PENDING = 'PENDING',
  AVAILABLE = 'AVAILABLE',
  CANCELLED = 'CANCELLED',
  EXPIRED = 'EXPIRED'
}

// Types de notification
export enum NotificationType {
  REMINDER = 'REMINDER',
  INFO = 'INFO',
  WARNING = 'WARNING',
  SUCCESS = 'SUCCESS'
}

// Périodes pour les analytiques
export enum AnalyticsPeriod {
  DAY = 'DAY',
  WEEK = 'WEEK',
  MONTH = 'MONTH',
  YEAR = 'YEAR'
}

// Types de thème pour le profil utilisateur
export enum ThemeType {
  LIGHT = 'light',
  DARK = 'dark',
  AUTO = 'auto'
}

// Niveaux de lecture
export enum ReadingLevel {
  BEGINNER = 'beginner',
  INTERMEDIATE = 'intermediate',
  ADVANCED = 'advanced'
}

// Fréquences de lecture
export enum ReadingPace {
  SLOW = 'slow',
  MODERATE = 'moderate',
  FAST = 'fast'
}
```

---

## Configuration Angular Recommandée

### Services Angular

```typescript
// Exemple de service Angular
@Injectable({
  providedIn: 'root'
})
export class BookService {
  constructor(private http: HttpClient) {}

  getBooks(filters: BookSearchFilters): Observable<PaginatedResponse<Book>> {
    return this.http.get<PaginatedResponse<Book>>('/api/books', { params: filters as any });
  }

  createBook(book: CreateBookRequest): Observable<Book> {
    return this.http.post<Book>('/api/books', book);
  }

  importFromGoogle(request: ImportBookFromGoogleRequest): Observable<Book> {
    return this.http.post<Book>('/api/books/import/google', request);
  }
}
```

### Intercepteur d'Authentification

```typescript
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('auth_token');
    
    if (token) {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(cloned);
    }
    
    return next.handle(req);
    }
  }
  
  // Exemple de service Angular pour le profil utilisateur
  @Injectable({
    providedIn: 'root'
  })
  export class UserProfileService {
    constructor(private http: HttpClient) {}
  
    getUserProfile(): Observable<UserProfile> {
      return this.http.get<UserProfile>('/api/userprofile');
    }
  
    createUserProfile(profile: CreateUserProfileRequest): Observable<UserProfile> {
      return this.http.post<UserProfile>('/api/userprofile', profile);
    }
  
    updateUserProfile(profile: UpdateUserProfileRequest): Observable<UserProfile> {
      return this.http.put<UserProfile>('/api/userprofile', profile);
    }
  
    deleteUserProfile(): Observable<void> {
      return this.http.delete<void>('/api/userprofile');
    }
  
    initializeUserProfile(): Observable<void> {
      return this.http.post<void>('/api/userprofile/initialize', {});
    }
  
    getCategoryPreferences(): Observable<UserCategoryPreference[]> {
      return this.http.get<UserCategoryPreference[]>('/api/userprofile/preferences/categories');
    }
  
    addOrUpdateCategoryPreference(preference: CreateUserCategoryPreferenceRequest): Observable<UserCategoryPreference> {
      return this.http.post<UserCategoryPreference>('/api/userprofile/preferences/categories', preference);
    }
  
    removeCategoryPreference(categoryId: string): Observable<void> {
      return this.http.delete<void>(`/api/userprofile/preferences/categories/${categoryId}`);
    }
  
    getReadingGoals(activeOnly?: boolean): Observable<UserReadingGoal[]> {
      const params = activeOnly !== undefined ? { activeOnly: activeOnly.toString() } : {};
      return this.http.get<UserReadingGoal[]>('/api/userprofile/goals', { params });
    }
  
    createReadingGoal(goal: CreateUserReadingGoalRequest): Observable<UserReadingGoal> {
      return this.http.post<UserReadingGoal>('/api/userprofile/goals', goal);
    }
  
    updateReadingGoal(goalId: string, goal: UpdateUserReadingGoalRequest): Observable<UserReadingGoal> {
      return this.http.put<UserReadingGoal>(`/api/userprofile/goals/${goalId}`, goal);
    }
  
    updateReadingGoalProgress(goalId: string, progress: UpdateReadingGoalProgressRequest): Observable<UserReadingGoal> {
      return this.http.put<UserReadingGoal>(`/api/userprofile/goals/${goalId}/progress`, progress);
    }
  
    deleteReadingGoal(goalId: string): Observable<void> {
      return this.http.delete<void>(`/api/userprofile/goals/${goalId}`);
    }
  
    getReadingHistory(limit?: number): Observable<UserReadingHistory[]> {
      const params = limit ? { limit: limit.toString() } : {};
      return this.http.get<UserReadingHistory[]>('/api/userprofile/history', { params });
    }
  
    getReadingHistoryByLoan(loanId: string): Observable<UserReadingHistory> {
      return this.http.get<UserReadingHistory>(`/api/userprofile/history/loan/${loanId}`);
    }
  
    createReadingHistory(history: CreateUserReadingHistoryRequest): Observable<UserReadingHistory> {
      return this.http.post<UserReadingHistory>('/api/userprofile/history', history);
    }
  
    updateReadingHistory(historyId: string, history: UpdateUserReadingHistoryRequest): Observable<UserReadingHistory> {
      return this.http.put<UserReadingHistory>(`/api/userprofile/history/${historyId}`, history);
    }
  
    deleteReadingHistory(historyId: string): Observable<void> {
      return this.http.delete<void>(`/api/userprofile/history/${historyId}`);
    }
  
    getReadingStats(): Observable<UserReadingStats> {
      return this.http.get<UserReadingStats>('/api/userprofile/stats');
    }
  }
```

Cette documentation fournit une base complète pour l'intégration frontend avec l'API de la bibliothèque.