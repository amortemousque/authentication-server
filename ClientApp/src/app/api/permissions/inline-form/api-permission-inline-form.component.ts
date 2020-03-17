import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngxs/store';

import * as apiActions from '../api-permissions.state';
import { BaseComponent } from '../../../core/base.component';
import { ApiScope, DomainError } from '../../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MessageService } from '../../../shared/message/message.service';



@Component({
  selector: 'app-api-permission-inline-form',
  templateUrl: './api-permission-inline-form.component.html'
})
export class ApiPermissionInlineFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;
  loading = false;

  @Output() cancelled: EventEmitter<ApiScope> = new EventEmitter();

  @Input()
  set apiScope(apiScope: ApiScope) {
    this._apiScope = apiScope;
  }

  companyLogoApiUrl: string;
  _apiScope: ApiScope;

  constructor(
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
      super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      apiResourceId: [null],
      name: [''],
      description: ['']
    });
    this.formGroup.patchValue(this._apiScope);
  }

  mapForm(formGroup): ApiScope {
    const api = new ApiScope();
    Object.assign(api, formGroup.value);

    return api;
  }

  setType(value) {
    this.formGroup.patchValue({apiTypeId: value})
  }

  cancelEdition() {
    (this._apiScope as any).isEditing = false;
    this.cancelled.emit(this._apiScope);
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const api = this.mapForm(this.formGroup);
      let promise = null;
      if (this._apiScope.id) {
        promise = this.store.dispatch(new apiActions.UpdateApiScope(api))
      } else {
        promise = this.store.dispatch(new apiActions.AddApiScope(api))
      }
      promise
      .catch(err => {
        if (err instanceof DomainError) {
          this.formGroup.get(err.field).setErrors({
            notUnique: true
          });
          this.loading = false;
        }
        return err;
       })
      .subscribe(() => {
          this.loading = false;
          this.messageService.openSuccessMessage('saved');
        });
    }
  }

}

