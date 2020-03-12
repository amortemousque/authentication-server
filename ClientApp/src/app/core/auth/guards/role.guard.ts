import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Injectable({
    providedIn: 'root'
})
export class RoleGuard implements CanActivate {

    constructor(private router: Router, private auth: AuthService) {
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        // on the data property
        const authorizedRoles = route.data.authorizedRoles;
        if (authorizedRoles) {
            for(var role of authorizedRoles)    {
                if (this.auth.isInRole(role)) {
                    return true;
                }
            }
        }
        this.router.navigate(['/unauthorized']);
        return true;
    }
}
