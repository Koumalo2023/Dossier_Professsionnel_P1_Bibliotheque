import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";

// auth.interceptor.ts
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const accessToken = authService.getAccessToken();
  
    if (accessToken) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
    }
  
    return next(req).pipe(
      catchError(error => {
        if (error.status === 401 && !req.url.includes('auth/refresh')) {
          return authService.refreshToken().pipe(
            switchMap(() => {
              const newReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${authService.getAccessToken()}`
                }
              });
              return next(newReq);
            }),
            catchError(refreshError => {
              authService.logout();
              return throwError(() => refreshError);
            })
          );
        }
        return throwError(() => error);
      })
    );
  };