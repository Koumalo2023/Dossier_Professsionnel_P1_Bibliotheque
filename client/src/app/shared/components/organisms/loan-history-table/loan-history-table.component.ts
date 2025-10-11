import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';


export interface LoanItem {
  id: number;
  bookTitle: string;
  bookAuthor: string;
  borrowerName?: string; // pour admin
  borrowedAt: string; // ISO date
  dueDate: string; // ISO date
  returnedAt?: string | null;
  status: 'active' | 'returned' | 'overdue';
}


@Component({
  selector: 'app-loan-history-table',
     standalone: true,
  imports: [CommonModule, ButtonComponent, IconComponent, TypographyComponent],
  templateUrl: './loan-history-table.component.html',
  styleUrl: './loan-history-table.component.scss'
})
export class LoanHistoryTableComponent {
 @Input() loans: LoanItem[] = [];
  @Input() loading = false;
  @Input() isAdminView = false;
  @Input() showActions = true;

  @Output() extendLoan = new EventEmitter<number>();
  @Output() markAsReturned = new EventEmitter<number>();
  @Output() sendReminder = new EventEmitter<number>();

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  getStatusText(status: LoanItem['status']): string {
    switch (status) {
      case 'active': return 'Actif';
      case 'returned': return 'Retourné';
      case 'overdue': return 'En retard';
      default: return status;
    }
  }

  getStatusColor(status: LoanItem['status']): 'success' | 'danger' | 'text-secondary' {
    switch (status) {
      case 'active': return 'success';
      case 'overdue': return 'danger';
      default: return 'text-secondary';
    }
  }

  onExtend(id: number): void {
    this.extendLoan.emit(id);
  }

  onMarkReturned(id: number): void {
    this.markAsReturned.emit(id);
  }

  onSendReminder(id: number): void {
    this.sendReminder.emit(id);
  }
}
