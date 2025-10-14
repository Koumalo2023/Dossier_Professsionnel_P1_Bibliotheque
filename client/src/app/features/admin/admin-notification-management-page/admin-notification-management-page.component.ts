import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../../core/services/api/notification.service';
import { Notification, NotificationType, CreateNotificationRequest } from '../../../core/models/notification.model';
import { NotificationPanelComponent } from '../../../shared/components/organisms/notification-panel/notification-panel.component';
import { PaginatorComponent } from '../../../shared/components/organisms/paginator/paginator.component';

@Component({
  selector: 'app-admin-notification-management-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NotificationPanelComponent,
    PaginatorComponent
  ],
  templateUrl: './admin-notification-management-page.component.html'
})
export class AdminNotificationManagementPageComponent implements OnInit {
  private notificationService = inject(NotificationService);

  notifications: Notification[] = [];
  filteredNotifications: Notification[] = [];
  loading = false;
  error: string | null = null;

  // Filtres
  typeFilter: string = 'all';
  searchTerm: string = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;

  // Modal state
  showCreateModal = false;
  showDeleteModal = false;
  selectedNotification: Notification | null = null;

  // Formulaire de création
  newNotification: CreateNotificationRequest = {
    title: '',
    message: '',
    type: NotificationType.INFO
  };

  // Filtres de type
  readonly typeOptions = [
    { value: 'all', label: 'Tous les types' },
    { value: NotificationType.REMINDER, label: 'Rappel' },
    { value: NotificationType.INFO, label: 'Information' },
    { value: NotificationType.WARNING, label: 'Avertissement' },
    { value: NotificationType.SUCCESS, label: 'Succès' }
  ];

  readonly typeLabels = {
    [NotificationType.REMINDER]: 'Rappel',
    [NotificationType.INFO]: 'Information',
    [NotificationType.WARNING]: 'Avertissement',
    [NotificationType.SUCCESS]: 'Succès'
  };

  readonly typeBadges = {
    [NotificationType.REMINDER]: 'reminder',
    [NotificationType.INFO]: 'info',
    [NotificationType.WARNING]: 'warning',
    [NotificationType.SUCCESS]: 'success'
  };

  // Modèles de notification prédéfinis
  readonly notificationTemplates = [
    {
      name: 'Rappel de retour',
      title: 'Rappel de retour de livre',
      message: 'Cher utilisateur, votre livre "{bookTitle}" doit être retourné avant le {dueDate}. Merci de le rapporter à la bibliothèque.',
      type: NotificationType.REMINDER
    },
    {
      name: 'Livre disponible',
      title: 'Votre livre est disponible',
      message: 'Bonjour {userName}, le livre "{bookTitle}" que vous avez réservé est maintenant disponible. Vous pouvez venir le récupérer à la bibliothèque.',
      type: NotificationType.INFO
    },
    {
      name: 'Retard de retour',
      title: 'Retard de retour de livre',
      message: 'Cher utilisateur, votre livre "{bookTitle}" est en retard. Des frais de retard peuvent s\'appliquer. Merci de le retourner dès que possible.',
      type: NotificationType.WARNING
    },
    {
      name: 'Objectif atteint',
      title: 'Félicitations !',
      message: 'Félicitations {userName} ! Vous avez atteint votre objectif de lecture pour ce mois.',
      type: NotificationType.SUCCESS
    }
  ];

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.loading = true;
    this.error = null;

    this.notificationService.getNotifications().subscribe({
      next: (response: any) => {
        this.notifications = response.items || response;
        this.applyFilters();
        this.loading = false;
      },
      error: (error: any) => {
        this.error = 'Erreur lors du chargement des notifications';
        this.loading = false;
        console.error('Error loading notifications:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = this.notifications;

    // Filtre par type
    if (this.typeFilter !== 'all') {
      filtered = filtered.filter(notification => notification.type === this.typeFilter);
    }

    // Filtre par recherche
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(notification =>
        notification.title?.toLowerCase().includes(term) ||
        notification.message?.toLowerCase().includes(term) ||
        notification.id?.toString().includes(term)
      );
    }

    this.filteredNotifications = filtered;
    this.totalItems = filtered.length;
    this.currentPage = 1;
  }

  onTypeFilterChange(): void {
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
  }

  get paginatedNotifications(): Notification[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredNotifications.slice(startIndex, endIndex);
  }

  // Création de notification
  openCreateModal(): void {
    this.newNotification = {
      title: '',
      message: '',
      type: NotificationType.INFO
    };
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.newNotification = {
      title: '',
      message: '',
      type: NotificationType.INFO
    };
  }

  useTemplate(template: any): void {
    this.newNotification.title = template.title;
    this.newNotification.message = template.message;
    this.newNotification.type = template.type;
  }

  createNotification(): void {
    if (!this.newNotification.title.trim() || !this.newNotification.message.trim()) {
      this.error = 'Le titre et le message sont obligatoires';
      return;
    }

    this.notificationService.createNotification(this.newNotification).subscribe({
      next: () => {
        this.loadNotifications();
        this.closeCreateModal();
      },
      error: (error: any) => {
        this.error = 'Erreur lors de la création de la notification';
        console.error('Error creating notification:', error);
      }
    });
  }

  // Suppression de notification
  openDeleteModal(notification: Notification): void {
    this.selectedNotification = notification;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.selectedNotification = null;
  }

  confirmDelete(): void {
    if (!this.selectedNotification) return;

    // Note: Le service de suppression n'est pas encore implémenté dans le service actuel
    // Pour l'instant, nous affichons juste un message
    this.error = 'La fonctionnalité de suppression n\'est pas encore disponible';
    this.closeDeleteModal();
  }

  // Marquer comme lu/non lu
  toggleReadStatus(notification: Notification): void {
    if (notification.isRead) {
      // Marquer comme non lu - fonctionnalité non disponible dans le service actuel
      this.error = 'La fonctionnalité de marquage comme non lu n\'est pas encore disponible';
    } else {
      this.notificationService.markAsRead(notification.id).subscribe({
        next: () => {
          this.loadNotifications();
        },
        error: (error: any) => {
          this.error = 'Erreur lors du marquage comme lu';
          console.error('Error marking as read:', error);
        }
      });
    }
  }

  // Méthodes utilitaires
  getNotificationTypeLabel(type: NotificationType): string {
    return this.typeLabels[type] || type;
  }

  getNotificationTypeBadge(type: NotificationType): string {
    return this.typeBadges[type] || 'default';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getReminderCount(): number {
    return this.filteredNotifications.filter(n => n.type === NotificationType.REMINDER).length;
  }

  getInfoCount(): number {
    return this.filteredNotifications.filter(n => n.type === NotificationType.INFO).length;
  }

  getWarningCount(): number {
    return this.filteredNotifications.filter(n => n.type === NotificationType.WARNING).length;
  }

  getUnreadCount(): number {
    return this.filteredNotifications.filter(n => !n.isRead).length;
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
}