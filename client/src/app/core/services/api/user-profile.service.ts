import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UserProfile,
  CreateUserProfileRequest,
  UpdateUserProfileRequest,
  UserCategoryPreference,
  CreateUserCategoryPreferenceRequest,
  UserReadingGoal,
  CreateUserReadingGoalRequest,
  UpdateUserReadingGoalRequest,
  UpdateReadingGoalProgressRequest,
  UserReadingHistory,
  CreateUserReadingHistoryRequest,
  UpdateUserReadingHistoryRequest,
  UserReadingStats
} from '../../models/user-profile/user-profile.model';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  private apiUrl = '/api/userprofile';

  constructor(private http: HttpClient) {}

  // Gestion du profil utilisateur
  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(this.apiUrl);
  }

  createUserProfile(profile: CreateUserProfileRequest): Observable<UserProfile> {
    return this.http.post<UserProfile>(this.apiUrl, profile);
  }

  updateUserProfile(profile: UpdateUserProfileRequest): Observable<UserProfile> {
    return this.http.put<UserProfile>(this.apiUrl, profile);
  }

  deleteUserProfile(): Observable<void> {
    return this.http.delete<void>(this.apiUrl);
  }

  initializeUserProfile(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/initialize`, {});
  }

  // Préférences de catégorie
  getCategoryPreferences(): Observable<UserCategoryPreference[]> {
    return this.http.get<UserCategoryPreference[]>(`${this.apiUrl}/preferences/categories`);
  }

  addOrUpdateCategoryPreference(preference: CreateUserCategoryPreferenceRequest): Observable<UserCategoryPreference> {
    return this.http.post<UserCategoryPreference>(`${this.apiUrl}/preferences/categories`, preference);
  }

  removeCategoryPreference(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/preferences/categories/${categoryId}`);
  }

  // Objectifs de lecture
  getReadingGoals(activeOnly?: boolean): Observable<UserReadingGoal[]> {
    let params = new HttpParams();
    if (activeOnly !== undefined) {
      params = params.set('activeOnly', activeOnly.toString());
    }
    return this.http.get<UserReadingGoal[]>(`${this.apiUrl}/goals`, { params });
  }

  getReadingGoalById(goalId: string): Observable<UserReadingGoal> {
    return this.http.get<UserReadingGoal>(`${this.apiUrl}/goals/${goalId}`);
  }

  createReadingGoal(goal: CreateUserReadingGoalRequest): Observable<UserReadingGoal> {
    return this.http.post<UserReadingGoal>(`${this.apiUrl}/goals`, goal);
  }

  updateReadingGoal(goalId: string, goal: UpdateUserReadingGoalRequest): Observable<UserReadingGoal> {
    return this.http.put<UserReadingGoal>(`${this.apiUrl}/goals/${goalId}`, goal);
  }

  updateReadingGoalProgress(goalId: string, progress: UpdateReadingGoalProgressRequest): Observable<UserReadingGoal> {
    return this.http.put<UserReadingGoal>(`${this.apiUrl}/goals/${goalId}/progress`, progress);
  }

  deleteReadingGoal(goalId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/goals/${goalId}`);
  }

  // Historique de lecture
  getReadingHistory(limit?: number): Observable<UserReadingHistory[]> {
    let params = new HttpParams();
    if (limit) {
      params = params.set('limit', limit.toString());
    }
    return this.http.get<UserReadingHistory[]>(`${this.apiUrl}/history`, { params });
  }

  getReadingHistoryByLoan(loanId: string): Observable<UserReadingHistory> {
    return this.http.get<UserReadingHistory>(`${this.apiUrl}/history/loan/${loanId}`);
  }

  createReadingHistory(history: CreateUserReadingHistoryRequest): Observable<UserReadingHistory> {
    return this.http.post<UserReadingHistory>(`${this.apiUrl}/history`, history);
  }

  updateReadingHistory(historyId: string, history: UpdateUserReadingHistoryRequest): Observable<UserReadingHistory> {
    return this.http.put<UserReadingHistory>(`${this.apiUrl}/history/${historyId}`, history);
  }

  deleteReadingHistory(historyId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/history/${historyId}`);
  }

  // Statistiques de lecture
  getReadingStats(): Observable<UserReadingStats> {
    return this.http.get<UserReadingStats>(`${this.apiUrl}/stats`);
  }
}