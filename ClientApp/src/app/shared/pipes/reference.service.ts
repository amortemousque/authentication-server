import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { publishReplay, refCount, map } from 'rxjs/operators';
import { AuthService } from '../../core/auth/auth.service';


export type CollectionType = 
    'clientTypes' | 'allowedScopes'

@Injectable({
  providedIn: "root"
})
export class ReferenceService {

  apiUrl: string;
  interviewApiUrl: string;
  companyApiUrl: string;

  private _references: Array<Observable<any[]>> = [];

  constructor(private http: HttpClient, private auth: AuthService) {
    this.apiUrl = environment.adminApiUrl;
  }

  private get _authHeader(): string {
    return `Bearer ${this.auth.access_token}`;
  }

  getReference(name: CollectionType): Observable<any[]> {
    if (!this._references[name]) {
        this._references[name] = this.http.get<any>(`${this.apiUrl}/collections/${name}`, {
          headers: new HttpHeaders().set('Authorization', this._authHeader),
        }).pipe(
        publishReplay(1),refCount());
    }
    return this._references[name];
  }  
}
