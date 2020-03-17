import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';

import * as apiListActions from '../list/api-list.state';
import * as apiActions from './api-details.state';
import { BaseComponent } from '../../core/base.component';
import { Api } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { MessageService } from '../../shared/message/message.service';

@Component({
  selector: 'app-api-details',
  templateUrl: './api-details.component.html',
  styleUrls: ['./api-details.component.scss']
})
export class ApiDetailsComponent extends BaseComponent implements OnInit {


  @Select(state => state.api.api) api$;
  api: Api;

  id: string;
  formGroup: FormGroup;
  schoolCtrl: FormControl;

  loading = false;

  constructor(
    private messageService: MessageService,
    private store: Store,
    public route: ActivatedRoute,
    public router: Router) {
      super();
  }

  ngOnInit() {

    this.route.params.pipe(
    takeUntil(this.componentDestroyed$))
    .subscribe(params => {
      this.id = params['id'];
      this.store.dispatch(new apiActions.LoadApi(this.id));
    });

    this.api$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(api2 => {
      if (api2) {
        this.api = api2;
      }
    });
  }

  delete() {

    this.messageService.openConfirmationDialog('Etes vous sur de supprimer le api ? Cette suppression sera définive.')
    .afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new apiListActions.DeleteApi(this.api)).subscribe(() => {
          this.messageService.openSuccessMessage('deleted');
          this.router.navigate(['/api']);
        });
      }
    })
  }



  setType(value) {
    this.formGroup.patchValue({apiTypeId: value})
  }


}

