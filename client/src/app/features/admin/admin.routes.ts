import { Routes } from '@angular/router';
import { AdminDashboardPageComponent } from './admin-dashboard-page/admin-dashboard-page.component';
import { AdminBookManagementPageComponent } from './admin-book-management-page/admin-book-management-page.component';
import { AdminLoansPageComponent } from './admin-loans-page/admin-loans-page.component';
import { AdminReservationManagementPageComponent } from './admin-reservation-management-page/admin-reservation-management-page.component';
import { AdminNotificationManagementPageComponent } from './admin-notification-management-page/admin-notification-management-page.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';

export const adminRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', component: AdminDashboardPageComponent },
      { path: 'books', component: AdminBookManagementPageComponent },
      { path: 'loans', component: AdminLoansPageComponent },
      { path: 'reservations', component: AdminReservationManagementPageComponent },
      { path: 'notifications', component: AdminNotificationManagementPageComponent }
    ]
  }
];