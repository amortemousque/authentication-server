import { Pipe, PipeTransform } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TenantService } from '../../core/services';

@Pipe({name: 'tenant'})
export class TenantPipe implements PipeTransform {

  constructor(public tenantService: TenantService) {
  }

  transform(value: number): Observable<string> {
    return this.tenantService.getTenant(value).pipe(map(tenant => {
      return tenant.name
    }))
  }
}

