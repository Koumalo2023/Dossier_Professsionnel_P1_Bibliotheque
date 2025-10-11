import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

export type HeadingLevel = 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';
export type HeadingVariant = 'default' | 'serif'; // serif = Merriweather (livres)

@Component({
  selector: 'app-heading',
   imports: [CommonModule],
  templateUrl: './heading.component.html',
  styleUrls: ['./heading.component.scss'],
  standalone: true
})
export class HeadingComponent {
  @Input() level: HeadingLevel = 'h2';
  @Input() variant: HeadingVariant = 'default';
  @Input() color: string = 'text-primary'; // clé de couleur
  @Input() align: 'left' | 'center' | 'right' = 'left';
}