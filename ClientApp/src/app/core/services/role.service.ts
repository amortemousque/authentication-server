import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Role, Permission } from '../models';
import { AuthService } from '../auth/auth.service';


@Injectable({
  providedIn: 'root'
})
export class RoleService {

  roleUrl: string;

  constructor(private http: HttpClient, private auth: AuthService) {
    this.roleUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  searchRoles(search: string): Observable<Role[]> {
    const params = {
      name: search
    }
    return this.http.get<Role[]>(`${this.roleUrl}/roles`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: params
    })
  }

  getRole(id): Observable<Role> {
    return this.http.get<Role>(`${this.roleUrl}/roles/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addRole(role: Role) {
    return this.http.post<any>(`${this.roleUrl}/roles/`, role, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateRole(role: Role) {
    return this.http.put<any>(`${this.roleUrl}/roles/${role.id}`, role, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

  deleteRole(id) {
    return this.http.delete<any>(`${this.roleUrl}/roles/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }


  // role permissions

  getPermissions(apiId: string): Observable<Permission[]> {
    return this.http.get<Permission[]>(`${this.roleUrl}/roles/${apiId}/permissions`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addPermissions(rolePermissions: Permission[]) {
    const params = {
      roleId: rolePermissions[0].roleId,
      permissions: rolePermissions.map(p => p.name)
    }
    return this.http.post<any>(`${this.roleUrl}/roles/${params.roleId}/permissions/`, params, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  deletePermission(rolePermission: Permission) {
    return this.http.delete<any>(`${this.roleUrl}/roles/${rolePermission.roleId}/permissions/${rolePermission.id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }
}



