import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from '../../config/api.config';
import { CreateReservationRequest, Reservation, UpdateReservationStatusRequest } from '../../models/reservation.model';
import { PaginatedResponse } from '../../models/shared.model';


@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private http = inject(HttpClient);
  private apiConfig = inject(ApiConfigService);

  // Gestion des réservations
  createReservation(reservationRequest: CreateReservationRequest): Observable<Reservation> {
    return this.http.post<Reservation>(
      this.apiConfig.buildReservationsUrl('create'),
      reservationRequest
    );
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
    return this.http.get<PaginatedResponse<Reservation>>(
      this.apiConfig.buildReservationsUrl('base'),
      { params }
    );
  }

  getReservationById(id: string): Observable<Reservation> {
    return this.http.get<Reservation>(this.apiConfig.buildReservationByIdUrl(id));
  }

  cancelReservation(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiConfig.buildReservationByIdUrl(id)}/cancel`
    );
  }

  canReserveBook(bookId: string): Observable<{ canReserve: boolean; reason?: string }> {
    return this.http.get<{ canReserve: boolean; reason?: string }>(
      `${this.apiConfig.buildReservationsUrl('base')}/can-reserve/${bookId}`
    );
  }

  getExpiredReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(
      `${this.apiConfig.buildReservationsUrl('base')}/expired`
    );
  }

  getAvailableReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(this.apiConfig.buildReservationsUrl('available'));
  }

  updateReservationStatus(id: string, statusRequest: UpdateReservationStatusRequest): Observable<Reservation> {
    return this.http.put<Reservation>(
      `${this.apiConfig.buildReservationByIdUrl(id)}/status`,
      statusRequest
    );
  }
}