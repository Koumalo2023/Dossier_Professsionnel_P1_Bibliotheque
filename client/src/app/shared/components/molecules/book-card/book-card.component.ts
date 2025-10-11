import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';  
import { ButtonComponent } from '../../atoms/button/button.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { Book } from '../../../../core/models/book.model';

@Component({
  selector: 'app-book-card',
  standalone: true,
  imports: [CommonModule, ButtonComponent, HeadingComponent, TypographyComponent, IconComponent],
  templateUrl: './book-card.component.html',
  styleUrls: ['./book-card.component.scss']
})
export class BookCardComponent {
   @Input() book!: Book;
  @Input() showActions = true;
  @Input() loading = false;

  @Output() borrow = new EventEmitter<string>();
  @Output() reserve = new EventEmitter<string>();

  onBorrow(): void {
    this.borrow.emit(this.book.id);
  }

  onReserve(): void {
    this.reserve.emit(this.book.id);
  }

  get availabilityText(): string {
    return this.book.available ? 'Disponible' : 'Emprunté';
  }

  get availabilityColor(): 'success' | 'danger' {
    return this.book.available ? 'success' : 'danger';
  }

  get coverFallback(): string {
    return this.book.category
      ? `${this.book.category.charAt(0).toUpperCase()}${this.book.category.slice(1)}`
      : 'Livre';
  }
}