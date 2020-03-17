import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';

import { BaseComponent } from '../../core/base.component';
import { Client } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';
import { LoadClient } from './client-details.state';
import { DeleteClient } from '../list/client-list.state';

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
    private messageService: MessageService,
    public dialog: MatDialog,
    private store: Store,
    public route: ActivatedRoute,
    public router: Router) {
      super();
  }

  ngOnInit() {

    this.route.params
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(params => {
      this.id = params['id'];
      this.store.dispatch(new LoadClient(this.id));
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
    this.messageService.openConfirmationDialog('Etes vous sur de supprimer le client ? Cette suppression sera définive.')
    .afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new DeleteClient(this.client)).subscribe(() => {
          this.messageService.openSuccessMessage('deleted');
          this.router.navigate(['/client']);
        });
      }
    })
  }


  setType(value) {
    this.formGroup.patchValue({clientTypeId: value})
  }
}

