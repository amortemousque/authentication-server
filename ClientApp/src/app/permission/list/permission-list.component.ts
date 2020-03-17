import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../../core/base.component';
import { Select, Store } from '@ngxs/store';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { PermissionCreateFormComponent } from '../create-form/permission-create-form.component';
import { DeletePermissions, SearchPermissions } from './permission-list.state';
import { MessageService } from '../../shared/message/message.service';

@Component({
  selector: 'app-permission-list',
  templateUrl: './permission-list.component.html',
  styleUrls: ['./permission-list.component.scss']
})
export class PermissionListComponent extends BaseComponent implements OnInit {

  @Select(state => state.permissions.permissions) permissions$;
  @Select(state => state.permissions.loading) loading$;

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
    private store: Store,
    private messageService: MessageService,

  ) {
      super();
      this.blobApiUrl = environment.blobApiUrl;
  }

  ngOnInit() {
    this.store.dispatch(new SearchPermissions(''));
    this.loading$.subscribe(loading => this.loading = loading);
  }


  deletePermissions() {
    this.store.dispatch(new DeletePermissions(this.selected))
    .subscribe(r => {
      this.messageService.openSuccessMessage('deleted')
    });
    this.selected.splice(0, this.selected.length);
  }

  onSelect({ selected }) {
    console.log('Select Event', selected, this.selected);

    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
  }


  openCreate() {
    this.dialog.open(PermissionCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
