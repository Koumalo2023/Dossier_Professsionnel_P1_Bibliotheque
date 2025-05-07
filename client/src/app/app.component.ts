import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
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
export class AppComponent {
  title = 'library-management-client';
  private _isSidebarOpen = false;
  

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

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
