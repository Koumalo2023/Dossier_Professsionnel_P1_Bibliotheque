import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Reservation } from '../../../../core/models/reservation.model';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { ButtonComponent } from '../../atoms/button/button.component';

@Component({
  selector: 'app-reservation-list',
  standalone: true,
  imports: [IconComponent, TypographyComponent, ButtonComponent],
  templateUrl: './reservation-list.component.html',
  styleUrl: './reservation-list.component.scss'
})
export class ReservationListComponent {
@Input() reservations: Reservation[] = [];
  @Input() loading = false;
  @Input() isAdminView = false;
  @Input() showActions = true;

  @Output() cancelReservation = new EventEmitter<string>();
  @Output() markAsAvailable = new EventEmitter<string>();

  getStatusText(status: string): string {
    switch (status) {
      case 'PENDING': return 'En attente';
      case 'AVAILABLE': return 'Disponible';
      case 'CANCELLED': return 'Annulée';
      case 'EXPIRED': return 'Expirée';
      default: return status;
    }
  }

  getStatusColor(status: string): 'primary' | 'success' | 'danger' | 'text-secondary' {
    switch (status) {
      case 'PENDING': return 'primary';
      case 'AVAILABLE': return 'success';
      case 'CANCELLED':
      case 'EXPIRED': return 'danger';
      default: return 'text-secondary';
    }
  }

  getIconForStatus(status: string): string {
    switch (status) {
      case 'AVAILABLE': return 'fa-check-circle';
      case 'CANCELLED': return 'fa-times-circle';
      case 'EXPIRED': return 'fa-clock';
      default: return 'fa-calendar';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  onCancel(id: string): void {
    this.cancelReservation.emit(id);
  }

  onMarkAvailable(id: string): void {
    this.markAsAvailable.emit(id);
  }
}
