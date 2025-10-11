import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { StorageService } from '../storage/storage.service';
import { EventBusService } from '../event-bus/event-bus.service';
import { ApiConfigService } from '../../config/api.config';
import { LoginRequest, LoginResponse, RegisterRequest, UpdateUserRequest, User } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isAdmin(): boolean {
    const user = this.getStoredUser();
    return user?.roles?.includes('Admin') || false;
  }
  private http = inject(HttpClient);
  private storageService = inject(StorageService);
  private eventBus = inject(EventBusService);
  private apiConfig = inject(ApiConfigService);

  // Authentification
  login(loginRequest: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      this.apiConfig.buildAuthUrl('login'),
      loginRequest
    ).pipe(
      tap(response => {
        this.storageService.setToken(response.token);
        this.storageService.setUser(response.user);
        this.eventBus.emitUserLogin(response.user);
      })
    );
  }

  register(registerRequest: RegisterRequest): Observable<User> {
    return this.http.post<User>(
      this.apiConfig.buildAuthUrl('register'),
      registerRequest
    ).pipe(
      tap(user => {
        this.eventBus.emitUserRegistered(user);
      })
    );
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(
      this.apiConfig.buildAuthUrl('login') + '/me'
    ).pipe(
      tap(user => {
        this.storageService.setUser(user);
        this.eventBus.emitUserProfileUpdated(user);
      })
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(
      this.apiConfig.buildAuthUrl('logout'),
      {}
    ).pipe(
      tap(() => {
        this.clearAuthData();
        this.eventBus.emitUserLogout();
      })
    );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.storageService.getToken();
    return this.http.post<LoginResponse>(
      this.apiConfig.buildAuthUrl('refresh'),
      { refreshToken }
    ).pipe(
      tap(response => {
        this.storageService.setToken(response.token);
        this.storageService.setUser(response.user);
        this.eventBus.emitTokenRefreshed();
      })
    );
  }

  forgotPassword(email: string): Observable<void> {
    return this.http.post<void>(
      this.apiConfig.buildAuthUrl('forgotPassword'),
      { email }
    );
  }

  resetPassword(token: string, newPassword: string): Observable<void> {
    return this.http.post<void>(
      this.apiConfig.buildAuthUrl('resetPassword'),
      { token, newPassword }
    );
  }

  // Gestion des tokens
  getToken(): string | null {
    return this.storageService.getToken();
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  // Gestion des données utilisateur
  getStoredUser(): User | null {
    return this.storageService.getUser();
  }

  updateStoredUser(user: User): void {
    this.storageService.setUser(user);
    this.eventBus.emitUserProfileUpdated(user);
  }

  // Gestion des utilisateurs (Admin/Manager)
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(
      this.apiConfig.buildUsersUrl('base')
    );
  }

  getUserById(userId: string): Observable<User> {
    return this.http.get<User>(
      this.apiConfig.buildUserByIdUrl(userId)
    );
  }

  updateUser(userId: string, updateRequest: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(
      this.apiConfig.buildUserByIdUrl(userId),
      updateRequest
    ).pipe(
      tap(user => {
        // Si l'utilisateur modifié est l'utilisateur courant, mettre à jour le stockage
        const currentUser = this.getStoredUser();
        if (currentUser && currentUser.id === userId) {
          this.updateStoredUser(user);
        }
        this.eventBus.emitUserUpdated(user);
      })
    );
  }

  deleteUser(userId: string): Observable<void> {
    return this.http.delete<void>(
      this.apiConfig.buildUserByIdUrl(userId)
    ).pipe(
      tap(() => {
        this.eventBus.emitUserDeleted(userId);
      })
    );
  }

  // Gestion des rôles
  addUserRole(userId: string, role: string): Observable<User> {
    return this.http.post<User>(
      `${this.apiConfig.buildUserByIdUrl(userId)}/roles`,
      { role }
    ).pipe(
      tap(user => {
        this.eventBus.emitUserRoleAdded(userId, role);
      })
    );
  }

  removeUserRole(userId: string, role: string): Observable<User> {
    return this.http.delete<User>(
      `${this.apiConfig.buildUserByIdUrl(userId)}/roles/${role}`
    ).pipe(
      tap(user => {
        this.eventBus.emitUserRoleRemoved(userId, role);
      })
    );
  }

  // Méthodes privées
  private clearAuthData(): void {
    this.storageService.removeToken();
    this.storageService.removeUser();
  }
}