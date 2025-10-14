import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../../core/services/api/book.service';
import { BookGridComponent } from '../../../shared/components/organisms/book-grid/book-grid.component';
import { BookSearchComponent } from '../book-search/book-search.component';
import { Book } from '../../../core/models/book.model';

@Component({
  selector: 'app-book-list-page',
  standalone: true,
  imports: [CommonModule, FormsModule, BookGridComponent, BookSearchComponent],
  templateUrl: './book-list-page.component.html'
})
export class BookListPageComponent implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  
  // Filtres
  searchQuery: string = '';
  selectedCategory: string = 'all';
  selectedStatus: string = 'all';
  availabilityFilter: string = 'all';
  sortBy: string = 'title';
  sortOrder: 'asc' | 'desc' = 'asc';
  
  // Pagination
  currentPage: number = 1;
  itemsPerPage: number = 12;
  totalItems: number = 0;
  
  // États
  isLoading: boolean = false;
  hasError: boolean = false;
  
  // Catégories disponibles
  categories: string[] = [
    'Fiction',
    'Science-Fiction',
    'Fantasy',
    'Philosophie',
    'Histoire',
    'Biographie',
    'Science',
    'Technologie',
    'Art',
    'Musique'
  ];

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.isLoading = true;
    this.hasError = false;

    // Appel API réel
    this.bookService.getBooks().subscribe({
      next: (response: any) => {
        // L'API retourne un objet avec une propriété 'items' contenant les livres
        this.books = response.items || [];
        this.totalItems = this.books.length;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        this.hasError = true;
        this.isLoading = false;
        console.error('Erreur lors du chargement des livres:', error);
      }
    });
  }


  onSearch(query: string): void {
    this.searchQuery = query;
    this.currentPage = 1;
    this.applyFilters();
  }

  onCategoryChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  onStatusChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  onAvailabilityChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  onSortChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = this.books;

    // Filtre par recherche
    if (this.searchQuery) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(book =>
        book.title.toLowerCase().includes(query) ||
        book.author.toLowerCase().includes(query) ||
        book.isbn.toLowerCase().includes(query)
      );
    }

    // Filtre par catégorie
    if (this.selectedCategory !== 'all') {
      filtered = filtered.filter(book => book.category === this.selectedCategory);
    }

    // Filtre par statut (disponibilité)
    if (this.selectedStatus !== 'all') {
      if (this.selectedStatus === 'available') {
        filtered = filtered.filter(book => book.availableCopies > 0);
      } else if (this.selectedStatus === 'unavailable') {
        filtered = filtered.filter(book => book.availableCopies === 0);
      }
    }

    // Tri
    filtered.sort((a, b) => {
      let aValue: any = a[this.sortBy as keyof Book];
      let bValue: any = b[this.sortBy as keyof Book];
      
      if (this.sortBy === 'publicationDate') {
        aValue = new Date(aValue);
        bValue = new Date(bValue);
      }

      if (aValue < bValue) return this.sortOrder === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortOrder === 'asc' ? 1 : -1;
      return 0;
    });

    this.totalItems = filtered.length;
    
    // Pagination
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.filteredBooks = filtered.slice(startIndex, endIndex);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.applyFilters();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const totalPages = this.totalPages;
    const currentPage = this.currentPage;
    
    // Afficher maximum 5 pages autour de la page courante
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, currentPage + 2);
    
    // Ajuster si on est près du début
    if (currentPage <= 3) {
      endPage = Math.min(totalPages, 5);
    }
    
    // Ajuster si on est près de la fin
    if (currentPage >= totalPages - 2) {
      startPage = Math.max(1, totalPages - 4);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  getStatusText(book: Book): string {
    return book.availableCopies > 0 ? 'Disponible' : 'Indisponible';
  }

  getStatusClass(book: Book): string {
    return book.availableCopies > 0 ? 'status-available' : 'status-unavailable';
  }
}
