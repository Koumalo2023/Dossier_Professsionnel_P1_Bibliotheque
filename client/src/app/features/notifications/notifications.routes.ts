import { Routes } from '@angular/router';
import { NotificationListComponent } from './notification-list/notification-list.component';
import { NotificationSettingsComponent } from './notification-settings/notification-settings.component';
import { NotificationManagementComponent } from './notification-management/notification-management.component';
import { NotificationsLayoutComponent } from './notifications-layout/notifications-layout.component';

export const notificationsRoutes: Routes = [
  {
    path: '',
    component: NotificationsLayoutComponent,
    children: [
      { path: '', component: NotificationListComponent },
      { path: 'settings', component: NotificationSettingsComponent },
      { path: 'management', component: NotificationManagementComponent }
    ]
  }
];