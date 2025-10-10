import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-notifications-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './notifications-layout.component.html',
  styleUrl: './notifications-layout.component.scss'
})
export class NotificationsLayoutComponent {
  // Layout pour la gestion des notifications
}