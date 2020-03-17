import { Component, Inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import * as roleActions from '../list/role-list.state';
import { BaseComponent } from '../../core/base.component';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Role, DomainError } from '../../core/models';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-role-create-form',
  templateUrl: './role-create-form.component.html',
  styleUrls: ['./role-create-form.component.scss']
})
export class RoleCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;
  roleCtrl = new FormControl();
  @ViewChild('roleInput') roleInput: ElementRef;

  constructor(
    private utils: UtilsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<RoleCreateFormComponent>) {
      super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  mapForm(formGroup): Role {
    const role = new Role();
    Object.assign(role, formGroup.value);

    return role;
  }


  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const role = this.mapForm(this.formGroup);
      this.store.dispatch(new roleActions.AddRole(role))
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
        this.dialogRef.close(role);
      });
    }
  }
}

