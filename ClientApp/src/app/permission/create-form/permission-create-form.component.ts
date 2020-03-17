import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import { BaseComponent } from '../../core/base.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Permission, DomainError } from '../../core/models';
import { catchError } from 'rxjs/operators';
import { AddPermission } from '../list/permission-list.state';
import { ReferenceService } from '../../shared/pipes/reference.service';
import { MessageService } from '../../shared/message/message.service';

@Component({
  selector: 'app-permission-create-form',
  templateUrl: './permission-create-form.component.html',
  styleUrls: ['./permission-create-form.component.scss']
})
export class PermissionCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;
  permissionCtrl = new FormControl();
  @ViewChild('permissionInput', {static: false}) permissionInput: ElementRef;

  constructor(
    private referenceService: ReferenceService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<PermissionCreateFormComponent>) {
      super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  mapForm(formGroup): Permission {
    const permission = new Permission();
    Object.assign(permission, formGroup.value);

    return permission;
  }


  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const permission = this.mapForm(this.formGroup);
      this.store.dispatch(new AddPermission(permission))
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
        this.dialogRef.close(permission);
      });
    }
  }
}

