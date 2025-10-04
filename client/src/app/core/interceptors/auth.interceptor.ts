import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";
import { LoginResponse } from "../models/user.model";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const router = inject(Router);
  
    // Ne pas intercepter les requêtes de rafraîchissement
    if (req.url.includes('/Auth/refresh')) {
      return next(req);
    }
  
    const accessToken = authService.getAccessToken();
    let authReq = req;
    if (accessToken) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
    }
  
    return next(authReq).pipe(
      catchError(error => {
        if (error.status === 401 && !req.url.includes('/Auth/refresh')) {
          // Tenter de rafraîchir le token
          return authService.refreshToken().pipe(
            switchMap((response: LoginResponse) => {
              // Recréer la requête avec le nouveau token
              const newReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${response.token}`
                }
              });
              return next(newReq);
            }),
            catchError(refreshError => {
              // En cas d'erreur de rafraîchissement, déconnecter et rediriger
              authService.logout();
              router.navigate(['/login']);
              return throwError(() => refreshError);
            })
          );
        } else if (error.status === 401) {
          // Si c'est une erreur 401 sur l'endpoint de rafraîchissement, déconnecter
          authService.logout();
          router.navigate(['/login']);
        }
        return throwError(() => error);
      })
    );
};