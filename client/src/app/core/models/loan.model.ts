
export interface Loan {
    id: string;
    userId: string;
    bookId: string;
    loanDate: Date;
    dueDate: Date;
    returnDate?: Date;
    status: string;
  }
  
  // DTO pour la création d'un nouvel emprunt
  export interface CreateLoanDto {
    bookId: string;
    dueDate: Date;
  }
  
  // DTO pour la mise à jour d'un emprunt
  export interface UpdateLoanDto {
    returnDate?: Date;
    status?: string;
  }