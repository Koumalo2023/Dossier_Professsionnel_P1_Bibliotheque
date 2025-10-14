import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReservationService } from '../../../core/services/api/reservation.service';
import { Reservation, ReservationStatus } from '../../../core/models/reservation.model';
import { ReservationListComponent } from '../../../shared/components/organisms/reservation-list/reservation-list.component';
import { PaginatorComponent } from '../../../shared/components/organisms/paginator/paginator.component';

@Component({
  selector: 'app-admin-reservation-management-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReservationListComponent,
    PaginatorComponent
  ],
  templateUrl: './admin-reservation-management-page.component.html'
})
export class AdminReservationManagementPageComponent implements OnInit {
  private reservationService = inject(ReservationService);

  reservations: Reservation[] = [];
  filteredReservations: Reservation[] = [];
  loading = false;
  error: string | null = null;

  // Filtres
  statusFilter: string = 'all';
  searchTerm: string = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;

  // Modal state
  showStatusModal = false;
  showCancelModal = false;
  selectedReservation: Reservation | null = null;
  newStatus: string = '';
  availableSince: string = '';

  // Filtres de statut
  readonly statusOptions = [
    { value: 'all', label: 'Toutes les réservations' },
    { value: ReservationStatus.PENDING, label: 'En attente' },
    { value: ReservationStatus.AVAILABLE, label: 'Disponible' },
    { value: ReservationStatus.CANCELLED, label: 'Annulée' },
    { value: ReservationStatus.EXPIRED, label: 'Expirée' }
  ];

  readonly statusLabels = {
    [ReservationStatus.PENDING]: 'En attente',
    [ReservationStatus.AVAILABLE]: 'Disponible',
    [ReservationStatus.CANCELLED]: 'Annulée',
    [ReservationStatus.EXPIRED]: 'Expirée'
  };

  readonly statusBadges = {
    [ReservationStatus.PENDING]: 'pending',
    [ReservationStatus.AVAILABLE]: 'available',
    [ReservationStatus.CANCELLED]: 'cancelled',
    [ReservationStatus.EXPIRED]: 'expired'
  };

  ngOnInit() {
    this.loadReservations();
  }

  loadReservations(): void {
    this.loading = true;
    this.error = null;

    this.reservationService.getReservations().subscribe({
      next: (response) => {
        this.reservations = response.items || response;
        this.applyFilters();
        this.loading = false;
      },
      error: (error: any) => {
        this.error = 'Erreur lors du chargement des réservations';
        this.loading = false;
        console.error('Error loading reservations:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = this.reservations;

    // Filtre par statut
    if (this.statusFilter !== 'all') {
      filtered = filtered.filter(reservation => reservation.status === this.statusFilter);
    }

    // Filtre par recherche
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(reservation =>
        reservation.bookTitle?.toLowerCase().includes(term) ||
        reservation.userName?.toLowerCase().includes(term) ||
        reservation.id?.toString().includes(term)
      );
    }

    this.filteredReservations = filtered;
    this.totalItems = filtered.length;
    this.currentPage = 1;
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
  }

  get paginatedReservations(): Reservation[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredReservations.slice(startIndex, endIndex);
  }

  // Gestion du statut
  openStatusModal(reservation: Reservation): void {
    this.selectedReservation = reservation;
    this.newStatus = reservation.status;
    this.availableSince = reservation.availableSince || '';
    this.showStatusModal = true;
  }

  closeStatusModal(): void {
    this.showStatusModal = false;
    this.selectedReservation = null;
    this.newStatus = '';
    this.availableSince = '';
  }

  updateStatus(): void {
    if (!this.selectedReservation) return;

    const statusRequest = {
      status: this.newStatus,
      availableSince: this.newStatus === ReservationStatus.AVAILABLE ? this.availableSince : undefined
    };

    this.reservationService.updateReservationStatus(this.selectedReservation.id, statusRequest).subscribe({
      next: () => {
        this.loadReservations();
        this.closeStatusModal();
      },
      error: (error: any) => {
        this.error = 'Erreur lors de la mise à jour du statut';
        console.error('Error updating reservation status:', error);
      }
    });
  }

  // Annulation de réservation
  openCancelModal(reservation: Reservation): void {
    this.selectedReservation = reservation;
    this.showCancelModal = true;
  }

  closeCancelModal(): void {
    this.showCancelModal = false;
    this.selectedReservation = null;
  }

  confirmCancel(): void {
    if (!this.selectedReservation) return;

    this.reservationService.cancelReservation(this.selectedReservation.id).subscribe({
      next: () => {
        this.loadReservations();
        this.closeCancelModal();
      },
      error: (error: any) => {
        this.error = 'Erreur lors de l\'annulation de la réservation';
        console.error('Error cancelling reservation:', error);
      }
    });
  }

  // Méthodes utilitaires
  getReservationStatusLabel(status: ReservationStatus): string {
    return this.statusLabels[status] || status;
  }

  getReservationStatusBadge(status: ReservationStatus): string {
    return this.statusBadges[status] || 'default';
  }

  isReservationExpired(reservation: Reservation): boolean {
    if (!reservation.expiryDate) return false;
    const expiryDate = new Date(reservation.expiryDate);
    const today = new Date();
    return expiryDate < today;
  }

  getPendingReservationsCount(): number {
    return this.filteredReservations.filter(r => r.status === ReservationStatus.PENDING).length;
  }

  getAvailableReservationsCount(): number {
    return this.filteredReservations.filter(r => r.status === ReservationStatus.AVAILABLE).length;
  }

  getExpiredReservationsCount(): number {
    return this.filteredReservations.filter(r => r.status === ReservationStatus.EXPIRED || this.isReservationExpired(r)).length;
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
}