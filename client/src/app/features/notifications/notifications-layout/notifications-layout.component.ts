import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';

@Component({
  selector: 'app-notifications-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './notifications-layout.component.html',
  styleUrl: './notifications-layout.component.scss'
})
export class NotificationsLayoutComponent {
  // Layout pour la gestion des notifications
}