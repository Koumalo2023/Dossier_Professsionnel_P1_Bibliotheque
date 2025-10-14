import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { GoogleBookPreview } from '../../models/book.model';

@Injectable({
  providedIn: 'root'
})
export class GoogleBooksService {
  private apiUrl = 'https://www.googleapis.com/books/v1/volumes';

  constructor(private http: HttpClient) {}

  searchBooks(query: string): Observable<GoogleBookPreview[]> {
    // Simulation de données - à remplacer par un appel API réel
    return of(this.generateMockGoogleBooks(query));
  }

  private generateMockGoogleBooks(query: string): GoogleBookPreview[] {
    const mockBooks: GoogleBookPreview[] = [
      {
        title: 'Le Petit Prince',
        authors: ['Antoine de Saint-Exupéry'],
        publishedDate: '1943-04-06',
        description: 'Un conte poétique et philosophique sous l\'apparence d\'un conte pour enfants.',
        isbn: '978-2-07-040000-0',
        coverUrl: 'https://via.placeholder.com/150x200?text=Le+Petit+Prince',
        categories: ['Fiction', 'Classique'],
        pageCount: 96,
        publisher: 'Gallimard',
        googleBooksId: 'google1'
      },
      {
        title: '1984',
        authors: ['George Orwell'],
        publishedDate: '1949-06-08',
        description: 'Une dystopie sur un régime totalitaire et la surveillance de masse.',
        isbn: '978-2-07-036000-0',
        coverUrl: 'https://via.placeholder.com/150x200?text=1984',
        categories: ['Science-Fiction', 'Dystopie'],
        pageCount: 328,
        publisher: 'Gallimard',
        googleBooksId: 'google2'
      },
      {
        title: 'L\'Étranger',
        authors: ['Albert Camus'],
        publishedDate: '1942-01-01',
        description: 'Roman existentialiste sur l\'absurdité de la condition humaine.',
        isbn: '978-2-07-036002-0',
        coverUrl: 'https://via.placeholder.com/150x200?text=L+Etranger',
        categories: ['Philosophie', 'Roman'],
        pageCount: 185,
        publisher: 'Gallimard',
        googleBooksId: 'google3'
      }
    ];

    // Filtrer par requête de recherche
    if (query) {
      const lowerQuery = query.toLowerCase();
      return mockBooks.filter(book => 
        book.title.toLowerCase().includes(lowerQuery) ||
        book.authors.some(author => author.toLowerCase().includes(lowerQuery)) ||
        book.categories.some(category => category.toLowerCase().includes(lowerQuery))
      );
    }

    return mockBooks;
  }

  importBook(book: GoogleBookPreview, totalCopies: number, defaultCategory: string): Observable<any> {
    // Simulation d'import - à remplacer par un appel API réel
    console.log('Importing book:', book, 'with copies:', totalCopies, 'category:', defaultCategory);
    return of({ success: true, message: 'Livre importé avec succès' });
  }
}