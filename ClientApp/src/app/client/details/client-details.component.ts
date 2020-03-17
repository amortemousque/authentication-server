import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';

import * as clientListActions from '../list/client-list.state';
import * as clientActions from './client-details.state';
import { BaseComponent } from '../../core/base.component';
import { Client } from '../../core/models';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmModalComponent } from '../../core/components/modal/confirm-modal.component';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-client-details',
  templateUrl: './client-details.component.html',
  styleUrls: ['./client-details.component.scss']
})
export class ClientDetailsComponent extends BaseComponent implements OnInit {


  @Select(state => state.client.client) client$;
  client: Client;

  id: string;
  formGroup: FormGroup;
  schoolCtrl: FormControl;

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
      this.store.dispatch(new clientActions.LoadClient(this.id));
    });

    this.client$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(client2 => {
      if (client2) {
        this.client = client2;
      }
    });
  
  }




  delete() {
    const dialogRef = this.dialog.open(ConfirmModalComponent, {
      width: '700px',
      panelClass: 'app-dialog',
      data: {
        message: 'Suppression',
        details: 'Etes vous sur de supprimer le client ? Cette suppression sera définive.'
     }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new clientListActions.DeleteClient(this.client)).subscribe(() => {
          this.utils.displaySnackMessage('deleted');
          this.router.navigate(['/client']);
        });
      }
    })
  }



  setType(value) {
    this.formGroup.patchValue({clientTypeId: value})
  }


}

