import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from '../../config/api.config';
import { ActiveUser, AnalyticsOverview, AnalyticsPeriod, AuditLog, BorrowTrend, PopularCategory, ReservationStats, TopBook } from '../../models/analytics.model';


@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private http = inject(HttpClient);
  private apiConfig = inject(ApiConfigService);

  // Vue d'ensemble
  getOverview(): Observable<AnalyticsOverview> {
    return this.http.get<AnalyticsOverview>(this.apiConfig.buildAnalyticsUrl('dashboard'));
  }

  // Catégories populaires
  getPopularCategories(period?: AnalyticsPeriod): Observable<PopularCategory[]> {
    let params = new HttpParams();
    if (period) {
      params = params.set('period', period);
    }
    return this.http.get<PopularCategory[]>(this.apiConfig.buildAnalyticsUrl('popularBooks'), { params });
  }

  // Tendances des emprunts
  getBorrowTrends(period?: AnalyticsPeriod): Observable<BorrowTrend[]> {
    let params = new HttpParams();
    if (period) {
      params = params.set('period', period);
    }
    return this.http.get<BorrowTrend[]>(this.apiConfig.buildAnalyticsUrl('loans'), { params });
  }

  // Statistiques des réservations
  getReservationStats(): Observable<ReservationStats> {
    return this.http.get<ReservationStats>(`${this.apiConfig.buildAnalyticsUrl('base')}/reservations/stats`);
  }

  // Livres les plus empruntés
  getTopBooks(limit?: number): Observable<TopBook[]> {
    let params = new HttpParams();
    if (limit) {
      params = params.set('limit', limit.toString());
    }
    return this.http.get<TopBook[]>(this.apiConfig.buildAnalyticsUrl('books'), { params });
  }

  // Utilisateurs actifs
  getActiveUsers(limit?: number): Observable<ActiveUser[]> {
    let params = new HttpParams();
    if (limit) {
      params = params.set('limit', limit.toString());
    }
    return this.http.get<ActiveUser[]>(this.apiConfig.buildAnalyticsUrl('users'), { params });
  }

  // Logs d'audit
  getAuditLogs(filters?: any): Observable<AuditLog[]> {
    let params = new HttpParams();
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== undefined && filters[key] !== null) {
          params = params.set(key, filters[key].toString());
        }
      });
    }
    return this.http.get<AuditLog[]>(`${this.apiConfig.buildAnalyticsUrl('base')}/audit/logs`, { params });
  }
}