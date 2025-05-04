import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/auth-form/auth-form.component';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterDto } from '../../../core/models/user.model';

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