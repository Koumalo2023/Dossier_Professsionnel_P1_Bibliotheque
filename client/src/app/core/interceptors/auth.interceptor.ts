import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { StorageService } from '../services/storage/storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const storageService = inject(StorageService);
  
  // Récupérer le token depuis le service de stockage
  const token = storageService.getToken();
  
  if (token) {
    // Cloner la requête et ajouter l'en-tête Authorization
    const cloned = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(cloned);
  }
  
  // Si pas de token, passer la requête originale
  return next(req);
};