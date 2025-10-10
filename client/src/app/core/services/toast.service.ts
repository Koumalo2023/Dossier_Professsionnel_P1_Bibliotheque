import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, filter, take } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
  duration?: number; // en ms, par défaut 4000
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  public toasts$ = this.toastsSubject.asObservable();

  private idCounter = 0;

  success(message: string, duration: number = 4000): void {
    this.addToast(message, 'success', duration);
  }

  error(message: string, duration: number = 5000): void {
    this.addToast(message, 'error', duration);
  }

  info(message: string, duration: number = 4000): void {
    this.addToast(message, 'info', duration);
  }

  warning(message: string, duration: number = 4000): void {
    this.addToast(message, 'warning', duration);
  }

  private addToast(message: string, type: ToastType, duration: number): void {
    const id = ++this.idCounter;
    const toast: Toast = { id, message, type, duration };

    // Ajouter le toast
    this.toastsSubject.next([...this.toastsSubject.value, toast]);

    // Supprimer automatiquement après `duration`
    setTimeout(() => {
      this.removeToast(id);
    }, duration);
  }

  removeToast(id: number): void {
    const current = this.toastsSubject.value;
    const updated = current.filter(toast => toast.id !== id);
    this.toastsSubject.next(updated);
  }

  clearAll(): void {
    this.toastsSubject.next([]);
  }
}