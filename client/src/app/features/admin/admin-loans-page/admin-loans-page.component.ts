import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { LoanService } from '../../../core/services/api/loan.service';
import { NotificationService } from '../../../core/services/api/notification.service';
import { Loan, LoanStatus } from '../../../core/models/loan.model';
import { CreateNotificationRequest, NotificationType } from '../../../core/models/notification.model';
import { LoanHistoryTableComponent } from '../../../shared/components/organisms/loan-history-table/loan-history-table.component';
import { PaginatorComponent } from '../../../shared/components/organisms/paginator/paginator.component';

@Component({
  selector: 'app-admin-loans-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoanHistoryTableComponent,
    PaginatorComponent
  ],
  templateUrl: './admin-loans-page.component.html'
})
export class AdminLoansPageComponent implements OnInit {
  private loanService = inject(LoanService);
  private notificationService = inject(NotificationService);
  private http = inject(HttpClient);

  loans: Loan[] = [];
  filteredLoans: Loan[] = [];
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
  showReturnModal = false;
  showNotificationModal = false;
  selectedLoan: Loan | null = null;
  notificationMessage = '';

  // Filtres de statut
  readonly statusOptions = [
    { value: 'all', label: 'Tous les emprunts' },
    { value: 'active', label: 'Emprunts actifs' },
    { value: 'returned', label: 'Emprunts retournés' },
    { value: 'overdue', label: 'Emprunts en retard' }
  ];

  ngOnInit() {
    this.loadLoans();
  }

  loadLoans(): void {
    this.loading = true;
    this.error = null;

    this.loanService.getLoans().subscribe({
      next: (response) => {
        this.loans = response.items || response;
        this.applyFilters();
        this.loading = false;
      },
      error: (error: any) => {
        this.error = 'Erreur lors du chargement des emprunts';
        this.loading = false;
        console.error('Error loading loans:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = this.loans;

    // Filtre par statut
    if (this.statusFilter !== 'all') {
      filtered = filtered.filter(loan => {
        switch (this.statusFilter) {
          case 'active':
            return loan.status === LoanStatus.ACTIVE;
          case 'returned':
            return loan.status === LoanStatus.RETURNED;
          case 'overdue':
            return this.isLoanOverdue(loan);
          default:
            return true;
        }
      });
    }

    // Filtre par recherche
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(loan =>
        loan.bookTitle?.toLowerCase().includes(term) ||
        loan.userName?.toLowerCase().includes(term) ||
        loan.id?.toString().includes(term)
      );
    }

    this.filteredLoans = filtered;
    this.totalItems = filtered.length;
    this.currentPage = 1;
  }

  isLoanOverdue(loan: Loan): boolean {
    if (!loan.dueDate || loan.status === LoanStatus.RETURNED) return false;
    const dueDate = new Date(loan.dueDate);
    const today = new Date();
    return dueDate < today;
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

  get paginatedLoans(): Loan[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredLoans.slice(startIndex, endIndex);
  }

  // Gestion des retours
  openReturnModal(loan: Loan): void {
    this.selectedLoan = loan;
    this.showReturnModal = true;
  }

  closeReturnModal(): void {
    this.showReturnModal = false;
    this.selectedLoan = null;
  }

  confirmReturn(): void {
    if (!this.selectedLoan) return;

    this.loanService.returnLoan(this.selectedLoan.id).subscribe({
      next: () => {
        this.loadLoans();
        this.closeReturnModal();
      },
      error: (error: any) => {
        this.error = 'Erreur lors du retour du livre';
        console.error('Error returning loan:', error);
      }
    });
  }

  // Gestion des notifications
  openNotificationModal(loan: Loan): void {
    this.selectedLoan = loan;
    this.notificationMessage = this.generateDefaultNotification(loan);
    this.showNotificationModal = true;
  }

  closeNotificationModal(): void {
    this.showNotificationModal = false;
    this.selectedLoan = null;
    this.notificationMessage = '';
  }

  generateDefaultNotification(loan: Loan): string {
    if (this.isLoanOverdue(loan)) {
      return `Cher ${loan.userName}, votre emprunt du livre "${loan.bookTitle}" est en retard. Merci de le retourner dès que possible.`;
    }
    return `Cher ${loan.userName}, votre emprunt du livre "${loan.bookTitle}" arrive bientôt à échéance.`;
  }

  sendNotification(): void {
    if (!this.selectedLoan || !this.notificationMessage.trim()) return;

    const notification: CreateNotificationRequest = {
      title: 'Notification de bibliothèque',
      message: this.notificationMessage,
      type: NotificationType.REMINDER,
      userId: this.selectedLoan.userId,
      relatedEntityId: this.selectedLoan.id,
      relatedEntityType: 'loan'
    };

    this.notificationService.createNotification(notification).subscribe({
      next: () => {
        this.closeNotificationModal();
      },
      error: (error: any) => {
        this.error = 'Erreur lors de l\'envoi de la notification';
        console.error('Error sending notification:', error);
      }
    });
  }

  sendOverdueNotification(loan: Loan): void {
    this.loanService.notifyOverdueLoan(loan.id).subscribe({
      next: () => {
        // Notification envoyée avec succès
      },
      error: (error: any) => {
        this.error = 'Erreur lors de l\'envoi de la notification de retard';
        console.error('Error sending overdue notification:', error);
      }
    });
  }

  getLoanStatusBadge(loan: Loan): string {
    if (loan.status === LoanStatus.RETURNED) return 'returned';
    if (this.isLoanOverdue(loan)) return 'overdue';
    return 'active';
  }

  getLoanStatusLabel(loan: Loan): string {
    if (loan.status === LoanStatus.RETURNED) return 'Retourné';
    if (this.isLoanOverdue(loan)) return 'En retard';
    return 'Actif';
  }

  getActiveLoansCount(): number {
    return this.filteredLoans.filter(loan => this.getLoanStatusBadge(loan) === 'active').length;
  }

  getOverdueLoansCount(): number {
    return this.filteredLoans.filter(loan => this.getLoanStatusBadge(loan) === 'overdue').length;
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
}