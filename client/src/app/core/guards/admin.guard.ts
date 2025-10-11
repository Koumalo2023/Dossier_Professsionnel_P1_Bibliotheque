import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/api/auth.service";
import { inject } from "@angular/core";

export const adminGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    
    // Vérifier d'abord l'authentification
    if (!authService.isAuthenticated()) {
      router.navigate(['/login']);
      return false;
    }
    
    // Ensuite vérifier le rôle admin
    const isAdmin = authService.isAdmin();
    if (!isAdmin) {
      router.navigate(['/access-denied']);
      return false;
    }
    
    return true;
};