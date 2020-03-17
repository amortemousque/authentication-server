import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { BaseComponent } from '../../core/base.component';
import { Tenant } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';
import { DeleteTenant } from '../list/tenant-list.state';
import { LoadTenant } from './tenant-details.state';

@Component({
  selector: 'app-tenant-details',
  templateUrl: './tenant-details.component.html',
  styleUrls: ['./tenant-details.component.scss']
})
export class TenantDetailsComponent extends BaseComponent implements OnInit {


  @Select(state => state.tenant.tenant) tenant$;
  tenant: Tenant;

  id: string;
  formGroup: FormGroup;
  schoolCtrl: FormControl;
  loading = false;

  constructor(
    private messageService: MessageService,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar,
    public route: ActivatedRoute,
    public router: Router) {
      super();
  }

  ngOnInit() {

    this.route.params
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(params => {
      this.id = params['id'];
      this.store.dispatch(new LoadTenant(this.id));
    });

    this.tenant$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(tenant2 => {
      if (tenant2) {
        this.tenant = tenant2;
      }
    });

  }

  delete() {
    this.messageService.openConfirmationDialog('Etes vous sur de supprimer le tenant ? Cette suppression sera définive.')
    .afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new DeleteTenant(this.tenant)).subscribe(() => {
          this.messageService.openSuccessMessage('deleted');
          this.router.navigate(['/tenant']);
        });
      }
    })
  }

  setType(value) {
    this.formGroup.patchValue({tenantTypeId: value})
  }
}

