import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonComponent } from '../../atoms/button/button.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { Book } from '../../../../core/models/book.model';
import { UserStateService } from '../../../../core/services/user-state.service';

@Component({
  selector: 'app-book-card',
  standalone: true,
  imports: [CommonModule, ButtonComponent, HeadingComponent, TypographyComponent, IconComponent],
  templateUrl: './book-card.component.html',
  styleUrls: ['./book-card.component.scss']
})
export class BookCardComponent {
  private userStateService = inject(UserStateService);
  private router = inject(Router);

  @Input() book!: Book;
  @Input() showActions = true;
  @Input() loading = false;

  @Output() borrow = new EventEmitter<string>();
  @Output() reserve = new EventEmitter<string>();
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();

  // Gestion des rôles
  get isAdminOrManager(): boolean {
    return this.userStateService.isAdminOrManager();
  }

  get isLoggedIn(): boolean {
    return this.userStateService.isLoggedIn();
  }

  get currentUserRole(): string {
    const user = this.userStateService.getCurrentUser();
    return user?.role || 'user';
  }

  // Actions utilisateur
  onBorrow(): void {
    this.borrow.emit(this.book.id);
  }

  onReserve(): void {
    this.reserve.emit(this.book.id);
  }

  // Actions admin/moderator
  onEdit(): void {
    this.edit.emit(this.book.id);
  }

  onDelete(): void {
    this.delete.emit(this.book.id);
  }

  onViewDetails(): void {
    this.router.navigate(['/books', this.book.id]);
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

  // Gestion des actions affichées
  get showUserActions(): boolean {
    return this.showActions && this.isLoggedIn && !this.isAdminOrManager;
  }

  get showAdminActions(): boolean {
    return this.showActions && this.isAdminOrManager;
  }
}