import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngxs/store';

import { BaseComponent } from '../../core/base.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Role, DomainError } from '../../core/models';
import { catchError } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';
import { AddRole } from '../list/role-list.state';

@Component({
  selector: 'app-role-create-form',
  templateUrl: './role-create-form.component.html',
  styleUrls: ['./role-create-form.component.scss']
})
export class RoleCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;
  roleCtrl = new FormControl();
  @ViewChild('roleInput', {static: false}) roleInput: ElementRef;

  constructor(
    private messageService: MessageService,
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
      this.store.dispatch(new AddRole(role))
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
        this.dialogRef.close(role);
      });
    }
  }
}

