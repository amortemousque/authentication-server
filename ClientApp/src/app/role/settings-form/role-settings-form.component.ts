import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';

import * as roleActions from '../list/role-list.state';
import { Observable } from '../../../../node_modules/rxjs';
import { BaseComponent } from '../../core/base.component';
import { Role } from '../../core/models';
import { ReferenceService, TenantService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';


@Component({
  selector: 'app-role-settings-form',
  templateUrl: './role-settings-form.component.html',
  styleUrls: ['./role-settings-form.component.scss']
})
export class RoleSettingsFormComponent extends BaseComponent implements OnInit {

  @Select(state => state.role.role) role$;
  role: Role;
  formGroup: FormGroup;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  loading = false;

  roles$: Observable<any>;
  tenants$: Observable<any>;

  roleCtrl = new FormControl();
  @ViewChild('roleInput') roleInput: ElementRef;

  constructor(
    private referenceService: ReferenceService,
    private tenantService: TenantService,

    private utils: UtilsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
      super();
  }


  ngOnInit() {

    this.formGroup = this.formBuilder.group({
      id: [null],
      description: ['', Validators.required],
      permissions:  [[]]
    });
    this.tenants$ = this.tenantService.searchTenants('');

    this.role$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(role2 => {
      if (role2) {
        this.role = role2;

      // FIll the form
        const role = new Role();
        Object.assign(role, this.role);
        this.utils.deleteEmptyKeys(role);
        this.formGroup.reset();
        this.formGroup.patchValue(role);
      }
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
      this.store.dispatch(new roleActions.UpdateRole(role))
      .subscribe(r => {
        this.loading = false;
        this.utils.displaySnackMessage('saved')
      });
    }
  }
}

