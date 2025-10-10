import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { AuthService } from './core/services/auth.service';
import { CurrentUser, UserInfo } from './core/models/user.model';

@Component({
    selector: 'app-root',
    imports: [CommonModule,
        RouterOutlet,
        NavbarComponent,
        FooterComponent,
        SidebarComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'library-management-client';
  private _isSidebarOpen = false;
  private routerSubscription!: Subscription;
  isAuthRoute = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    // Surveiller les changements de route pour détecter les routes d'authentification
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.isAuthRoute = event.url.startsWith('/auth');
      });
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  // Récupère les informations de l'utilisateur connecté
  currentUserInfo(): UserInfo {
    const user = this.authService.getCurrentUser();
    return {
      isLoggedIn: this.authService.isAuthenticated(),
      username: user?.name,
      isAdmin: this.authService.isAdmin()
    };
  }

  // Vérifie si la sidebar est ouverte
  isSidebarOpen(): boolean {
    return this._isSidebarOpen;
  }

  // Bascule l'état de la sidebar
  toggleSidebar(): void {
    this._isSidebarOpen = !this._isSidebarOpen;
  }

  // Ferme la sidebar
  closeSidebar(): void {
    this._isSidebarOpen = false;
  }

  // Gère la déconnexion
  handleLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.closeSidebar(); // Ferme la sidebar après déconnexion
  }
}
