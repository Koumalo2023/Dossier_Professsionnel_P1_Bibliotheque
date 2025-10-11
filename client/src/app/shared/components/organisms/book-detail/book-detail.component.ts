import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { Book } from '../../../../core/models/book.model';

@Component({
    selector: 'app-book-detail',
    standalone: true,
    imports: [CommonModule, TypographyComponent, HeadingComponent, ButtonComponent, IconComponent],
    templateUrl: './book-detail.component.html',
    styleUrl: './book-detail.component.scss'
})
export class BookDetailComponent {
@Input() book!: Book;
  @Input() loading = false;
  @Input() showActions = true;

  @Output() borrow = new EventEmitter<string>();
  @Output() reserve = new EventEmitter<string>();

  get isAvailable(): boolean {
    return this.book.availableCopies > 0;
  }

  onBorrow(): void {
    this.borrow.emit(this.book.id);
  }

  onReserve(): void {
    this.reserve.emit(this.book.id);
  }
}
