import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastService, Toast } from './core/services/toast.service';
import { ToastComponent } from './shared/components/organisms/toast/toast.component';

@Component({
    selector: 'app-root',
    imports: [CommonModule, RouterOutlet, ToastComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'library-management-client';
  private toastService = inject(ToastService);
  
  toasts: Toast[] = [];
  private toastSubscription: any;

  constructor() {}

  ngOnInit(): void {
    console.log('AppComponent: Initializing toast subscription');
    this.toastSubscription = this.toastService.toasts$.subscribe(toasts => {
      console.log('AppComponent: Received toasts update:', toasts);
      this.toasts = toasts;
    });
  }

  ngOnDestroy(): void {
    if (this.toastSubscription) {
      this.toastSubscription.unsubscribe();
    }
  }

  onToastClose(toastId: number): void {
    this.toastService.removeToast(toastId);
  }
  
}
