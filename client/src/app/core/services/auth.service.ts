import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from "rxjs";
import { environment } from "../../../environments/environment";
import {  CurrentUser,  RefreshTokenResponse, TokenResponse, UserDto, UserInfo } from "../models/user.model";

// auth.service.ts

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(null);
  private tokenTimer: ReturnType<typeof setTimeout> | null = null;
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {
    this.initializeAuthState();
  }

  login(email: string, password: string): Observable<CurrentUser> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/auth/login`, { email, password }).pipe(
      tap(response => this.handleAuthentication(response)),
      map(response => this.convertUserDto(response.user)),
      catchError(error => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  refreshToken(): Observable<RefreshTokenResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      this.clearAuthState();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<RefreshTokenResponse>(
      `${this.apiUrl}/auth/refresh-token`,
      { refreshToken }
    ).pipe(
      tap(response => this.handleTokenRefresh(response)),
      catchError(error => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    this.clearAuthState();
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  getCurrentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }
  /**
   * Convertit CurrentUser en UserInfo pour les composants UI
   */
  getUserInfo(): UserInfo {
    const user = this.getCurrentUser();
    return {
      isLoggedIn: this.isAuthenticated(),
      username: user?.name || undefined,
      isAdmin: this.isAdmin()
    };
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.roles.includes(role) ?? false;
  }

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  // Private methods

  private initializeAuthState(): void {
    const userData = localStorage.getItem('currentUser');
    if (userData) {
      try {
        const user = JSON.parse(userData) as CurrentUser;
        this.currentUserSubject.next({
          ...user,
          createdAt: new Date(user.createdAt),
          updatedAt: new Date(user.updatedAt)
        });
      } catch {
        this.clearAuthState();
      }
    }
  }

  private handleAuthentication(response: TokenResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    this.scheduleTokenRefresh(new Date(response.tokenExpires));
  }

  private handleTokenRefresh(response: RefreshTokenResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    this.scheduleTokenRefresh(new Date(response.tokenExpires));
  }

  private scheduleTokenRefresh(expiryDate: Date): void {
    if (this.tokenTimer) {
      clearTimeout(this.tokenTimer);
    }

    const expiresIn = expiryDate.getTime() - Date.now() - 60000; // 1 minute before expiry
    if (expiresIn > 0) {
      this.tokenTimer = setTimeout(() => {
        this.refreshToken().subscribe();
      }, expiresIn);
    }
  }

  private convertUserDto(userDto: UserDto): CurrentUser {
    return {
      id: userDto.id,
      name: userDto.name,
      email: userDto.email,
      roles: userDto.role ? [userDto.role] : [],
      createdAt: new Date(userDto.createdAt),
      updatedAt: new Date(userDto.updatedAt)
    };
  }

  private clearAuthState(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('currentUser');
    
    if (this.tokenTimer) {
      clearTimeout(this.tokenTimer);
      this.tokenTimer = null;
    }
    
    this.currentUserSubject.next(null);
  }
}