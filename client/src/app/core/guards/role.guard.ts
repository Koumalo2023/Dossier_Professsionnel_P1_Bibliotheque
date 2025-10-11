import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/api/auth.service";

export const roleGuard: (requiredRoles: string[]) => CanActivateFn = (requiredRoles) => {
    return (route, state) => {
        const authService = inject(AuthService);
        const router = inject(Router);

        if (!authService.isAuthenticated()) {
            router.navigate(['/login']);
            return false;
        }

        // Vérifier les rôles de l'utilisateur
        const user = authService.getCurrentUser();
        let userRoles: string[] = [];
        
        if (user) {
          // Si l'utilisateur est un Observable, on ne peut pas accéder directement aux rôles
          // Pour l'instant, on retourne false par sécurité
          return false;
        }
        
        const hasRequiredRole = requiredRoles.some(role => userRoles.includes(role));
        
        if (!hasRequiredRole) {
            router.navigate(['/access-denied']);
            return false;
        }

        return true;
    };
};