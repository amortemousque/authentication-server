
​import { HttpErrorResponse } from '@angular/common/http';

export class DomainError implements Error {
    readonly name: string;
    readonly message: string;
    readonly field: string;
    readonly error: any | null;

    constructor(httpError: HttpErrorResponse) {
        const values = httpError.error.split('Parameter name:');
        this.field = values[1].trim();
        this.message = values[0].trim();
    }
}
