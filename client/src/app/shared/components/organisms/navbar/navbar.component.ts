import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserAvatarComponent, UserInfo } from '../../molecules/user-avatar/user-avatar.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { ButtonComponent } from '../../atoms/button/button.component';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [CommonModule, RouterModule, HeadingComponent, IconComponent, TypographyComponent, UserAvatarComponent, ButtonComponent],
    templateUrl: './navbar.component.html',
    styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
   @Input() userInfo: UserInfo | null = null;
  @Input() isSidebarOpen = false;

  @Output() logout = new EventEmitter<void>();
  @Output() toggleSidebar = new EventEmitter<void>();

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

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  isLoggedIn(): boolean {
    return !!this.userInfo;
  }

  getUserRole(): string {
    // Dans une implémentation réelle, ce serait dans `userInfo.roles`
    // Ici, on simule pour le design
    return this.userInfo ? 'User' : '';
  }

  isAdminOrManager(): boolean {
    const role = this.getUserRole();
    return ['Admin', 'Manager'].includes(role);
  }
}
