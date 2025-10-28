import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expected = route.data['role'] as 'superadmin' | 'admin' | 'user' | undefined;

    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }

    // super admin bypasses all checks
    if (this.auth.isSuperAdmin()) return true;

    if (!expected) return true; // no role required

    switch (expected) {
      case 'admin':
        if (this.auth.isAdmin()) return true;
        break;
      case 'user':
        if (this.auth.isUser()) return true;
        break;
      case 'superadmin':
        if (this.auth.isSuperAdmin()) return true;
        break;
    }

    // unauthorized
    this.router.navigate(['/access-denied']);
    return false;
  }
}
