import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import * as tenantActions from '../list/tenant-list.state';
import { BaseComponent } from '../../core/base.component';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Tenant, DomainError } from '../../core/models';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-tenant-create-form',
  templateUrl: './tenant-create-form.component.html',
  styleUrls: ['./tenant-create-form.component.scss']
})
export class TenantCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;

  constructor(
    private referenceService: ReferenceService,
    private utils: UtilsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<TenantCreateFormComponent>) {
      super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      id: [null],
      name: ['', Validators.required]
    });
  }

  mapForm(formGroup): Tenant {
    const tenant = new Tenant();
    Object.assign(tenant, formGroup.value);

    return tenant;
  }

  setType(value) {
    this.formGroup.patchValue({tenantTypeId: value})
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const tenant = this.mapForm(this.formGroup);
      this.store.dispatch(new tenantActions.AddTenant(tenant))
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
        this.dialogRef.close(tenant);
      });
    }
  }
}

