import { Routes } from '@angular/router';
import { BookListComponent } from './book-list/book-list.component';
import { BookDetailsComponent } from './book-details/book-details.component';
import { BookSearchComponent } from './book-search/book-search.component';
import { BookImportComponent } from './book-import/book-import.component';
import { BooksLayoutComponent } from './books-layout/books-layout.component';
import { roleGuard } from '../../core/guards/role.guard';

export const booksRoutes: Routes = [
  {
    path: '',
    component: BooksLayoutComponent,
    children: [
      { path: '', component: BookListComponent },
      { path: 'search', component: BookSearchComponent },
      {
        path: 'import',
        component: BookImportComponent,
        canActivate: [roleGuard],
        data: { roles: ['Manager', 'Admin'] }
      },
      { path: ':id', component: BookDetailsComponent }
    ]
  }
];