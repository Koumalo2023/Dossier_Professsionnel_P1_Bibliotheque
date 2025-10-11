import { Component } from '@angular/core';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user-profile-layout',
   standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './user-profile-layout.component.html',
  styleUrl: './user-profile-layout.component.scss'
})
export class UserProfileLayoutComponent {

}
