import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Permission } from '../models';
import { AuthService } from '../auth/auth.service';


@Injectable({
  providedIn: 'root'
})export class PermissionService {

  permissionUrl: string;

  constructor(private http: HttpClient, private auth: AuthService) {
    this.permissionUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  searchPermissions(search: string): Observable<Permission[]> {
    const params = {
      name: search
    }
    return this.http.get<Permission[]>(`${this.permissionUrl}/permissions`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: params
    })
  }

  getPermission(id): Observable<Permission> {
    return this.http.get<Permission>(`${this.permissionUrl}/permissions/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addPermission(permission: Permission) {
    return this.http.post<any>(`${this.permissionUrl}/permissions/`, permission, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updatePermission(permission: Permission) {
    return this.http.put<any>(`${this.permissionUrl}/permissions/${permission.id}`, permission, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

  deletePermissions(permissions: Permission[]) {
    const ids = permissions.map(p => p.id);
    return this.http.delete<any>(`${this.permissionUrl}/permissions/`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: {
        ids : ids
      }
    })
  }


}



