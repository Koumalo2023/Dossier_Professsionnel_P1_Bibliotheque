import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IconComponent } from '../../../shared/components/atoms/icons/icon.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule, 
    IconComponent
  ],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {
  features = [
    {
      icon: 'fa-book-open',
      title: 'Catalogue complet',
      description: 'Accédez à des milliers de livres numériques et physiques'
    },
    {
      icon: 'fa-search',
      title: 'Recherche avancée',
      description: 'Trouvez facilement les livres qui vous intéressent'
    },
    {
      icon: 'fa-clock',
      title: 'Réservation en ligne',
      description: 'Réservez vos livres et venez les chercher quand vous voulez'
    },
    {
      icon: 'fa-chart-line',
      title: 'Suivi de lecture',
      description: 'Suivez votre progression et définissez vos objectifs'
    }
  ];

  currentYear = new Date().getFullYear();

  navigateToAuth(path: string): void {
    // Navigation sera gérée par le routerLink dans le template
  }
}
