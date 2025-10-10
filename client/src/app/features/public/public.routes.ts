import { Routes } from '@angular/router';
import { LandingPageComponent } from './landing-page/landing-page.component'; 
import { AccessDeniedPageComponent } from './access-denied-page/access-denied-page.component';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { BookListPageComponent } from '../books/book-list/book-list-page.component';
import { BookDetailsPageComponent } from '../books/book-details/book-details-page.component';

export const publicRoutes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: LandingPageComponent },
      { path: 'books', component: BookListPageComponent },
      { path: 'books/:id', component: BookDetailsPageComponent },
      { path: 'access-denied', component: AccessDeniedPageComponent }
    ]
  }
];