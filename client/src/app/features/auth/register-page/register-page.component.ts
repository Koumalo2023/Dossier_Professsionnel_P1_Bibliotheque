import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router'; 
import { AuthFormComponent } from '../../../shared/components/organisms/auth-form/auth-form.component';

@Component({
    selector: 'app-register-page',
    imports: [CommonModule, RouterModule, AuthFormComponent],
    templateUrl: './register-page.component.html',
    styleUrl: './register-page.component.scss'
})
export class RegisterPageComponent {
 
}