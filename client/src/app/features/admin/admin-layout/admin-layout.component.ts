import { Component } from '@angular/core';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet,  HeaderComponent, FooterComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {
  // Layout pour l'administration
}