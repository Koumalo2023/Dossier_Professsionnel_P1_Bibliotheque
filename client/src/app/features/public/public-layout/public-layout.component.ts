import { Component } from '@angular/core';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-public-layout',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent {

}
