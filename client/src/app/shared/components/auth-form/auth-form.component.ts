import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';


export interface AuthFormData {
  email: string;
  name?: string; // Optionnel pour l'inscription
  password: string;
  // rememberMe?: boolean; // Optionnel pour la connexion
}
@Component({
    selector: 'app-auth-form',
    imports: [CommonModule, ReactiveFormsModule],
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

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    const formControls: any = {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    };

    if (this.formType === 'register') {
      formControls.name = ['', Validators.required];
    } else {
      // formControls.rememberMe = [false];
    }

    this.authForm = this.fb.group(formControls);
  }

  onSubmit(): void {
    if (this.authForm.valid) {
      this.formSubmit.emit(this.authForm.value);
    }
  }

  get email() { return this.authForm.get('email'); }
  get name() { return this.authForm.get('name'); }
  get password() { return this.authForm.get('password'); }
}
