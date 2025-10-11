import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TypographyComponent } from '../../atoms/typography/typography.component';


/** Exemple d'utilisation
 * 
 * <!-- Objectif de lecture (dans ProfilePageComponent) -->
<app-progress-bar
  [value]="80"
  [max]="100"
  label="Objectif 2025 : 30 livres"
  [showPercentage]="true"
  size="md"
  color="success"
></app-progress-bar>

<!-- Statistique admin (dans AdminStatisticsPageComponent) -->
<app-progress-bar
  [value]="65"
  label="Livres empruntés ce mois-ci"
  color="primary"
></app-progress-bar>

<!-- Petite barre (dans une carte) -->
<app-progress-bar
  [value]="45"
  size="sm"
  [showPercentage]="false"
></app-progress-bar>
 */
@Component({
  selector: 'app-progress-bar',
  standalone: true,
  imports: [CommonModule, TypographyComponent],
  templateUrl: './progress-bar.component.html',
  styleUrl: './progress-bar.component.scss'
})
export class ProgressBarComponent {
@Input() value: number = 0;        // entre 0 et 100
  @Input() max: number = 100;
  @Input() label: string | null = null;
  @Input() showPercentage: boolean = true;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() color: 'primary' | 'success' | 'accent' = 'primary';
  @Input() animated: boolean = true;
}
