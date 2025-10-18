import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateUserCategoryPreferenceRequest, CreateUserProfileRequest, CreateUserReadingGoalRequest, CreateUserReadingHistoryRequest, UpdateReadingGoalProgressRequest, UpdateUserProfileRequest, UpdateUserReadingGoalRequest, UpdateUserReadingHistoryRequest, UserCategoryPreference, UserProfile, UserReadingGoal, UserReadingHistory, UserReadingStats } from '../../models/user-profile.model';
import { ApiConfigService } from '../../config/api.config';


@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  private http = inject(HttpClient);
  private apiConfig = inject(ApiConfigService);

  // Gestion du profil utilisateur
  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(this.apiConfig.buildUsersUrl('profile'));
  }

  createUserProfile(profile: CreateUserProfileRequest): Observable<UserProfile> {
    return this.http.post<UserProfile>(this.apiConfig.buildUsersUrl('base'), profile);
  }

  updateUserProfile(profile: UpdateUserProfileRequest): Observable<UserProfile> {
    return this.http.put<UserProfile>(this.apiConfig.buildUsersUrl('updateProfile'), profile);
  }

  deleteUserProfile(): Observable<void> {
    return this.http.delete<void>(this.apiConfig.buildUsersUrl('base'));
  }

  initializeUserProfile(): Observable<void> {
    return this.http.post<void>(`${this.apiConfig.buildUsersUrl('base')}/initialize`, {});
  }

  // Préférences de catégorie
  getCategoryPreferences(): Observable<UserCategoryPreference[]> {
    return this.http.get<UserCategoryPreference[]>(this.apiConfig.buildPreferencesUrl('categories'));
  }

  addOrUpdateCategoryPreference(preference: CreateUserCategoryPreferenceRequest): Observable<UserCategoryPreference> {
    return this.http.post<UserCategoryPreference>(this.apiConfig.buildPreferencesUrl('categories'), preference);
  }

  removeCategoryPreference(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiConfig.buildPreferencesUrl('categories')}/${categoryId}`);
  }

  // Objectifs de lecture
  getReadingGoals(activeOnly?: boolean): Observable<UserReadingGoal[]> {
    let params = new HttpParams();
    if (activeOnly !== undefined) {
      params = params.set('activeOnly', activeOnly.toString());
    }
    return this.http.get<UserReadingGoal[]>(this.apiConfig.buildUsersUrl('readingGoals'), { params });
  }

  getReadingGoalById(goalId: string): Observable<UserReadingGoal> {
    return this.http.get<UserReadingGoal>(`${this.apiConfig.buildUsersUrl('readingGoals')}/${goalId}`);
  }

  createReadingGoal(goal: CreateUserReadingGoalRequest): Observable<UserReadingGoal> {
    return this.http.post<UserReadingGoal>(this.apiConfig.buildUsersUrl('readingGoals'), goal);
  }

  updateReadingGoal(goalId: string, goal: UpdateUserReadingGoalRequest): Observable<UserReadingGoal> {
    return this.http.put<UserReadingGoal>(`${this.apiConfig.buildUsersUrl('readingGoals')}/${goalId}`, goal);
  }

  updateReadingGoalProgress(goalId: string, progress: UpdateReadingGoalProgressRequest): Observable<UserReadingGoal> {
    return this.http.put<UserReadingGoal>(`${this.apiConfig.buildUsersUrl('readingGoals')}/${goalId}/progress`, progress);
  }

  deleteReadingGoal(goalId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiConfig.buildUsersUrl('readingGoals')}/${goalId}`);
  }

  // Historique de lecture
  getReadingHistory(limit?: number): Observable<UserReadingHistory[]> {
    let params = new HttpParams();
    if (limit) {
      params = params.set('limit', limit.toString());
    }
    return this.http.get<UserReadingHistory[]>(this.apiConfig.buildUsersUrl('readingHistory'), { params });
  }

  getReadingHistoryByLoan(loanId: string): Observable<UserReadingHistory> {
    return this.http.get<UserReadingHistory>(`${this.apiConfig.buildUsersUrl('readingHistory')}/loan/${loanId}`);
  }

  createReadingHistory(history: CreateUserReadingHistoryRequest): Observable<UserReadingHistory> {
    return this.http.post<UserReadingHistory>(this.apiConfig.buildUsersUrl('readingHistory'), history);
  }

  updateReadingHistory(historyId: string, history: UpdateUserReadingHistoryRequest): Observable<UserReadingHistory> {
    return this.http.put<UserReadingHistory>(`${this.apiConfig.buildUsersUrl('readingHistory')}/${historyId}`, history);
  }

  deleteReadingHistory(historyId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiConfig.buildUsersUrl('readingHistory')}/${historyId}`);
  }

  // Statistiques de lecture
  getReadingStats(): Observable<UserReadingStats> {
    return this.http.get<UserReadingStats>(`${this.apiConfig.buildUsersUrl('base')}/stats`);
  }
}