import { Routes } from '@angular/router';
import { AnalyticsDashboardPageComponent } from './analytics-dashboard-page/analytics-dashboard-page.component';
import { CategoryStatsPageComponent } from './category-stats-page/category-stats-page.component';
import { LoanTrendsPageComponent } from './loan-trends-page/loan-trends-page.component';
import { UserActivityPageComponent } from './user-activity-page/user-activity-page.component';
import { ReservationStatsPageComponent } from './reservation-stats-page/reservation-stats-page.component';
import { AnalyticsLayoutComponent } from './analytics-layout/analytics-layout.component';

export const analyticsRoutes: Routes = [
  {
    path: '',
    component: AnalyticsLayoutComponent,
    children: [
      { path: '', component: AnalyticsDashboardPageComponent },
      { path: 'categories', component: CategoryStatsPageComponent },
      { path: 'loans', component: LoanTrendsPageComponent },
      { path: 'users', component: UserActivityPageComponent },
      { path: 'reservations', component: ReservationStatsPageComponent }
    ]
  }
];