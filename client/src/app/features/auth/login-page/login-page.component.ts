import { Component } from '@angular/core';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/auth-form/auth-form.component';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginDto } from '../../../core/models/user.model';
import { AuthService } from '../../../core/services/auth.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-login-page',
    imports: [CommonModule, RouterModule, AuthFormComponent, MatIconModule],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  loading = false;
  errorMessage: string | null = null;
  returnUrl: string;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  onLogin(formData: AuthFormData) {
    this.loading = true;
    this.errorMessage = null;

    this.authService.login(formData.email, formData.password).subscribe({
      next: () => {
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Email ou mot de passe incorrect';
      }
    });
  }
}