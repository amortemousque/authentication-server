import { Component, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';

import { ApiCreateFormComponent } from '../create-form/api-create-form.component';
import { BaseComponent } from '../../core/base.component';
import { MatDialog } from '@angular/material/dialog';
import { environment } from '../../../environments/environment';
import { SearchApis } from './api-list.state';


@Component({
  selector: 'app-api-list',
  templateUrl: './api-list.component.html',
  styleUrls: ['./api-list.component.scss']
})
export class ApiListComponent extends BaseComponent implements OnInit {

  @Select(state => state.apis.apis) apis$;
  @Select(state => state.apis.loading) loading$;

  readonly pageLimit = 200;
  readonly headerHeight = 50;
  readonly rowHeight = 55;
  loading = true;
  totalCount: number;
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

    this.store.dispatch(new SearchApis(''));
    this.loading$.subscribe(loading => this.loading = loading);
  }


  onActivate(selected) {
    this.rowHovered = selected.row;
  }

  onSelect(selected) {
    const id = selected.selected[0].id;
    this.router.navigate(['/api/details/' + id]);
  }


  openCreate() {
    const dialogRef = this.dialog.open(ApiCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
