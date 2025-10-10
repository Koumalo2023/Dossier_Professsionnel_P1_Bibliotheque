import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-loans-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './loans-layout.component.html',
  styleUrl: './loans-layout.component.scss'
})
export class LoansLayoutComponent {
  // Layout pour la gestion des emprunts
}