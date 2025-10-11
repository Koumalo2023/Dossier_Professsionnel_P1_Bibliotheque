import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type TextVariant = 'body' | 'small' | 'label' | 'caption';
export type TextColor = 'text-primary' | 'text-secondary' | 'primary' | 'success' | 'danger' | 'accent';


/* Exemple d'utilisation
<!-- Titre principal (landing page) -->
<app-heading level="h1" variant="default" align="center">
  Votre Bibliothèque Numérique à Portée de Main
</app-heading>

<!-- Titre de livre (dans BookCard) -->
<app-heading level="h3" variant="serif" color="primary">
  Les Misérables
</app-heading>

<!-- Description -->
<app-text variant="body" color="text-secondary">
  Découvrez des milliers de livres depuis chez vous.
</app-text>

<!-- Statut de disponibilité -->
<app-text variant="small" color="success" weight="semibold">
  ✅ Disponible
</app-text>

*/
@Component({
  selector: 'app-typography',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './typography.component.html',
  styleUrls: ['./typography.component.scss']
})
export class TypographyComponent {

  @Input() variant: TextVariant = 'body';
  @Input() color: TextColor = 'text-primary';
  @Input() weight: 'regular' | 'medium' | 'semibold' | 'bold' = 'regular';
  @Input() italic = false;
  @Input() align: 'left' | 'center' | 'right' = 'left';
}