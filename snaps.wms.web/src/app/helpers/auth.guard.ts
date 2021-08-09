import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { authService } from '../auth/services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authSv: authService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const isAuthen = this.authSv.isAuthen;
        if (isAuthen) {
            return true;
        }else { 
            this.router.navigate(['/signin'], { queryParams: { returnUrl: state.url } });
            return false;
        }
    }
}