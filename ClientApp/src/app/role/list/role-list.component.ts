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
    private el: ElementRef,
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

  // onScroll(e: any, offsetY: number) {
  //   this.store.dispatch(new stateActions.UpdatePointerPositionY(offsetY));
  //   if (this.talentNumer < this.totalCount) {
  //     // total height of all rows in the viewport
  //     const viewHeight = this.el.nativeElement.getBoundingRoleRect().height - this.headerHeight;
  //     const length = this.talentNumer;
  //     // check if we scrolled to the end of the viewport
  //     const left = offsetY + viewHeight;
  //     const right =  (length * this.rowHeight - 12000);
  //     if (!this.loading && left >= right) {

  //       // total number of results to load
  //       let limit = this.pageLimit;

  //       // check if we haven't fetched any results yet
  //       if (length === 0) {

  //         // calculate the number of rows that fit within viewport
  //         const pageSize = Math.ceil(viewHeight / this.rowHeight);

  //         // change the limit to pageSize such that we fill the first page entirely
  //         // (otherwise, we won't be able to scroll past it)
  //         limit = Math.max(pageSize, this.pageLimit);
  //       }
  //       this.loadPage(limit);
  //     }
  //   }
  // }

  // private loadPage(limit: number) {
  //   this.store.dispatch(new stateActions.NextPage(limit))
  // }

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
