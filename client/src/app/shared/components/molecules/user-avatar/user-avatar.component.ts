import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TypographyComponent } from '../../atoms/typography/typography.component';


export interface UserInfo {
  name: string;
  email?: string;
  avatarUrl?: string | null;
  role?: 'user' | 'manager' | 'admin';
}
@Component({
  selector: 'app-user-avatar',
  imports: [CommonModule, TypographyComponent],
  templateUrl: './user-avatar.component.html',
  styleUrl: './user-avatar.component.scss'
})
export class UserAvatarComponent {
@Input() user!: UserInfo;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() showName = false;
  @Input() clickable = false;

  get initials(): string {
    if (!this.user.name) return '?';
    return this.user.name
      .split(' ')
      .map(part => part[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  get containerClasses(): string {
    return [
      'user-avatar',
      `user-avatar--${this.size}`,
      this.clickable ? 'user-avatar--clickable' : ''
    ].join(' ');
  }
}
