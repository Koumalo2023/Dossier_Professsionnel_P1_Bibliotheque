import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-reservations-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="reservations-layout">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    .reservations-layout {
      min-height: 100vh;
      background-color: var(--background-color);
    }
  `]
})
export class ReservationsLayoutComponent {}