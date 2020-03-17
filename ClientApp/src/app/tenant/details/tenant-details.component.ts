import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

import { Select, Store } from '@ngxs/store';

import * as tenantListActions from '../list/tenant-list.state';
import * as tenantActions from './tenant-details.state';
import { BaseComponent } from '../../core/base.component';
import { Tenant } from '../../core/models';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmModalComponent } from '../../core/components/modal/confirm-modal.component';
import { takeUntil } from 'rxjs/operators';

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

  // filteredSchools = new Observable<ReferenceLabel[]>();
  // studyLevels = new Observable<StudyLevel[]>();
  loading = false;

  constructor(
    private referenceService: ReferenceService,
    private utils: UtilsService,
    private formBuilder: FormBuilder,
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
      this.store.dispatch(new tenantActions.LoadTenant(this.id));
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
    const dialogRef = this.dialog.open(ConfirmModalComponent, {
      width: '700px',
      panelClass: 'app-dialog',
      data: {
        message: 'Suppression',
        details: 'Etes vous sur de supprimer le tenant ? Cette suppression sera définive.'
     }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new tenantListActions.DeleteTenant(this.tenant)).subscribe(() => {
          this.utils.displaySnackMessage('deleted');
          this.router.navigate(['/tenant']);
        });
      }
    })
  }



  setType(value) {
    this.formGroup.patchValue({tenantTypeId: value})
  }


}

