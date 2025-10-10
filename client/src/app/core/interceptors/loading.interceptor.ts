import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { EventBusService } from '../services/event-bus/event-bus.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const eventBus = inject(EventBusService);

  // Ignorer les requêtes de monitoring ou de santé
  if (req.url.includes('/health') || req.url.includes('/ping')) {
    return next(req);
  }

  // Émettre l'événement de début de loading
  eventBus.emitLoadingStarted();

  // Créer un timeout pour les requêtes longues
  const timeoutDuration = 30000; // 30 secondes
  let timeoutId: any;

  const timeoutPromise = new Promise((_, reject) => {
    timeoutId = setTimeout(() => {
      reject({
        name: 'TimeoutError',
        message: 'La requête a expiré',
        url: req.url,
        method: req.method
      });
    }, timeoutDuration);
  });

  // Exécuter la requête HTTP et gérer le timeout
  const request$ = next(req).pipe(
    finalize(() => {
      // Nettoyer le timeout
      clearTimeout(timeoutId);
      // Émettre l'événement de fin de loading
      eventBus.emitLoadingFinished();
    })
  );

  // Créer une promesse race entre la requête et le timeout
  return new Promise((resolve, reject) => {
    const subscription = request$.subscribe({
      next: (value) => resolve(value),
      error: (error) => reject(error)
    });

    timeoutPromise.catch((timeoutError) => {
      subscription.unsubscribe();
      // Émettre un événement d'erreur de timeout
      eventBus.emit('REQUEST_TIMEOUT', {
        url: req.url,
        method: req.method,
        duration: timeoutDuration,
        timestamp: new Date().toISOString()
      });
      reject(timeoutError);
    });
  }) as any;
};