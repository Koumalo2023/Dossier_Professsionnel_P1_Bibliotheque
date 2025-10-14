import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { IconComponent, IconColor } from '../../atoms/icons/icon.component';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
  duration?: number;
}
@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [TypographyComponent, IconComponent],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss'
})
export class ToastComponent {
  @Input() toast!: Toast;
  @Output() close = new EventEmitter<number>();

  getIcon(): string {
    switch (this.toast.type) {
      case 'success': return 'fa-check-circle';
      case 'error': return 'fa-exclamation-triangle';
      case 'warning': return 'fa-exclamation-circle';
      default: return 'fa-info-circle';
    }
  }

  getColor(): IconColor {
    switch (this.toast.type) {
      case 'success': return 'success';
      case 'error': return 'danger';
      case 'warning': return 'warning';
      default: return 'primary';
    }
  }

  onClose(): void {
    this.close.emit(this.toast.id);
  }
}
