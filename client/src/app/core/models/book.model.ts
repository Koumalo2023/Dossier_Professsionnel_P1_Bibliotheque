// Modèles de livres

export interface Book {
  available: any;
  id: string;
  title: string;
  author: string;
  category: string;
  isbn: string;
  publicationDate?: string;
  coverUrl: string;
  availableCopies: number;
  totalCopies: number;
  description: string;
  pageCount?: number;
  publisher: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  category: string;
  isbn: string;
  publicationDate?: string;
  coverUrl: string;
  totalCopies: number;
  description: string;
  pageCount?: number;
  publisher: string;
}

export interface UpdateBookRequest {
  title?: string;
  author?: string;
  category?: string;
  isbn?: string;
  publicationDate?: string;
  coverUrl?: string;
  description?: string;
  pageCount?: number;
  publisher?: string;
}

export interface BookStats {
  totalLoans: number;
  activeLoans: number;
  totalReservations: number;
  popularityScore: number;
}

export interface ImportBookFromGoogleRequest {
  isbn: string;
  totalCopies: number;
  defaultCategory: string;
}

export interface GoogleBookPreview {
  title: string;
  authors: string[];
  publishedDate: string;
  description: string;
  isbn: string;
  coverUrl: string;
  categories: string[];
  pageCount?: number;
  publisher: string;
  googleBooksId: string;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  bookCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
}

export interface UpdateCategoryRequest {
  name?: string;
  description?: string;
}