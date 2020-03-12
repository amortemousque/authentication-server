import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Api, ApiScope } from '../models';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  apiUrl: string;

  constructor(private http: HttpClient, private auth: AuthService) {
    this.apiUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  searchApis(search: string): Observable<Api[]> {
    const params = {
      name: search
    }
    return this.http.get<Api[]>(`${this.apiUrl}/apis`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: params
    })
  }

  getApi(id): Observable<Api> {
    return this.http.get<Api>(`${this.apiUrl}/apis/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addApi(api: Api) {
    return this.http.post<any>(`${this.apiUrl}/apis/`, api, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateApi(api: Api) {
    return this.http.put<any>(`${this.apiUrl}/apis/${api.id}`, api, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

  deleteApi(id) {
    return this.http.delete<any>(`${this.apiUrl}/apis/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

   /* api scope */
   getApiScopes(apiId: string): Observable<ApiScope[]> {

    return this.http.get<ApiScope[]>(`${this.apiUrl}/apis/${apiId}/scopes`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addApiScope(apiScope: ApiScope) {
    return this.http.post<any>(`${this.apiUrl}/apis/${apiScope.apiResourceId}/scopes/`, apiScope, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateApiScope(apiScope: ApiScope) {
    return this.http.put<any>(`${this.apiUrl}/apis/${apiScope.apiResourceId}/scopes/${apiScope.id}`, apiScope, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

  deleteApiScope(apiScope: ApiScope) {
    return this.http.delete<any>(`${this.apiUrl}/apis/${apiScope.apiResourceId}/scopes/${apiScope.id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }


}



