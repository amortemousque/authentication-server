import { Pipe, PipeTransform } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CollectionType, ReferenceService } from './reference.service';

@Pipe({name: 'reference'})
export class ReferencePipe implements PipeTransform {

  constructor(public referenceService: ReferenceService) {
  }

  transform(value: string | number, collectionName: CollectionType, key: string = 'id', displayValue: string = 'name'): Observable<string> {
    return this.referenceService.getReference(collectionName).pipe(map(collection => {
      const ref = collection.find(l => l[key] == value);
      return ref ? ref[displayValue] : '';
    }))
  }
}

