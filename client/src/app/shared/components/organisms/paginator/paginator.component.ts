import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';

@Component({
  selector: 'app-paginator',
  templateUrl: './paginator.component.html',
  styleUrls: ['./paginator.component.scss'],
  imports: [CommonModule, TypographyComponent, IconComponent],
  standalone: true
})
export class PaginatorComponent {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() totalCount: number = 0;
  @Input() pageSize: number = 10;
  @Input() loading = false;

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  pageSizeOptions = [5, 10, 20, 50];

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage && !this.loading) {
      this.pageChange.emit(page);
    }
  }

  onPageSizeChange(event: Event): void {
    const newSize = Number((event.target as HTMLSelectElement).value);
    if (newSize !== this.pageSize) {
      this.pageSizeChange.emit(newSize);
    }
  }

  get pages(): number[] {
    const delta = 2;
    const range: number[] = [];
    const start = Math.max(1, this.currentPage - delta);
    const end = Math.min(this.totalPages, this.currentPage + delta);

    for (let i = start; i <= end; i++) {
      range.push(i);
    }

    // Ajoute "1" et "..." si nécessaire
    if (start > 2) {
      range.unshift(-1); // placeholder pour "..."
    }
    if (start > 1) {
      range.unshift(1);
    }

    // Ajoute "..." et "totalPages" si nécessaire
    if (end < this.totalPages - 1) {
      range.push(-1); // placeholder pour "..."
    }
    if (end < this.totalPages) {
      range.push(this.totalPages);
    }

    return range;
  }

  isEllipsis(page: number): boolean {
    return page === -1;
  }
}

