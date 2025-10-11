import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/organisms/auth-form/auth-form.component';
import { AuthService, RegisterRequest } from '../../../core/services/auth.service';

@Component({
    selector: 'app-register-page',
    standalone: true,
    imports: [CommonModule, RouterModule, AuthFormComponent],
    templateUrl: './register-page.component.html',
    styleUrl: './register-page.component.scss'
})
export class RegisterPageComponent {
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onRegisterSubmit(formData: AuthFormData): void {
    this.loading = true;
    this.errorMessage = null;

    const registerRequest: RegisterRequest = {
      name: formData.name!,
      email: formData.email,
      password: formData.password
    };

    this.authService.register(registerRequest).subscribe({
      next: (response) => {
        this.loading = false;
        console.log('Inscription réussie:', response);
        // Redirection vers la page de connexion après inscription réussie
        this.router.navigate(['/auth/login'], {
          queryParams: { message: 'Inscription réussie! Vous pouvez maintenant vous connecter.' }
        });
      },
      error: (error) => {
        this.loading = false;
        console.error('Erreur d\'inscription:', error);
        
        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors) {
          this.errorMessage = error.error.errors.join(', ');
        } else {
          this.errorMessage = 'Une erreur est survenue lors de l\'inscription';
        }
      }
    });
  }
}