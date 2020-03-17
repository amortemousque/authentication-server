import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

import { TenantCreateFormComponent } from '../create-form/tenant-create-form.component';
import * as stateActions from './tenant-list.state';
import { BaseComponent } from '../../core/base.component';
import { Select, Store } from '@ngxs/store';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';


@Component({
  selector: 'app-tenant-list',
  templateUrl: './tenant-list.component.html',
  styleUrls: ['./tenant-list.component.scss']
})
export class TenantListComponent extends BaseComponent implements OnInit {

  @Select(state => state.tenants.tenants) tenants$;
  @Select(state => state.tenants.loading) loading$;

  readonly pageLimit = 200;
  readonly headerHeight = 50;
  readonly rowHeight = 55;
  loading = true;
  totalCount: number;
  talentNumer: number;
  blobApiUrl: string;
  throttle = 300;
  open = true;
  selected = [];
  rowHovered: any;

  constructor(
    public dialog: MatDialog,
    public router: Router,
    private el: ElementRef,
    private store: Store
  ) {
      super();
      this.blobApiUrl = environment.blobApiUrl;
  }

  ngOnInit() {

    this.store.dispatch(new stateActions.SearchTenants(''));
    this.tenants$.subscribe(tenants => {
      if (tenants != null) {
        this.talentNumer = tenants.length;
      }
    });

    this.loading$.subscribe(loading => this.loading = loading);
  }

  onActivate(selected) {
    this.rowHovered = selected.row;
  }

  onSelect(selected) {
    const id = selected.selected[0].id;
    this.router.navigate(['/tenant/details/' + id]);
  }


  openCreate() {
    const dialogRef = this.dialog.open(TenantCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
