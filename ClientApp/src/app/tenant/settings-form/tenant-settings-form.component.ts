import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { BaseComponent } from '../../core/base.component';
import { Tenant } from '../../core/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';
import { deleteEmptyKeys } from '../../core/utils/global.utils';
import { UpdateTenant } from '../list/tenant-list.state';


@Component({
  selector: 'app-tenant-settings-form',
  templateUrl: './tenant-settings-form.component.html',
  styleUrls: ['./tenant-settings-form.component.scss']
})
export class TenantSettingsFormComponent extends BaseComponent implements OnInit {

  @Select(state => state.tenant.tenant) tenant$;
  tenant: Tenant;
  formGroup: FormGroup;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  loading = false;

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
      id: [null],
      name: ['', Validators.required],
      description: [''],
      logoUri: [''],
      enabled: [true, Validators.required],
      regionId: [1, Validators.required]
    });

    this.tenant$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(tenant2 => {
      if (tenant2) {
        this.tenant = tenant2;

      // FIll the form
        const tenant = new Tenant();
        Object.assign(tenant, this.tenant);
        deleteEmptyKeys(tenant);
        this.formGroup.reset();
        this.formGroup.patchValue(tenant);
      }
    });

  }


  mapForm(formGroup): Tenant {
    const tenant = new Tenant();
    Object.assign(tenant, formGroup.value);

    return tenant;
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const tenant = this.mapForm(this.formGroup);
      this.store.dispatch(new UpdateTenant(tenant))
      .subscribe(r => {
        this.loading = false;
        this.messageService.openSuccessMessage('saved')
      });
    }
  }
}

