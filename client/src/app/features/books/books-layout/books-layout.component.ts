import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-books-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="books-layout">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    .books-layout {
      min-height: 100vh;
      background-color: var(--background-color);
    }
  `]
})
export class BooksLayoutComponent {}