
export interface Notification {
    id: string;
    userId: string;
    message: string;
    type: string;
    read: boolean;
    createdAt: Date;
  }
  
  // DTO pour la création d'une nouvelle notification
  export interface CreateNotificationDto {
    userId: string;
    message: string;
    type: string;
  }