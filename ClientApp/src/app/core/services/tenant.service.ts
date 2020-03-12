import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Tenant } from '../models';
import { AuthService } from '../auth/auth.service';


@Injectable({
  providedIn: 'root'
})
export class TenantService {

  tenantUrl: string;

  constructor(private http: HttpClient, private auth: AuthService) {
    this.tenantUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  searchTenants(search: string): Observable<Tenant[]> {
    const params = {
      name: search
    }
    return this.http.get<Tenant[]>(`${this.tenantUrl}/tenants`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: params
    })
  }

  getTenant(id): Observable<Tenant> {
    return this.http.get<Tenant>(`${this.tenantUrl}/tenants/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addTenant(tenant: Tenant) {
    return this.http.post<any>(`${this.tenantUrl}/tenants/`, tenant, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateTenant(tenant: Tenant) {
    return this.http.put<any>(`${this.tenantUrl}/tenants/${tenant.id}`, tenant, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

  deleteTenant(id) {
    return this.http.delete<any>(`${this.tenantUrl}/tenants/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }
}



