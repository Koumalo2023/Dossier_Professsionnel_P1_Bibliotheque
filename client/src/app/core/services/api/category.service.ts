import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { StorageService } from '../storage/storage.service';
import { ApiConfigService } from '../../config/api.config';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../models/book.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private storageService = inject(StorageService);
  private apiConfig = inject(ApiConfigService);

  private readonly CATEGORIES_CACHE_KEY = 'categories_cache';

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
    ).pipe(
      tap(category => {
        this.updateCategoryInCache(category);
      })
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
  getCachedCategories(): Category[] {
    return this.storageService.getItem<Category[]>(this.CATEGORIES_CACHE_KEY) || [];
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
  clearCategoriesCache(): void {
    this.storageService.removeItem(this.CATEGORIES_CACHE_KEY);
  }
}