import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

export type LoaderVariant = 'spinner' | 'dots' | 'bar';
export type LoaderSize = 'sm' | 'md' | 'lg';
export type LoaderColor = 
  | 'primary'
  | 'secondary'
  | 'accent'
  | 'light'
  | 'text-primary';

  /** Exemple d'utilisation
   * 
   * <!-- Spinner classique dans un bouton -->
<app-button [loading]="true">Chargement...</app-button>

<!-- Loader centré sur une page -->
<div class="page-loading">
  <app-loader variant="spinner" size="lg" color="primary" label="Chargement des livres..."></app-loader>
</div>

<!-- Dots dans une carte -->
<app-loader variant="dots" color="accent"></app-loader>

<!-- Barre de chargement (ex. upload) -->
<app-loader variant="bar" color="primary"></app-loader>
   */
  
@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent {
  @Input() variant: LoaderVariant = 'spinner';
  @Input() size: LoaderSize = 'md';
  @Input() color: LoaderColor = 'primary';
  @Input() label: string | null = null; // texte optionnel sous le loader
}