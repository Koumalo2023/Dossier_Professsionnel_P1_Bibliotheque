import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputComponent } from '../../atoms/inputs/input.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';

export type AuthFormType = 'login' | 'register';

export interface AuthFormData {
  email: string;
  password: string;
  name?: string;
}

@Component({
  selector: 'app-auth-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputComponent,
    ButtonComponent,
    TypographyComponent
  ],
  templateUrl: './auth-form.component.html',
  styleUrl: './auth-form.component.scss'
})
export class AuthFormComponent {
  @Input() formType: AuthFormType = 'login';
  @Input() loading = false;
  @Input() errorMessage: string | null = null;

  @Output() formSubmit = new EventEmitter<AuthFormData>();

  authForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6)
    ]),
    name: new FormControl('')
  });

  constructor() {
    // Validation conditionnelle pour le nom (register uniquement)
    if (this.formType === 'register') {
      this.authForm.controls.name.setValidators([Validators.required]);
      this.authForm.controls.name.updateValueAndValidity();
    } else {
      this.authForm.controls.name.clearValidators();
      this.authForm.controls.name.updateValueAndValidity();
    }
  }

  get isRegister(): boolean {
    return this.formType === 'register';
  }

  get nameControl() {
    return this.authForm.get('name');
  }

  onSubmit(): void {
    if (this.authForm.invalid || this.loading) return;

    const { email, password, name } = this.authForm.value;

    this.formSubmit.emit({
      email: email!,
      password: password!,
      ...(this.isRegister && { name: name! })
    });
  }
}