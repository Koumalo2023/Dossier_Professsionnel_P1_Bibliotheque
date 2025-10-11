import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { UserAvatarComponent, UserInfo } from '../../molecules/user-avatar/user-avatar.component';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../../atoms/button/button.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, ButtonComponent, TypographyComponent, HeadingComponent, IconComponent, UserAvatarComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Input() userInfo: UserInfo | null = null;
  @Input() isSidebarOpen = false;

  @Output() logout = new EventEmitter<void>();
  @Output() toggleSidebar = new EventEmitter<void>();

  isMobileMenuOpen = false;

  constructor(private router: Router) {}

  onLogoClick(): void {
    this.router.navigate(['/']);
  }

  onLogout(): void {
    this.logout.emit();
  }

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
    this.isMobileMenuOpen = false;
  }

  get userRole(): string {
    // Dans une implémentation réelle, ce serait dans `userInfo`
    return this.userInfo ? 'Utilisateur' : '';
  }

  isLoggedIn(): boolean {
    return !!this.userInfo;
  }

  isAdminOrManager(): boolean {
    // À adapter selon la structure réelle de `userInfo`
    return this.isLoggedIn() && ['Admin', 'Manager'].includes(this.userRole);
  }
}