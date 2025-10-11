import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/organisms/auth-form/auth-form.component';
import { AuthService, LoginRequest } from '../../../core/services/auth.service';
import { UserStateService } from '../../../core/services/user-state.service';

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
    private router: Router,
    private userStateService: UserStateService
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
        
        // Mettre à jour l'état utilisateur
        const userRoles = response.user.role;
        const role = Array.isArray(userRoles) ? userRoles[0] : userRoles;
        const normalizedRole = this.userStateService.normalizeRole(role);
        
        this.userStateService.setCurrentUser({
          name: response.user.name,
          email: response.user.email,
          role: normalizedRole,
          avatarUrl: null
        });
        
        // Redirection en fonction du rôle de l'utilisateur
        let redirectPath = '/books'; // Par défaut
        
        if (userRoles.includes('admin') || userRoles.includes('manager')) {
          redirectPath = '/admin';
        } else if (userRoles.includes('user')) {
          redirectPath = '/profile';
        }
        
        console.log(`Redirection vers: ${redirectPath} (rôles: ${userRoles.join(', ')})`);
        this.router.navigate([redirectPath]);
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