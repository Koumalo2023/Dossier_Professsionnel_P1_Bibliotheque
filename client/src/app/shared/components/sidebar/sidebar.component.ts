import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UserInfo } from '../../../core/models/user.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  @Input() isOpen: boolean = false;
  @Input() userInfo: UserInfo = { isLoggedIn: false }; // Pass user info for conditional links
  @Output() closeSidebar = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();

  onCloseClick(): void {
    this.closeSidebar.emit();
  }

  onNavLinkClick(): void {
    this.closeSidebar.emit(); // Close sidebar when a link is clicked
  }

  onLogoutClick(): void {
    this.logout.emit();
    this.closeSidebar.emit(); // Close sidebar after logout
  }
}
