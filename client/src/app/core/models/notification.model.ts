// Modèles de notifications

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
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

// Types de notification
export enum NotificationType {
  REMINDER = 'REMINDER',
  INFO = 'INFO',
  WARNING = 'WARNING',
  SUCCESS = 'SUCCESS'
}