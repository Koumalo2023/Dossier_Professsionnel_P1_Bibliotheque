import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from '../../../shared/components/organisms/footer/footer.component';
import { HeaderComponent } from '../../../shared/components/organisms/header/header.component';
@Component({
  selector: 'app-books-layout',
  templateUrl: './books-layout.component.html',
  styleUrls: ['./books-layout.component.scss'],
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent]
})
export class BooksLayoutComponent {}