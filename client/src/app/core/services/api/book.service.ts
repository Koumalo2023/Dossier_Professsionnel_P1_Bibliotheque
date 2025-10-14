import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';


import { StorageService } from '../storage/storage.service';
import { EventBusService } from '../event-bus/event-bus.service';
import { ApiConfigService } from '../../config/api.config';
import { Book, BookStats, Category, CreateBookRequest, CreateCategoryRequest, GoogleBookPreview, ImportBookFromGoogleRequest, UpdateBookRequest, UpdateCategoryRequest } from '../../models/book.model';
import { PaginatedResponse } from '../../models/shared.model';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private http = inject(HttpClient);
  private storageService = inject(StorageService);
  private eventBus = inject(EventBusService);
  private apiConfig = inject(ApiConfigService);

  private readonly BOOKS_CACHE_KEY = 'books_cache';
  private readonly CATEGORIES_CACHE_KEY = 'categories_cache';

  // Gestion des livres
  getBooks(filters?: any): Observable<PaginatedResponse<Book>> {
    let params = new HttpParams();
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<PaginatedResponse<Book>>(
      this.apiConfig.buildBooksUrl('base'), 
      { params }
    ).pipe(
      tap(books => {
        this.cacheBooks(books.items);
      })
    );
  }

  getBookById(id: string): Observable<Book> {
    return this.http.get<Book>(
      this.apiConfig.buildBookByIdUrl(id)
    ).pipe(
      tap(book => {
        this.updateBookInCache(book);
      })
    );
  }

  searchBooks(query: string, filters?: any): Observable<PaginatedResponse<Book>> {
    let params = new HttpParams().set('query', query);
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<PaginatedResponse<Book>>(
      this.apiConfig.buildBooksUrl('search'), 
      { params }
    );
  }

  getBooksByCategory(categoryId: string): Observable<PaginatedResponse<Book>> {
    return this.http.get<PaginatedResponse<Book>>(
      `${this.apiConfig.buildBooksUrl('categories')}/${categoryId}`
    );
  }

  getRecentBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(
      this.apiConfig.buildBooksUrl('recent')
    ).pipe(
      tap(books => {
        this.cacheBooks(books);
      })
    );
  }

  getPopularBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(
      this.apiConfig.buildBooksUrl('popular')
    ).pipe(
      tap(books => {
        this.cacheBooks(books);
      })
    );
  }

  createBook(book: CreateBookRequest): Observable<Book> {
    return this.http.post<Book>(
      this.apiConfig.buildBooksUrl('create'), 
      book
    ).pipe(
      tap(newBook => {
        this.eventBus.emitBookCreated(newBook);
        this.addBookToCache(newBook);
      })
    );
  }

  updateBook(id: string, book: UpdateBookRequest): Observable<Book> {
    return this.http.put<Book>(
      this.apiConfig.buildBookByIdUrl(id), 
      book
    ).pipe(
      tap(updatedBook => {
        this.eventBus.emitBookUpdated(updatedBook);
        this.updateBookInCache(updatedBook);
      })
    );
  }

  deleteBook(id: string): Observable<void> {
    return this.http.delete<void>(
      this.apiConfig.buildBookByIdUrl(id)
    ).pipe(
      tap(() => {
        this.eventBus.emitBookDeleted(id);
        this.removeBookFromCache(id);
      })
    );
  }

  addBookCopies(id: string, copies: number): Observable<Book> {
    return this.http.post<Book>(
      `${this.apiConfig.buildBookByIdUrl(id)}/copies`, 
      { copies }
    ).pipe(
      tap(updatedBook => {
        this.eventBus.emitBookUpdated(updatedBook);
        this.updateBookInCache(updatedBook);
      })
    );
  }

  updateBookAvailability(id: string, available: boolean): Observable<Book> {
    return this.http.put<Book>(
      `${this.apiConfig.buildBookByIdUrl(id)}/availability`, 
      { available }
    ).pipe(
      tap(updatedBook => {
        this.eventBus.emitBookUpdated(updatedBook);
        this.updateBookInCache(updatedBook);
      })
    );
  }

  getBookStats(id: string): Observable<BookStats> {
    return this.http.get<BookStats>(
      `${this.apiConfig.buildBookByIdUrl(id)}/stats`
    );
  }

  // Importation depuis Google Books
  importFromGoogle(request: ImportBookFromGoogleRequest): Observable<Book> {
    return this.http.post<Book>(
      `${this.apiConfig.buildBooksUrl('import')}/google`,
      request
    ).pipe(
      tap(book => {
        this.eventBus.emitBookImported(book);
        this.addBookToCache(book);
      })
    );
  }

  searchGoogleBooks(query: string): Observable<GoogleBookPreview[]> {
    const params = new HttpParams()
      .set('q', query)
      .set('maxResults', '10');
    
    return this.http.get<{ items: GoogleBookPreview[] }>(
      `${this.apiConfig.buildBooksUrl('import')}/google/search`,
      { params }
    ).pipe(
      map(response => response.items)
    );
  }

  // Gestion des catégories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(
      this.apiConfig.buildBooksUrl('categories')
    ).pipe(
      tap(categories => {
        this.cacheCategories(categories);
      })
    );
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(
      `${this.apiConfig.buildBooksUrl('categories')}/${id}`
    );
  }

  createCategory(category: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(
      this.apiConfig.buildBooksUrl('categories'), 
      category
    ).pipe(
      tap(newCategory => {
        this.addCategoryToCache(newCategory);
      })
    );
  }

  updateCategory(id: string, category: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(
      `${this.apiConfig.buildBooksUrl('categories')}/${id}`, 
      category
    ).pipe(
      tap(updatedCategory => {
        this.updateCategoryInCache(updatedCategory);
      })
    );
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiConfig.buildBooksUrl('categories')}/${id}`
    ).pipe(
      tap(() => {
        this.removeCategoryFromCache(id);
      })
    );
  }

  // Méthodes de cache
  getCachedBooks(): Book[] {
    return this.storageService.getItem<Book[]>(this.BOOKS_CACHE_KEY) || [];
  }

  getCachedCategories(): Category[] {
    return this.storageService.getItem<Category[]>(this.CATEGORIES_CACHE_KEY) || [];
  }

  private cacheBooks(books: Book[]): void {
    const cachedBooks = this.getCachedBooks();
    const updatedBooks = [...cachedBooks];
    
    books.forEach(book => {
      const existingIndex = updatedBooks.findIndex(b => b.id === book.id);
      if (existingIndex >= 0) {
        updatedBooks[existingIndex] = book;
      } else {
        updatedBooks.push(book);
      }
    });
    
    this.storageService.setItem(this.BOOKS_CACHE_KEY, updatedBooks);
  }

  private addBookToCache(book: Book): void {
    const cachedBooks = this.getCachedBooks();
    const existingIndex = cachedBooks.findIndex(b => b.id === book.id);
    
    if (existingIndex >= 0) {
      cachedBooks[existingIndex] = book;
    } else {
      cachedBooks.push(book);
    }
    
    this.storageService.setItem(this.BOOKS_CACHE_KEY, cachedBooks);
  }

  private updateBookInCache(book: Book): void {
    const cachedBooks = this.getCachedBooks();
    const index = cachedBooks.findIndex(b => b.id === book.id);
    
    if (index >= 0) {
      cachedBooks[index] = book;
      this.storageService.setItem(this.BOOKS_CACHE_KEY, cachedBooks);
    }
  }

  private removeBookFromCache(bookId: string): void {
    const cachedBooks = this.getCachedBooks();
    const filteredBooks = cachedBooks.filter(b => b.id !== bookId);
    this.storageService.setItem(this.BOOKS_CACHE_KEY, filteredBooks);
  }

  private cacheCategories(categories: Category[]): void {
    this.storageService.setItem(this.CATEGORIES_CACHE_KEY, categories);
  }

  private addCategoryToCache(category: Category): void {
    const cachedCategories = this.getCachedCategories();
    const existingIndex = cachedCategories.findIndex(c => c.id === category.id);
    
    if (existingIndex >= 0) {
      cachedCategories[existingIndex] = category;
    } else {
      cachedCategories.push(category);
    }
    
    this.storageService.setItem(this.CATEGORIES_CACHE_KEY, cachedCategories);
  }

  private updateCategoryInCache(category: Category): void {
    const cachedCategories = this.getCachedCategories();
    const index = cachedCategories.findIndex(c => c.id === category.id);
    
    if (index >= 0) {
      cachedCategories[index] = category;
      this.storageService.setItem(this.CATEGORIES_CACHE_KEY, cachedCategories);
    }
  }

  private removeCategoryFromCache(categoryId: string): void {
    const cachedCategories = this.getCachedCategories();
    const filteredCategories = cachedCategories.filter(c => c.id !== categoryId);
    this.storageService.setItem(this.CATEGORIES_CACHE_KEY, filteredCategories);
  }

  // Nettoyage du cache
  clearBooksCache(): void {
    this.storageService.removeItem(this.BOOKS_CACHE_KEY);
  }

  clearCategoriesCache(): void {
    this.storageService.removeItem(this.CATEGORIES_CACHE_KEY);
  }

  clearAllCache(): void {
    this.clearBooksCache();
    this.clearCategoriesCache();
  }
}