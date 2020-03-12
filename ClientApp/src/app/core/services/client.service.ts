import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Client } from '../models';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})export class ClientService {

  apiUrl: string;

  constructor(private http: HttpClient, private auth: AuthService) {
    this.apiUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  searchClients(search: string): Observable<Client[]> {
    const params = {
      name: search
    }
    return this.http.get<Client[]>(`${this.apiUrl}/clients`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader),
      params: params
    })
  }

  getClient(id): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/clients/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  addClient(client: Client): Observable<Client>  {
    return this.http.post<any>(`${this.apiUrl}/clients/`, client, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateClientSecret(id: string): Observable<Client>  {
    return this.http.put<any>(`${this.apiUrl}/clients/${id}/clientSecret`, {id:id}, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  updateClient(client: Client) {
    return this.http.put<any>(`${this.apiUrl}/clients/${client.id}`, client, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })
  }

  deleteClient(id) {
    return this.http.delete<any>(`${this.apiUrl}/clients/${id}`, {
      headers: new HttpHeaders().set('Authorization', this._authHeader)
    })

  }

   /* client */




}



