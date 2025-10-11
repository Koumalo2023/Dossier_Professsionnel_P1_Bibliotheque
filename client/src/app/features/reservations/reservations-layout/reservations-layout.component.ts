import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';


@Component({
  selector: 'app-reservations-layout',
  templateUrl: './reservations-layout.component.html',
  styleUrls: ['./reservations-layout.component.scss'],
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent]
})
export class ReservationsLayoutComponent {}
