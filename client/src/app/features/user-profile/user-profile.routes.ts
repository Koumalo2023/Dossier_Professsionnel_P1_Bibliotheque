import { Routes } from '@angular/router';
import { ProfilePageComponent } from './profile-page/profile-page.component';
import { PreferencesPageComponent } from './preferences-page/preferences-page.component';
import { ReadingGoalsPageComponent } from './reading-goals-page/reading-goals-page.component';
import { ReadingHistoryPageComponent } from './reading-history-page/reading-history-page.component';
import { UserProfileLayoutComponent } from './user-profile-layout/user-profile-layout.component';

export const userProfileRoutes: Routes = [
  {
    path: '',
    component: UserProfileLayoutComponent,
    children: [
      { path: '', component: ProfilePageComponent },
      { path: 'preferences', component: PreferencesPageComponent },
      { path: 'reading-goals', component: ReadingGoalsPageComponent },
      { path: 'reading-history', component: ReadingHistoryPageComponent }
    ]
  }
];