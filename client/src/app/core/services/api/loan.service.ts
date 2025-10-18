import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateLoanRequest, ExtendLoanRequest, Loan, LoanStats } from '../../models/loan.model';
import { PaginatedResponse } from '../../models/shared.model';
import { ApiConfigService } from '../../config/api.config';
 

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private http = inject(HttpClient);
  private apiConfig = inject(ApiConfigService);

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
    return this.http.get<PaginatedResponse<Loan>>(
      this.apiConfig.buildLoansUrl('base'),
      { params }
    );
  }

  getOverdueLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(this.apiConfig.buildLoansUrl('overdue'));
  }

  getUserLoans(userId: string): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.apiConfig.buildLoansUrl('userLoans')}/${userId}`);
  }

  createLoan(loanRequest: CreateLoanRequest): Observable<Loan> {
    return this.http.post<Loan>(
      this.apiConfig.buildLoansUrl('borrow'),
      loanRequest
    );
  }

  getLoanById(id: string): Observable<Loan> {
    return this.http.get<Loan>(this.apiConfig.buildLoanByIdUrl(id));
  }

  returnLoan(id: string): Observable<Loan> {
    return this.http.put<Loan>(
      `${this.apiConfig.buildLoanByIdUrl(id)}/return`,
      {}
    );
  }

  extendLoan(id: string, extendRequest: ExtendLoanRequest): Observable<Loan> {
    return this.http.put<Loan>(
      `${this.apiConfig.buildLoanByIdUrl(id)}/extend`,
      extendRequest
    );
  }

  notifyOverdueLoan(id: string): Observable<void> {
    return this.http.put<void>(
      `${this.apiConfig.buildLoanByIdUrl(id)}/notify-overdue`,
      {}
    );
  }

  canBorrowBook(bookId: string): Observable<{ canBorrow: boolean; reason?: string }> {
    return this.http.get<{ canBorrow: boolean; reason?: string }>(
      `${this.apiConfig.buildLoansUrl('base')}/can-borrow/${bookId}`
    );
  }

  getLoanStats(): Observable<LoanStats> {
    return this.http.get<LoanStats>(`${this.apiConfig.buildLoansUrl('base')}/stats`);
  }
}