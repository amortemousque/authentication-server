import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

import { RoleCreateFormComponent } from '../create-form/role-create-form.component';
import { BaseComponent } from '../../core/base.component';
import { Select, Store } from '@ngxs/store';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { SearchRoles } from './role-list.state';


@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.scss']
})
export class RoleListComponent extends BaseComponent implements OnInit {

  @Select(state => state.roles.roles) roles$;
  @Select(state => state.roles.loading) loading$;

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
    private store: Store
  ) {
      super();
      this.blobApiUrl = environment.blobApiUrl;
  }

  ngOnInit() {

    this.store.dispatch(new SearchRoles(''));
    this.roles$.subscribe(roles => {
      if (roles != null) {
        this.talentNumer = roles.length;
      }
    });

    this.loading$.subscribe(loading => this.loading = loading);
  }

  onActivate(selected) {
    this.rowHovered = selected.row;
  }

  onSelect(selected) {
    const id = selected.selected[0].id;
    this.router.navigate(['/role/details/' + id]);
  }


  openCreate() {
    const dialogRef = this.dialog.open(RoleCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
