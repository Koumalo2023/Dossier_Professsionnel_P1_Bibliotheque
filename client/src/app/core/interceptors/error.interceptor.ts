import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { EventBusService } from '../services/event-bus/event-bus.service';
import { StorageService } from '../services/storage/storage.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const eventBus = inject(EventBusService);
  const storageService = inject(StorageService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error) => {
      // Émettre un événement de loading terminé
      eventBus.emitLoadingFinished();

      let errorMessage = 'Une erreur est survenue';
      let shouldLogout = false;
      let shouldRedirect = false;
      let redirectTo = '/';

      // Analyser le type d'erreur
      if (error.status) {
        switch (error.status) {
          case 0:
            // Erreur réseau
            errorMessage = 'Erreur de connexion. Vérifiez votre connexion internet.';
            break;
          
          case 400:
            // Mauvaise requête
            errorMessage = error.error?.message || 'Requête invalide';
            break;
          
          case 401:
            // Non autorisé
            errorMessage = 'Session expirée. Veuillez vous reconnecter.';
            shouldLogout = true;
            shouldRedirect = true;
            redirectTo = '/auth/login';
            break;
          
          case 403:
            // Accès refusé
            errorMessage = 'Accès refusé. Vous n\'avez pas les permissions nécessaires.';
            shouldRedirect = true;
            redirectTo = '/access-denied';
            break;
          
          case 404:
            // Ressource non trouvée
            errorMessage = error.error?.message || 'Ressource non trouvée';
            break;
          
          case 409:
            // Conflit
            errorMessage = error.error?.message || 'Conflit de données';
            break;
          
          case 422:
            // Erreur de validation
            errorMessage = 'Données invalides. Veuillez vérifier les informations saisies.';
            if (error.error?.errors) {
              // Formater les erreurs de validation
              const validationErrors = Object.values(error.error.errors).flat();
              errorMessage += `\n${validationErrors.join('\n')}`;
            }
            break;
          
          case 429:
            // Trop de requêtes
            errorMessage = 'Trop de tentatives. Veuillez réessayer plus tard.';
            break;
          
          case 500:
            // Erreur serveur
            errorMessage = 'Erreur interne du serveur. Veuillez réessayer plus tard.';
            break;
          
          case 502:
          case 503:
          case 504:
            // Erreurs de gateway/timeout
            errorMessage = 'Service temporairement indisponible. Veuillez réessayer plus tard.';
            break;
          
          default:
            // Autres erreurs
            errorMessage = error.error?.message || `Erreur ${error.status}`;
            break;
        }
      } else if (error.name === 'TimeoutError') {
        // Timeout
        errorMessage = 'La requête a expiré. Veuillez réessayer.';
      } else if (error.name === 'NetworkError') {
        // Erreur réseau
        errorMessage = 'Erreur de réseau. Vérifiez votre connexion internet.';
      }

      // Émettre l'événement d'erreur
      eventBus.emit('HTTP_ERROR', {
        message: errorMessage,
        status: error.status,
        url: req.url,
        method: req.method,
        timestamp: new Date().toISOString()
      });

      // Gérer la déconnexion si nécessaire
      if (shouldLogout) {
        storageService.removeToken();
        storageService.removeUser();
        eventBus.emitUserLogout();
      }

      // Gérer la redirection si nécessaire
      if (shouldRedirect) {
        router.navigate([redirectTo]);
      }

      // Log en développement
      if (!environment.production) {
        console.error('HTTP Error Interceptor:', {
          url: req.url,
          method: req.method,
          status: error.status,
          message: errorMessage,
          error: error
        });
      }

      // Propager l'erreur
      return throwError(() => ({
        ...error,
        userMessage: errorMessage,
        handled: true
      }));
    })
  );
};

// Import d'environnement (à créer si nécessaire)
const environment = {
  production: false
};