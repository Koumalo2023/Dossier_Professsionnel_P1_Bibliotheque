import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserAvatarComponent, UserInfo } from '../../molecules/user-avatar/user-avatar.component';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../../atoms/button/button.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { filter, Subscription } from 'rxjs';
import { UserStateService } from '../../../../core/services/user-state.service';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, ButtonComponent, TypographyComponent, HeadingComponent, IconComponent, UserAvatarComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isMobileMenuOpen = false;
  isUserDropdownOpen = false;
  currentRoute: string = '';
  userInfo: UserInfo | null = null;
  
  private routerSubscription!: Subscription;
  private userSubscription!: Subscription;

  constructor(
    private router: Router,
    private userStateService: UserStateService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentRoute = this.router.url;
    
    // Surveiller les changements de route
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentRoute = event.url;
        this.isMobileMenuOpen = false; // Fermer le menu mobile lors de la navigation
        this.isUserDropdownOpen = false; // Fermer le menu déroulant utilisateur
      });

    // Surveiller les changements d'état utilisateur
    this.userSubscription = this.userStateService.currentUser$.subscribe(user => {
      this.userInfo = user;
    });
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }

  onLogoClick(): void {
    if (this.isLoggedIn()) {
      // Rediriger vers la page d'accueil appropriée selon le rôle
      if (this.isAdminOrManager()) {
        this.router.navigate(['/admin']);
      } else {
        this.router.navigate(['/books']);
      }
    } else {
      this.router.navigate(['/']);
    }
  }

  onLogout(): void {
    this.authService.logout();
    this.userStateService.clearUser();
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
    this.router.navigate(['/']);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    if (this.isMobileMenuOpen) {
      this.isUserDropdownOpen = false; // Fermer le menu utilisateur si on ouvre le menu mobile
    }
  }

  toggleUserDropdown(): void {
    this.isUserDropdownOpen = !this.isUserDropdownOpen;
    if (this.isUserDropdownOpen) {
      this.isMobileMenuOpen = false; // Fermer le menu mobile si on ouvre le menu utilisateur
    }
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  get userRole(): string {
    if (!this.userInfo?.role) return '';
    switch (this.userInfo.role) {
      case 'admin': return 'Administrateur';
      case 'manager': return 'Manager';
      case 'user': return 'Utilisateur';
      default: return '';
    }
  }

  isLoggedIn(): boolean {
    return this.userStateService.isLoggedIn();
  }

  isAdminOrManager(): boolean {
    return this.userStateService.isAdminOrManager();
  }

  isAdmin(): boolean {
    const user = this.userInfo;
    return this.isLoggedIn() && user?.role === 'admin';
  }

  isManager(): boolean {
    const user = this.userInfo;
    return this.isLoggedIn() && user?.role === 'manager';
  }

  isRegularUser(): boolean {
    const user = this.userInfo;
    return this.isLoggedIn() && user?.role === 'user';
  }

  navigateToAdmin(): void {
    this.router.navigate(['/admin']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToProfile(): void {
    this.router.navigate(['/profile']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToLoans(): void {
    this.router.navigate(['/loans']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToReservations(): void {
    this.router.navigate(['/reservations']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToNotifications(): void {
    this.router.navigate(['/notifications']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
    this.isMobileMenuOpen = false;
    this.isUserDropdownOpen = false;
  }

  // Vérifier si la route actuelle correspond au chemin donné
  isActiveRoute(route: string): boolean {
    return this.currentRoute.startsWith(route);
  }
}