
import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { IconComponent } from '../../atoms/icons/icon.component';

@Component({
  selector: 'app-modal',
  imports: [CommonModule, IconComponent],
  templateUrl: './modal.component.html',
  standalone: true
})
export class ModalComponent {
  @Input() isOpen = false;
  @Input() title: string | null = null;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() showCloseButton = true;
  @Input() closeOnBackdropClick = true;

  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<void>();

  @HostListener('document:keydown.escape', ['$event'])
  onEscapeKey(event: KeyboardEvent): void {
    if (this.isOpen) {
      event.preventDefault();
      this.onClose();
    }
  }

  onClose(): void {
    this.close.emit();
  }

  onConfirm(): void {
    this.confirm.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if (this.closeOnBackdropClick && (event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.onClose();
    }
  }
}
