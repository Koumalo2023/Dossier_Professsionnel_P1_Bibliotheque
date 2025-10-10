import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Loan, 
  CreateLoanRequest, 
  ExtendLoanRequest, 
  LoanStats 
} from '../../models/loans/loan.model';
import { PaginatedResponse } from '../../models/shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = '/api/loans';

  constructor(private http: HttpClient) {}

  // Gestion des emprunts
  getLoans(filters?: any): Observable<PaginatedResponse<Loan>> {
    let params = new HttpParams();
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<PaginatedResponse<Loan>>(this.apiUrl, { params });
  }

  getOverdueLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.apiUrl}/overdue`);
  }

  getUserLoans(userId: string): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.apiUrl}/user/${userId}`);
  }

  createLoan(loanRequest: CreateLoanRequest): Observable<Loan> {
    return this.http.post<Loan>(this.apiUrl, loanRequest);
  }

  getLoanById(id: string): Observable<Loan> {
    return this.http.get<Loan>(`${this.apiUrl}/${id}`);
  }

  returnLoan(id: string): Observable<Loan> {
    return this.http.put<Loan>(`${this.apiUrl}/${id}/return`, {});
  }

  extendLoan(id: string, extendRequest: ExtendLoanRequest): Observable<Loan> {
    return this.http.put<Loan>(`${this.apiUrl}/${id}/extend`, extendRequest);
  }

  notifyOverdueLoan(id: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/notify-overdue`, {});
  }

  canBorrowBook(bookId: string): Observable<{ canBorrow: boolean; reason?: string }> {
    return this.http.get<{ canBorrow: boolean; reason?: string }>(`${this.apiUrl}/can-borrow/${bookId}`);
  }

  getLoanStats(): Observable<LoanStats> {
    return this.http.get<LoanStats>(`${this.apiUrl}/stats`);
  }
}