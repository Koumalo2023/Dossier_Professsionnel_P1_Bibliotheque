import { Routes } from '@angular/router';

// Composants publics
import { LandingPageComponent } from './features/public/landing-page/landing-page.component';
import { BookListPageComponent } from './features/public/book-list-page/book-list-page.component';
import { BookDetailPageComponent } from './features/public/book-detail-page/book-detail-page.component';
import { AccessDeniedPageComponent } from './features/public/access-denied-page/access-denied-page.component';

// Composants d'authentification
import { LoginPageComponent } from './features/auth/login-page/login-page.component';
import { RegisterPageComponent } from './features/auth/register-page/register-page.component';


// Composants utilisateur
import { ProfilePageComponent } from './features/user/profile-page/profile-page.component';
import { UserLoansPageComponent } from './features/user/user-loans-page/user-loans-page.component';

// Composants administrateur
import { AdminBookFormPageComponent } from './features/admin/admin-book-form-page/admin-book-form-page.component';
import { AdminLoansPageComponent } from './features/admin/admin-loans-page/admin-loans-page.component';
import { AdminStatisticsPageComponent } from './features/admin/admin-statistics-page/admin-statistics-page.component'; 
import { roleGuard } from './core/guards/role.guard';
// Composants utilisateur

// Composants administrateur
export const routes: Routes = [
    // Routes publiques
    { path: '', component: LandingPageComponent },
    { path: 'books', component: BookListPageComponent },
    { path: 'books/:id', component: BookDetailPageComponent },
    { path: 'access-denied', component: AccessDeniedPageComponent },
    
    // Routes d'authentification
    { path: 'login', component: LoginPageComponent },
    { path: 'register', component: RegisterPageComponent },
    
    // Routes utilisateur (protégées par authGuard)
    { 
      path: 'profile', 
      component: ProfilePageComponent,
       canActivate: [roleGuard(['Manager', 'User', 'Admin'])],
    },
    { 
      path: 'profile/loans', 
      component: UserLoansPageComponent,
      canActivate: [roleGuard(['Manager', 'User', 'Admin'])],
    },
    
    // Routes administrateur (protégées par adminGuard)
    { 
      path: 'admin/books/new', 
      component: AdminBookFormPageComponent,
      canActivate: [roleGuard(['Manager', 'Admin'])],
    },
    { 
      path: 'admin/books/:id/edit', 
      component: AdminBookFormPageComponent,
      canActivate: [roleGuard(['Manager', 'Admin'])],
    },
    { 
      path: 'admin/loans', 
      component: AdminLoansPageComponent,
      canActivate: [roleGuard(['Manager', 'Admin'])],
    },
    { 
      path: 'admin/statistics', 
      component: AdminStatisticsPageComponent,
      canActivate: [roleGuard(['Manager', 'Admin'])],
    },
    
    // Route par défaut (redirection vers la page d'accueil)
    { path: '**', redirectTo: '' }
  ];
