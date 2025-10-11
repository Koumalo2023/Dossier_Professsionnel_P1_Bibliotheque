import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { IconComponent } from '../../atoms/icons/icon.component';

/**Exemple d'utilisation
 * 
 * <!-- Affichage statique (lecture seule) -->
<app-rating-stars [rating]="4.5" [readonly]="true"></app-rating-stars>

<!-- Petite taille -->
<app-rating-stars [rating]="3" size="sm"></app-rating-stars>

<!-- Grande taille (dans une fiche livre) -->
<app-rating-stars [rating]="4.8" size="lg"></app-rating-stars>

 */

@Component({
  selector: 'app-rating-stars',
  imports: [CommonModule, IconComponent],
  templateUrl: './rating-stars.component.html',
  styleUrl: './rating-stars.component.scss'
})
export class RatingStarsComponent {
@Input() rating: number = 0; // entre 0 et 5
  @Input() max: number = 5;
  @Input() readonly: boolean = true;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() interactive: boolean = false; // pour édition future

  get stars(): number[] {
    return Array(this.max).fill(0).map((_, i) => i + 1);
  }

  getStarIcon(starIndex: number): string {
    const filled = starIndex <= this.rating;
    const half = !Number.isInteger(this.rating) && starIndex === Math.ceil(this.rating);
    return half ? 'fa-star-half-alt' : (filled ? 'fa-star' : 'fa-star-o');
  }

  getStarColor(starIndex: number): 'accent' | 'text-secondary' {
    return starIndex <= this.rating ? 'accent' : 'text-secondary';
  }

  // Méthodes pour interaction future (non activées si readonly)
  onStarClick(starIndex: number): void {
    if (this.readonly || !this.interactive) return;
    // À connecter à un EventEmitter si besoin
  }

  onStarHover(starIndex: number): void {
    if (this.readonly || !this.interactive) return;
    // Optionnel : prévisualisation du hover
  }
}
