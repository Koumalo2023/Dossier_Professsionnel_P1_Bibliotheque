import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from '../../config/api.config';
import { PaginatedResponse } from '../../models/shared.model';
import { CreateNotificationRequest, UnreadNotificationsResponse } from '../../models/notification.model';


@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private apiConfig = inject(ApiConfigService);

  // Gestion des notifications
  getNotifications(filters?: any): Observable<PaginatedResponse<Notification>> {
    let params = new HttpParams();
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<PaginatedResponse<Notification>>(
      this.apiConfig.buildNotificationsUrl('userNotifications'),
      { params }
    );
  }

  markAsRead(id: string): Observable<Notification> {
    return this.http.put<Notification>(
      `${this.apiConfig.buildNotificationsUrl('base')}/${id}/read`,
      {}
    );
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>(
      this.apiConfig.buildNotificationsUrl('markAllRead'),
      {}
    );
  }

  createNotification(notification: CreateNotificationRequest): Observable<Notification> {
    return this.http.post<Notification>(
      this.apiConfig.buildNotificationsUrl('base'),
      notification
    );
  }

  getUnreadCount(): Observable<UnreadNotificationsResponse> {
    return this.http.get<UnreadNotificationsResponse>(
      `${this.apiConfig.buildNotificationsUrl('base')}/unread-count`
    );
  }
}