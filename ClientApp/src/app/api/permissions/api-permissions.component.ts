import { Component, OnInit} from '@angular/core';
import { Select, Store } from '@ngxs/store';

import { BaseComponent } from '../../core/base.component';
import { Api, ApiScope } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';
import { GetApiScopes, DeleteApiScope } from './api-permissions.state';


@Component({
  selector: 'app-api-permissions',
  templateUrl: './api-permissions.component.html',
  styleUrls: ['./api-permissions.component.scss']
})
export class ApiPermissionsComponent extends BaseComponent implements OnInit {

  @Select(state => state.apiPermissions.apiScopes) apiScopes$;
  @Select(state => state.api.api) api$;

  api: Api;
  scopes: any[];


  constructor(
    private messageService: MessageService,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
      super();
  }

  ngOnInit() {
    this.api$.subscribe(api => {
        this.api = api;
        this.store.dispatch(new GetApiScopes(api.id));
    })

    this.apiScopes$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(scopes => {
        this.scopes = [...scopes.map(s => Object.assign(new ApiScope(), s))];
    });
  }
  cancelledEdition(scope) {
    if (!scope.id) {
      this.scopes.pop();
    }
  }
  openPermissionForm(scope = null) {
    this.scopes = this.scopes.filter(s => s.id).map(s => {
      (s as any).isEditing = false;
      return s;
    });

    if (scope) {
      (scope as any).isEditing = true;
    } else {
      this.scopes.push({
        apiResourceId: this.api.id,
        name: '',
        displayName: '',
        description: '',
        isEditing: true
      })
    }
  }
  deletePermission(scope) {
    this.store.dispatch(new DeleteApiScope(scope)).subscribe(() => {
      this.messageService.openSuccessMessage('deleted');
    });
  }
}

