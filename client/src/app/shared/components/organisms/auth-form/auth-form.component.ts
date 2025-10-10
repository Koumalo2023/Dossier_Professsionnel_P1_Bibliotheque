import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

export interface AuthFormData {
  email: string;
  name?: string; // Optionnel pour l'inscription
  password: string;
}
@Component({
    selector: 'app-auth-form',
    imports: [
      CommonModule,
      ReactiveFormsModule,
      MatFormFieldModule,
      MatInputModule,
      MatButtonModule,
      MatProgressSpinnerModule,
      MatIconModule
    ],
    templateUrl: './auth-form.component.html',
    styleUrl: './auth-form.component.scss'
})
export class AuthFormComponent {
  @Input() formType: 'login' | 'register' = 'login';
  @Input() loading: boolean = false;
  @Input() errorMessage: string | null = null;
  @Output() formSubmit = new EventEmitter<AuthFormData>();

  private fb = inject(FormBuilder);
  authForm!: FormGroup;
  
  hidePassword = true;

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    const formControls: any = {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    };

    if (this.formType === 'register') {
      formControls.name = ['', [Validators.required, Validators.minLength(2)]];
    }

    this.authForm = this.fb.group(formControls);
  }

  onSubmit(): void {
    if (this.authForm.valid && !this.loading) {
      this.formSubmit.emit(this.authForm.value);
    }
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  get email() { return this.authForm.get('email'); }
  get name() { return this.authForm.get('name'); }
  get password() { return this.authForm.get('password'); }

  get submitButtonText(): string {
    return this.loading
      ? this.formType === 'login' ? 'Connexion...' : 'Inscription...'
      : this.formType === 'login' ? 'Se connecter' : 'Créer un compte';
  }

  get title(): string {
    return this.formType === 'login' ? 'Connexion' : 'Inscription';
  }

  get subtitle(): string {
    return this.formType === 'login'
      ? 'Connectez-vous à votre compte'
      : 'Créez votre compte bibliothèque';
  }
}
