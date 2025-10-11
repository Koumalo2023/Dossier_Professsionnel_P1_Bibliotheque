import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';

@Component({
    selector: 'app-footer',
    standalone: true,
    imports: [CommonModule, IconComponent, TypographyComponent],
    templateUrl: './footer.component.html',
    styleUrl: './footer.component.scss'
})
export class FooterComponent {
  currentYear: number = new Date().getFullYear();
}
