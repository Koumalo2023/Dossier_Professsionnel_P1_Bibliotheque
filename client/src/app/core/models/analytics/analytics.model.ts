// Modèles d'analytiques

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

// Périodes pour les analytiques
export enum AnalyticsPeriod {
  DAY = 'DAY',
  WEEK = 'WEEK',
  MONTH = 'MONTH',
  YEAR = 'YEAR'
}