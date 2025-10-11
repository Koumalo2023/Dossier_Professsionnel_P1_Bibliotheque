import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';

@Component({
  selector: 'app-loans-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './loans-layout.component.html',
  styleUrl: './loans-layout.component.scss'
})
export class LoansLayoutComponent {
  // Layout pour la gestion des emprunts
}