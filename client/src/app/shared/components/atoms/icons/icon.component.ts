import { Component, Input } from '@angular/core';

export type IconSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2x' | '3x';
export type IconColor = 
  | 'primary'
  | 'secondary'
  | 'accent'
  | 'success'
  | 'danger'
  | 'warning'
  | 'info'
  | 'text-primary'
  | 'text-secondary'
  | 'inherit';

  /** Exemple d'utilisation
   * 
   * <!-- Icône de base -->
<app-icon name="fa-book"></app-icon>

<!-- Icône colorée -->
<app-icon name="fa-user" color="primary"></app-icon>

<!-- Icône grande et animée -->
<app-icon name="fa-spinner" size="lg" spin="true" color="accent"></app-icon>

<!-- Icône dans un bouton -->
<app-button iconLeft="fa-search">Rechercher</app-button>
   */

@Component({
  selector: 'app-icon',
  templateUrl: './icon.component.html',
  styleUrls: ['./icon.component.scss'],
  standalone: true
})
export class IconComponent {
  @Input() name!: string; // ex: 'fa-book', 'fa-user'
  @Input() size: IconSize = 'md';
  @Input() color: IconColor = 'inherit';
  @Input() spin = false;
  @Input() pulse = false;
  @Input() rotate?: 90 | 180 | 270;
  @Input() flip?: 'horizontal' | 'vertical';

  get classes(): string[] {
    const classes = ['fas', this.name];

    // Taille
    if (this.size !== 'md') {
      classes.push(`fa-${this.size}`);
    }

    // Animations
    if (this.spin) classes.push('fa-spin');
    if (this.pulse) classes.push('fa-pulse');

    // Rotation
    if (this.rotate) classes.push(`fa-rotate-${this.rotate}`);

    // Flip
    if (this.flip === 'horizontal') classes.push('fa-flip-horizontal');
    if (this.flip === 'vertical') classes.push('fa-flip-vertical');

    return classes;
  }

  get colorClass(): string {
    return this.color !== 'inherit' ? `icon-color--${this.color}` : '';
  }
}