import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';

@Component({
  selector: 'app-analytics-layout',
  standalone: true,
  imports: [RouterOutlet,  HeaderComponent, FooterComponent],
  templateUrl: './analytics-layout.component.html',
  styleUrl: './analytics-layout.component.scss'
})
export class AnalyticsLayoutComponent {
  // Layout pour les analytiques
}