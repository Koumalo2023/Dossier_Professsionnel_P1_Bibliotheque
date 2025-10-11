import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  // Route par défaut - Redirection vers la page d'accueil
  { path: '', redirectTo: 'books', pathMatch: 'full' },

  // Routes publiques
  { 
    path: 'books', 
    loadChildren: () => import('./features/books/books.routes').then(m => m.booksRoutes)
  },
  { 
    path: 'home', 
    loadChildren: () => import('./features/public/public.routes').then(m => m.publicRoutes)
  },
  { 
    path: 'auth', 
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
  },

  // Routes utilisateur (protégées par authentification)
  { 
    path: 'profile', 
    loadChildren: () => import('./features/user-profile/user-profile.routes').then(m => m.userProfileRoutes),
    canActivate: [authGuard]
  },
  { 
    path: 'loans', 
    loadChildren: () => import('./features/loans/loans.routes').then(m => m.loansRoutes),
    canActivate: [authGuard]
  },
  { 
    path: 'reservations', 
    loadChildren: () => import('./features/reservations/reservations.routes').then(m => m.reservationsRoutes),
    canActivate: [authGuard]
  },
  { 
    path: 'notifications', 
    loadChildren: () => import('./features/notifications/notifications.routes').then(m => m.notificationsRoutes),
    canActivate: [authGuard]
  },

  // Routes analytiques (protégées par rôle Manager/Admin)
  { 
    path: 'analytics', 
    loadChildren: () => import('./features/analytics/analytics.routes').then(m => m.analyticsRoutes),
    canActivate: [roleGuard],
    data: { roles: ['Manager', 'Admin'] }
  },

  // Routes administration (protégées par rôle admin ou manager)
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes),
    canActivate: [roleGuard(['Admin', 'Manager'])]
  },

  // Pages d'erreur
  { 
    path: 'access-denied', 
    loadChildren: () => import('./features/public/public.routes').then(m => m.publicRoutes)
  },

  // Route de secours - Redirection vers la liste des livres
  { path: '**', redirectTo: 'home' }
];
