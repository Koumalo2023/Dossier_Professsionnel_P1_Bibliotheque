import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  template: `
    <div class="auth-layout">
      <!-- Header minimal avec logo et lien d'accueil -->
      <header class="auth-header">
        <div class="auth-header__content">
          <a routerLink="/" class="auth-header__logo">
            <h1 class="auth-header__title">Bibliothèque</h1>
          </a>
        </div>
      </header>

      <!-- Contenu principal centré -->
      <main class="auth-main">
        <div class="auth-container">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styleUrls: ['./auth-layout.component.scss']
})
export class AuthLayoutComponent {}