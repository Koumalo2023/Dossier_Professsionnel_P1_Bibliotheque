import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/organisms/auth-form/auth-form.component';
import { AuthService, LoginRequest } from '../../../core/services/auth.service';

@Component({
    selector: 'app-login-page',
    standalone: true,
    imports: [CommonModule, RouterModule, AuthFormComponent],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLoginSubmit(formData: AuthFormData): void {
    this.loading = true;
    this.errorMessage = null;

    const loginRequest: LoginRequest = {
      email: formData.email,
      password: formData.password
    };

    this.authService.login(loginRequest).subscribe({
      next: (response) => {
        this.loading = false;
        console.log('Connexion réussie:', response);
        // Redirection vers la page d'accueil ou dashboard
        this.router.navigate(['/books']);
      },
      error: (error) => {
        this.loading = false;
        console.error('Erreur de connexion:', error);
        
        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors) {
          this.errorMessage = error.error.errors.join(', ');
        } else {
          this.errorMessage = 'Une erreur est survenue lors de la connexion';
        }
      }
    });
  }
}