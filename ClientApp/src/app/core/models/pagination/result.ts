import { HttpClient, HttpHeaders, HttpResponse, HttpParams} from '@angular/common/http';

export class PagingResult<T> {
    totalCount: number = null;
    results: T;

    mapHttpReponse(response: HttpResponse<T>) {
      const tc = response.headers.get('X-Total-Count');
      this.totalCount = tc ? Number(tc) : null;
      this.results = response.body;
    }
}
