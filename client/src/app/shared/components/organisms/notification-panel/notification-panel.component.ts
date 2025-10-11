export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'REMINDER' | 'WARNING' | 'SUCCESS' | 'INFO';
  isRead: boolean;
  createdAt: string;
}

import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';

@Component({
  selector: 'app-notification-panel',
     standalone: true,
  imports: [IconComponent, TypographyComponent],
  templateUrl: './notification-panel.component.html',
  styleUrl: './notification-panel.component.scss'
})
export class NotificationPanelComponent {
 @Input() notifications: Notification[] = [];
  @Input() loading = false;
  @Input() showActions = true;

  @Output() markAsRead = new EventEmitter<string>();
  @Output() deleteNotification = new EventEmitter<string>();

  getIconForType(type: string): string {
    switch (type) {
      case 'REMINDER': return 'fa-clock';
      case 'WARNING': return 'fa-exclamation-triangle';
      case 'SUCCESS': return 'fa-check-circle';
      default: return 'fa-info-circle';
    }
  }

  getColorForType(type: string): 'primary' | 'success' | 'warning' | 'danger' | 'text-secondary' {
    switch (type) {
      case 'REMINDER': return 'primary';
      case 'SUCCESS': return 'success';
      case 'WARNING': return 'warning';
      case 'INFO': return 'text-secondary';
      default: return 'text-secondary';
    }
  }

  onMarkAsRead(id: string): void {
    this.markAsRead.emit(id);
  }

  onDelete(id: string): void {
    this.deleteNotification.emit(id);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: 'short',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
