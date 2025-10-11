// Modèles de profil utilisateur

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
  theme: ThemeType;
  itemsPerPage: number;
  language: string;
  totalBooksRead: number;
  totalPagesRead: number;
  averageReadingTime: number;
  readingLevel?: ReadingLevel;
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
  theme?: ThemeType;
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
  theme?: ThemeType;
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

// Types pour le profil utilisateur
export enum ThemeType {
  LIGHT = 'light',
  DARK = 'dark',
  AUTO = 'auto'
}

export enum ReadingLevel {
  BEGINNER = 'beginner',
  INTERMEDIATE = 'intermediate',
  ADVANCED = 'advanced'
}

export enum ReadingPace {
  SLOW = 'slow',
  MODERATE = 'moderate',
  FAST = 'fast'
}