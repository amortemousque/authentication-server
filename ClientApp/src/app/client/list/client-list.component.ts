import { Component, ElementRef, OnInit } from '@angular/core';

import { ClientCreateFormComponent } from '../create-form/client-create-form.component';
import { BaseComponent } from '../../core/base.component';
import { Select, Store } from '@ngxs/store';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { SearchClients } from './client-list.state';


@Component({
  selector: 'app-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.scss']
})
export class ClientListComponent extends BaseComponent implements OnInit {

  @Select(state => state.clients.clients) clients$;
  @Select(state => state.clients.loading) loading$;

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

    this.store.dispatch(new SearchClients(''));
    this.clients$.subscribe(clients => {
      if (clients != null) {
        this.talentNumer = clients.length;
      }
    });

    this.loading$.subscribe(loading => this.loading = loading);
  }

  onActivate(selected) {
    this.rowHovered = selected.row;
  }

  onSelect(selected) {
    const id = selected.selected[0].id;
    this.router.navigate(['/client/details/' + id]);
  }


  openCreate() {
    const dialogRef = this.dialog.open(ClientCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
