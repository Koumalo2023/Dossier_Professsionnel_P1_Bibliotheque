import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/auth-form/auth-form.component';
import { AuthService } from '../../../core/services/auth.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-register-page',
    imports: [CommonModule, RouterModule, AuthFormComponent, MatIconModule],
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

  onRegister(formData: AuthFormData) {
    this.loading = true;
    this.errorMessage = null;

    this.authService.register({
      name: formData.name!,
      email: formData.email,
      password: formData.password
    }).subscribe({
      next: () => {
        this.router.navigate(['/login'], {
          queryParams: { registered: 'true' }
        });
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Erreur lors de l\'inscription';
      }
    });
  }
}