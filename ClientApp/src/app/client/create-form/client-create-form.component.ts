import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import { BaseComponent } from '../../core/base.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Client, DomainError } from '../../core/models';
import { catchError } from 'rxjs/operators';
import { AddClient } from '../list/client-list.state';
import { MessageService } from '../../shared/message/message.service';

@Component({
  selector: 'app-client-create-form',
  templateUrl: './client-create-form.component.html',
  styleUrls: ['./client-create-form.component.scss']
})
export class ClientCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;

  constructor(
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public dialogRef: MatDialogRef<ClientCreateFormComponent>) {
    super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      id: [null],
      clientName: ['', Validators.required],
      clientTypeId: ['', Validators.required]
    });
  }

  mapForm(formGroup): Client {
    const client = new Client();
    Object.assign(client, formGroup.value);

    return client;
  }

  setType(value) {
    this.formGroup.patchValue({ clientTypeId: value })
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const client = this.mapForm(this.formGroup);
      this.store.dispatch(new AddClient(client))
        .pipe(catchError(err => {
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
          this.dialogRef.close(client);
        })
    }
  }
}

