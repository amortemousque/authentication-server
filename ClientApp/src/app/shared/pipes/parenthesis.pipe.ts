import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'parenthesis'})
export class ParenthesisPipe implements PipeTransform {
  transform(value: string): string {
    if (value) {
        return '(' + value + ')';
    }
    return value;
  }
}
