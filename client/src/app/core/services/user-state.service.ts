import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserInfo } from '../../shared/components/molecules/user-avatar/user-avatar.component';
import { StorageService } from './storage/storage.service';

@Injectable({
  providedIn: 'root'
})
export class UserStateService {
  private currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$: Observable<UserInfo | null> = this.currentUserSubject.asObservable();

  constructor(private storageService: StorageService) {
    this.initializeUserState();
  }

  private initializeUserState(): void {
    const user = this.storageService.getUser();
    if (user) {
      const role = Array.isArray(user.role) ? user.role[0] : user.role;
      const normalizedRole = this.normalizeRole(role);
      
      const userInfo: UserInfo = {
        name: user.name,
        email: user.email,
        role: normalizedRole,
        avatarUrl: null
      };
      this.currentUserSubject.next(userInfo);
    }
  }

  setCurrentUser(user: UserInfo | null): void {
    this.currentUserSubject.next(user);
  }

  getCurrentUser(): UserInfo | null {
    return this.currentUserSubject.value;
  }

  clearUser(): void {
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.storageService.getToken();
  }

  isAdminOrManager(): boolean {
    const user = this.getCurrentUser();
    return this.isLoggedIn() && (user?.role === 'admin' || user?.role === 'manager');
  }

  normalizeRole(role: string): 'user' | 'manager' | 'admin' | undefined {
    const normalized = role.toLowerCase();
    if (normalized === 'admin' || normalized === 'administrator') return 'admin';
    if (normalized === 'manager') return 'manager';
    if (normalized === 'user') return 'user';
    return undefined;
  }
}