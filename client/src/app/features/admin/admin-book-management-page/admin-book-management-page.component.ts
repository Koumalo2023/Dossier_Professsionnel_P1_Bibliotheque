import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BookService } from '../../../core/services/api/book.service';
import { Book, Category } from '../../../core/models/book.model';
import { ToastService } from '../../../core/services/toast.service';
import { BookGridComponent } from '../../../shared/components/organisms/book-grid/book-grid.component';
import { BookCardComponent } from '../../../shared/components/molecules/book-card/book-card.component';
import { ModalComponent } from '../../../shared/components/organisms/modal/modal.component';
import { ButtonComponent } from '../../../shared/components/atoms/button/button.component';
import { IconComponent } from '../../../shared/components/atoms/icons/icon.component';
import { TypographyComponent } from '../../../shared/components/atoms/typography/typography.component';
import { InputComponent } from '../../../shared/components/atoms/inputs/input.component';

@Component({
  selector: 'app-admin-book-management-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    BookGridComponent,
    BookCardComponent,
    ModalComponent,
    ButtonComponent,
    IconComponent,
    TypographyComponent,
    InputComponent
  ],
  templateUrl: './admin-book-management-page.component.html',
  styleUrls: ['./admin-book-management-page.component.scss']
})
export class AdminBookManagementPageComponent implements OnInit {
  private bookService = inject(BookService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  // États des données
  books: Book[] = [];
  categories: Category[] = [];
  loading = false;

  // États de l'interface
  searchQuery = '';

  ngOnInit(): void {
    this.loadBooks();
    this.loadCategories();
  }

  // Chargement des données
  loadBooks(): void {
    this.loading = true;
    
    this.bookService.getBooks().subscribe({
      next: (response: any) => {
        // L'API retourne un objet avec une propriété 'items' contenant les livres
        this.books = response.items || [];
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error('Erreur lors du chargement des livres');
        this.loading = false;
        console.error('Erreur chargement livres:', error);
      }
    });
  }

  loadCategories(): void {
    this.bookService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Erreur chargement catégories:', error);
        // En cas d'erreur, initialiser avec des catégories par défaut
        this.categories = [
          { id: '1', name: 'Fiction', description: 'Livres de fiction', icon: '📚', bookCount: 0, createdAt: '', updatedAt: '' },
          { id: '2', name: 'Science-Fiction', description: 'Science-fiction', icon: '🚀', bookCount: 0, createdAt: '', updatedAt: '' },
          { id: '3', name: 'Fantasy', description: 'Fantasy', icon: '🧙', bookCount: 0, createdAt: '', updatedAt: '' },
          { id: '4', name: 'Philosophie', description: 'Philosophie', icon: '🧠', bookCount: 0, createdAt: '', updatedAt: '' },
          { id: '5', name: 'Histoire', description: 'Histoire', icon: '📜', bookCount: 0, createdAt: '', updatedAt: '' }
        ];
      }
    });
  }

  // Recherche
  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.loadBooks();
      return;
    }

    this.loading = true;
    this.bookService.searchBooks(this.searchQuery).subscribe({
      next: (response: any) => {
        // L'API retourne un objet avec une propriété 'items' contenant les livres
        this.books = response.items || [];
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error('Erreur lors de la recherche');
        this.loading = false;
        console.error('Erreur recherche:', error);
      }
    });
  }

  // Redirection vers BookEditComponent
  navigateToCreateBook(): void {
    this.router.navigate(['/admin/book-edit']);
  }

  navigateToCreateBookWithGoogle(): void {
    this.router.navigate(['/admin/book-edit'], { 
      queryParams: { mode: 'google' } 
    });
  }

  navigateToEditBook(book: Book): void {
    console.log('Navigating to edit book:', book.id);
    this.router.navigate(['/admin/book-edit', book.id]);
  }

  // Suppression de livre
  deleteBook(book: Book): void {
    this.loading = true;
    
    this.bookService.deleteBook(book.id).subscribe({
      next: () => {
        this.books = this.books.filter(b => b.id !== book.id);
        this.loading = false;
        this.toastService.success('Livre supprimé avec succès');
      },
      error: (error) => {
        this.toastService.error('Erreur lors de la suppression du livre');
        this.loading = false;
        console.error('Erreur suppression livre:', error);
      }
    });
  }


  // Actions sur les livres
  onAddCopies(bookId: string): void {
    const book = this.books.find(b => b.id === bookId);
    if (!book) return;
    
    const copies = prompt('Nombre d\'exemplaires à ajouter:', '1');
    if (copies && !isNaN(Number(copies)) && Number(copies) > 0) {
      this.loading = true;
      this.bookService.addBookCopies(bookId, Number(copies)).subscribe({
        next: (updatedBook) => {
          // Mettre à jour le livre dans la liste
          const index = this.books.findIndex(b => b.id === bookId);
          if (index !== -1) {
            this.books[index] = updatedBook;
          }
          this.loading = false;
          this.toastService.success(`${copies} exemplaire(s) ajouté(s) avec succès`);
        },
        error: (error) => {
          this.toastService.error('Erreur lors de l\'ajout d\'exemplaires');
          this.loading = false;
          console.error('Erreur ajout exemplaires:', error);
        }
      });
    }
  }

  onToggleAvailability(bookId: string): void {
    const book = this.books.find(b => b.id === bookId);
    if (!book) return;
    
    const newAvailability = !book.available;
    this.loading = true;
    this.bookService.updateBookAvailability(bookId, newAvailability).subscribe({
      next: (updatedBook) => {
        // Mettre à jour le livre dans la liste
        const index = this.books.findIndex(b => b.id === bookId);
        if (index !== -1) {
          this.books[index] = updatedBook;
        }
        this.loading = false;
        this.toastService.success(`Livre ${newAvailability ? 'activé' : 'désactivé'} avec succès`);
      },
      error: (error) => {
        this.toastService.error('Erreur lors de la modification de disponibilité');
        this.loading = false;
        console.error('Erreur modification disponibilité:', error);
      }
    });
  }
}
