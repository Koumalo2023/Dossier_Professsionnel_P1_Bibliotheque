
export interface Book {
    id: string;
    title: string;
    author: string;
    category: string;
    isbn: string;
    publicationDate?: Date;
    coverUrl?: string;
    availableCopies: number;
    totalCopies: number;
    createdAt: Date;
    updatedAt: Date;
  }
  
  // DTO pour la création d'un nouveau livre
  export interface CreateBookDto {
    title: string;
    author: string;
    category: string;
    isbn: string;
    publicationDate?: Date;
    coverUrl?: string;
    totalCopies: number;
  }
  
  // DTO pour la mise à jour d'un livre
  export interface UpdateBookDto {
    title: string;
    author: string;
    category: string;
    publicationDate?: Date;
    coverUrl?: string;
    totalCopies: number;
  }