import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from "rxjs";
import { environment } from "../../../environments/environment";
import { CurrentUser, RegisterDto, UserDto, UserInfo, LoginResponse } from "../models/user.model";

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
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, { email, password }).pipe(
      tap(response => this.handleAuthentication(response)),
      map(response => this.convertUserDto(response.user)),
      catchError(error => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  register(registerData: RegisterDto): Observable<{ success: boolean; message: string; user?: UserDto }> {
    return this.http.post<{
      success: boolean;
      message: string;
      user?: UserDto;
    }>(`${this.apiUrl}/Auth/register`, registerData).pipe(
      catchError(error => {
        let errorMessage = 'Une erreur est survenue lors de l\'inscription';
        
        if (error.error?.errors) {
          errorMessage = Object.values(error.error.errors).join('\n');
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        }
        
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  logout(): void {
    this.clearAuthState();
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<LoginResponse> {
    const token = this.getAccessToken();
    if (!token) {
      return throwError(() => new Error('No token available'));
    }
    
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/refresh`, { token }).pipe(
      tap(response => this.handleAuthentication(response)),
      catchError(error => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  getAccessToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;
    
    // Vérifier l'expiration du token
    const tokenExpired = this.isTokenExpired(token);
    if (tokenExpired) {
      this.clearAuthState();
      return false;
    }
    
    return true;
  }

  getCurrentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

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

  private initializeAuthState(): void {
    const userData = localStorage.getItem('currentUser');
    const token = localStorage.getItem('token');
    
    if (userData && token) {
      try {
        // Vérifier si le token est expiré
        if (this.isTokenExpired(token)) {
          this.clearAuthState();
          return;
        }

        const user = JSON.parse(userData) as CurrentUser;
        this.currentUserSubject.next({
          ...user,
          createdAt: new Date(user.createdAt),
          updatedAt: new Date(user.updatedAt)
        });

        // Planifier le rafraîchissement automatique du token
        this.scheduleTokenRefresh(token);
      } catch {
        this.clearAuthState();
      }
    }
  }

  private handleAuthentication(response: LoginResponse): void {
    localStorage.setItem('token', response.token);
    const user = this.convertUserDto(response.user);
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);

    // Planifier le rafraîchissement automatique du token
    this.scheduleTokenRefresh(response.token);
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
    localStorage.removeItem('currentUser');
    
    if (this.tokenTimer) {
      clearTimeout(this.tokenTimer);
      this.tokenTimer = null;
    }
    
    this.currentUserSubject.next(null);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000; // Convertir en millisecondes
      return Date.now() >= expirationTime;
    } catch {
      return true;
    }
  }

  private scheduleTokenRefresh(token: string): void {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;
      const currentTime = Date.now();
      const timeUntilExpiry = expirationTime - currentTime;
      
      // Rafraîchir le token 5 minutes avant expiration
      const refreshTime = Math.max(timeUntilExpiry - 5 * 60 * 1000, 0);
      
      if (this.tokenTimer) {
        clearTimeout(this.tokenTimer);
      }
      
      this.tokenTimer = setTimeout(() => {
        this.refreshToken().subscribe({
          error: () => this.logout()
        });
      }, refreshTime);
    } catch {
      // En cas d'erreur de parsing, déconnecter l'utilisateur
      this.logout();
    }
  }
}