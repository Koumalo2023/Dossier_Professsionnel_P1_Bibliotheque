import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StorageService } from './storage/storage.service';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  tokenExpires: string;
  user: {
    id: string;
    name: string;
    email: string;
    role: string[];
  };
}

export interface RegisterResponse {
  success: boolean;
  message: string;
  user: {
    id: string;
    name: string;
    email: string;
    role: string[];
    createdAt: string;
    updatedAt: string;
  };
}

export interface ApiError {
  error: string;
  message: string;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private storageService: StorageService
  ) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          if (response.token) {
            this.storageService.setToken(response.token);
            this.storageService.setUser(response.user);
          }
        })
      );
  }

  register(userData: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/auth/register`, userData);
  }

  logout(): void {
    this.storageService.removeToken();
    this.storageService.removeUser();
  }

  isLoggedIn(): boolean {
    return !!this.storageService.getToken();
  }

  isAuthenticated(): boolean {
    return this.isLoggedIn();
  }

  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user && user.role && (user.role.includes('admin') || user.role.includes('Admin'));
  }

  getToken(): string | null {
    return this.storageService.getToken();
  }

  getCurrentUser(): any {
    const user = this.storageService.getUser();
    console.log('🔐 AuthService - getCurrentUser:', user);
    return user;
  }
}