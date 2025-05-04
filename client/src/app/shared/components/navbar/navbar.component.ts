import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UserInfo } from '../../../core/models/user.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  @Input() userInfo: UserInfo = { isLoggedIn: false };
  @Output() logout = new EventEmitter<void>();
  @Output() toggleSidebar = new EventEmitter<void>(); // Pour le menu mobile

  isMobileMenuOpen = false;

  onLogoutClick(): void {
    this.logout.emit();
    this.isMobileMenuOpen = false; // Fermer le menu mobile après déconnexion
  }

  onToggleSidebarClick(): void {
    this.toggleSidebar.emit();
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
