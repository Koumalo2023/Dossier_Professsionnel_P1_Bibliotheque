import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { UserAvatarComponent, UserInfo } from '../../molecules/user-avatar/user-avatar.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';


export interface UserStats {
  booksRead: number;
  goalsAchieved: number;
  readingTimeHours: number;
  favorites: number;
}
@Component({
  selector: 'app-user-profile-card',
  standalone: true,
  imports: [CommonModule, TypographyComponent, UserAvatarComponent, IconComponent, ButtonComponent, HeadingComponent],
  templateUrl: './user-profile-card.component.html',
  styleUrl: './user-profile-card.component.scss'
})
export class UserProfileCardComponent {
@Input() user!: UserInfo;
  @Input() stats: UserStats = {
    booksRead: 0,
    goalsAchieved: 0,
    readingTimeHours: 0,
    favorites: 0
  };
  @Input() showActions = true;
  @Input() role: string = 'Utilisateur';

  get initials(): string {
    return this.user.name
      .split(' ')
      .map(part => part[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }
}
