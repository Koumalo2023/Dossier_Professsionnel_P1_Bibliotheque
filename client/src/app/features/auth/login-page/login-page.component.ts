import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginDto } from '../../../core/models/user.model';
import { AuthFormComponent, AuthFormData } from '../../../shared/components/organisms/auth-form/auth-form.component';
import { AuthService } from '../../../core/services/api/auth.service';

@Component({
    selector: 'app-login-page',
    imports: [CommonModule, RouterModule,],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
   

  constructor(
    
  ) {
   
  }

  
}