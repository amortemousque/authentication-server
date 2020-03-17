import { Component, Inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import * as permissionActions from '../list/permission-list.state';
import { BaseComponent } from '../../core/base.component';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Permission, DomainError } from '../../core/models';
import { pipe } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-permission-create-form',
  templateUrl: './permission-create-form.component.html',
  styleUrls: ['./permission-create-form.component.scss']
})
export class PermissionCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;
  permissionCtrl = new FormControl();
  @ViewChild('permissionInput') permissionInput: ElementRef;

  constructor(
    private referenceService: ReferenceService,
    private utils: UtilsService,
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
      this.store.dispatch(new permissionActions.AddPermission(permission))
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
        this.utils.displaySnackMessage('saved')
        this.dialogRef.close(permission);
      });
    }
  }
}

