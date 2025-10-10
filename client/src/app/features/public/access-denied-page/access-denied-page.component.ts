import { Component } from '@angular/core';

@Component({
    selector: 'app-access-denied-page',
    imports: [],
    templateUrl: './access-denied-page.component.html',
    styleUrl: './access-denied-page.component.scss'
})
export class AccessDeniedPageComponent {
private router = inject(Router);

  goHome(): void {
    this.router.navigate(['/']);
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
