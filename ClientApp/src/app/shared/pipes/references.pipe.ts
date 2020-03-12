import { Pipe, PipeTransform } from '@angular/core';
import { Observable } from 'rxjs';
import { ReferenceService } from './reference.service';

@Pipe({name: 'references'})
export class ReferencesPipe implements PipeTransform {

  constructor(public referenceService: ReferenceService) {
  }

  transform(values: string[], collectionName: string, key: string = 'id', separator: ','): Observable<string> {
    const functionName = 'get' + collectionName.substr(0, 1).toUpperCase() + collectionName.substr(1);
    return this.referenceService[functionName]().map(collection => {
      return collection.filter(tt => values.includes(tt[key])).map(tt => tt.name).join(', ');
    })
  }
}
