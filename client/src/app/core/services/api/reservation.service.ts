import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Reservation, 
  CreateReservationRequest, 
  UpdateReservationStatusRequest 
} from '../../models/reservations/reservation.model';
import { PaginatedResponse } from '../../models/shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private apiUrl = '/api/reservations';

  constructor(private http: HttpClient) {}

  // Gestion des réservations
  createReservation(reservationRequest: CreateReservationRequest): Observable<Reservation> {
    return this.http.post<Reservation>(this.apiUrl, reservationRequest);
  }

  getReservations(filters?: any): Observable<PaginatedResponse<Reservation>> {
    let params = new HttpParams();
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<PaginatedResponse<Reservation>>(this.apiUrl, { params });
  }

  getReservationById(id: string): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`);
  }

  cancelReservation(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  canReserveBook(bookId: string): Observable<{ canReserve: boolean; reason?: string }> {
    return this.http.get<{ canReserve: boolean; reason?: string }>(`${this.apiUrl}/can-reserve/${bookId}`);
  }

  getExpiredReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/expired`);
  }

  getAvailableReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/available`);
  }

  updateReservationStatus(id: string, statusRequest: UpdateReservationStatusRequest): Observable<Reservation> {
    return this.http.put<Reservation>(`${this.apiUrl}/${id}/status`, statusRequest);
  }
}