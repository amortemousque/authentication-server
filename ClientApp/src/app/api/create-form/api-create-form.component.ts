import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';
import { catchError } from 'rxjs/operators'
import { BaseComponent } from '../../core/base.component';
import { Api, DomainError } from '../../core/models';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MessageService } from '../../shared/message/message.service';
import { AddApi } from '../list/api-list.state';


@Component({
  selector: 'app-api-create-form',
  templateUrl: './api-create-form.component.html',
  styleUrls: ['./api-create-form.component.scss']
})
export class ApiCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;
  schoolCtrl: FormControl;

  loading = false;

  constructor(
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    private messageService: MessageService,
    public dialogRef: MatDialogRef<ApiCreateFormComponent>) {
    super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      id: [null],
      name: ['', Validators.required],
      displayName: ['', Validators.required],
    });
  }

  mapForm(formGroup): Api {
    const api = new Api();
    Object.assign(api, formGroup.value);

    return api;
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const api = this.mapForm(this.formGroup);
      this.store.dispatch(new AddApi(api)).pipe(
        catchError(err => {
          if (err instanceof DomainError) {
            this.formGroup.get(err.field).setErrors({
              notUnique: true
            });
            this.loading = false;
          }
          return err;
        }))
        .subscribe(r => {
          this.loading = false;
          this.messageService.openSuccessMessage('saved')
          this.dialogRef.close(api);
        });
    }
  }

}

