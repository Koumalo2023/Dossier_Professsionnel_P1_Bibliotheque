import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Book } from '../../../../core/models/book.model';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { BookCardComponent } from '../../molecules/book-card/book-card.component';

@Component({
  selector: 'app-book-grid',
  standalone: true,
  imports: [CommonModule, IconComponent, TypographyComponent, BookCardComponent],
  templateUrl: './book-grid.component.html',
  styleUrl: './book-grid.component.scss'
})
export class BookGridComponent {
 @Input() books: Book[] = [];
  @Input() loading = false;
  @Input() showActions = true;
  @Input() emptyMessage = 'Aucun livre trouvé.';

  @Output() borrow = new EventEmitter<string>();
  @Output() reserve = new EventEmitter<string>();

  onBorrow(bookId: string): void {
    this.borrow.emit(bookId);
  }

  onReserve(bookId: string): void {
    this.reserve.emit(bookId);
  }

  trackByBookId(index: number, book: Book): string {
    return book.id;
  }
}
