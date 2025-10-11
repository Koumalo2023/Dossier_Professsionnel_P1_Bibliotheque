// Modèles d'emprunts

export interface Loan {
  id: string;
  bookId: string;
  userId: string;
  bookTitle: string;
  userName: string;
  borrowDate: string;
  dueDate: string;
  returnDate?: string;
  status: LoanStatus;
  canExtend: boolean;
}

export interface CreateLoanRequest {
  bookId: string;
}

export interface ExtendLoanRequest {
  additionalDays: number;
}

export interface LoanStats {
  totalLoans: number;
  activeLoans: number;
  overdueLoans: number;
  favoriteCategory: string;
}

// Statuts d'emprunt
export enum LoanStatus {
  ACTIVE = 'ACTIVE',
  RETURNED = 'RETURNED',
  OVERDUE = 'OVERDUE'
}