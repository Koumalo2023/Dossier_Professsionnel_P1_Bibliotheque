import { Routes } from '@angular/router';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { BookListPageComponent } from './book-list-page/book-list-page.component';
import { BookDetailPageComponent } from './book-detail-page/book-detail-page.component';
import { AccessDeniedPageComponent } from './access-denied-page/access-denied-page.component';
import { PublicLayoutComponent } from './public-layout/public-layout.component';

export const publicRoutes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: LandingPageComponent },
      { path: 'books', component: BookListPageComponent },
      { path: 'books/:id', component: BookDetailPageComponent },
      { path: 'access-denied', component: AccessDeniedPageComponent }
    ]
  }
];