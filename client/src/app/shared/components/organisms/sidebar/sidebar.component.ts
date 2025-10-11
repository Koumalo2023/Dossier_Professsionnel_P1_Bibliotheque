import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserAvatarComponent, UserInfo } from '../../molecules/user-avatar/user-avatar.component';
import { CommonModule } from '@angular/common';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { IconComponent } from '../../atoms/icons/icon.component';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterModule, TypographyComponent, UserAvatarComponent, IconComponent],
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  @Input() isOpen = false;
  @Input() userInfo: UserInfo | null = null;

  @Output() closeSidebar = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();

  constructor(private router: Router) {}

  navigateTo(path: string): void {
    this.router.navigate([path]);
    this.closeSidebar.emit();
  }

  onLogout(): void {
    this.logout.emit();
    this.closeSidebar.emit();
  }

  isLoggedIn(): boolean {
    return !!this.userInfo;
  }

  isAdminOrManager(): boolean {
    // À adapter selon la structure réelle de `userInfo`
    return this.isLoggedIn() && ['Admin', 'Manager'].includes(this.userRole);
  }

  get userRole(): string {
    return this.userInfo ? 'Utilisateur' : '';
  }
}
