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
        const user = authService.getStoredUser();
        let userRoles: string[] = [];
        
        // Vérifier la structure de l'utilisateur
        if (user) {
          // Plusieurs façons possibles d'accéder au rôle
          const roleFromUser = (user as any).role || (user as any).roles || (user as any).user?.role;
          
          if (roleFromUser) {
            userRoles = Array.isArray(roleFromUser) ? roleFromUser : [roleFromUser];
          }
        }
        
        const hasRequiredRole = requiredRoles.some(requiredRole =>
          userRoles.some(userRole =>
            userRole?.toLowerCase() === requiredRole?.toLowerCase()
          )
        );
        
        if (!hasRequiredRole) {
            router.navigate(['/access-denied']);
            return false;
        }

        return true;
    };
};