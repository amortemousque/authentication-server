import { NgxPermissionsService, NgxRolesService } from 'ngx-permissions';
import { combineLatest } from 'rxjs';
import { Store } from '@ngxs/store';
import { map, filter, switchMap } from 'rxjs/operators';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { AuthenticationState } from '../auth/auth.state';
import { environment } from '../../../environments/environment';

export function PermissionInitializerFactory(ps: NgxPermissionsService, rs: NgxRolesService, http: HttpClient, store: Store) {
  return () => new Promise<any>((resolve: any) => {
    combineLatest(store.select(AuthenticationState.valid), store.select(AuthenticationState.accessToken)).pipe(
      map(data => ({valid: data[0], accessToken: data[1]})),
      filter(({valid, accessToken}) => valid && accessToken != null),
      switchMap(({accessToken}) => http.get<any>(`${environment.baseUri}/users/me/permissions`, { headers: new HttpHeaders().set('Authorization', `Bearer ${accessToken}`)}))
    ).subscribe((permissions: Array<string>) => {
      ps.loadPermissions(permissions);
      console.info(`Successfully initialized permissions.`);
      resolve(null);
    });
  });
}