import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { BaseComponent } from '../../core/base.component';
import { Role } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil, filter } from 'rxjs/operators';
import { LoadRole } from './role-details.state';
import { DeleteRole } from '../list/role-list.state';
import { MessageService } from '../../shared/message/message.service';

@Component({
  selector: 'app-role-details',
  templateUrl: './role-details.component.html',
  styleUrls: ['./role-details.component.scss']
})
export class RoleDetailsComponent extends BaseComponent implements OnInit {

  @Select(state => state.role.role) role$;
  role: Role;
  id: string;
  formGroup: FormGroup;
  schoolCtrl: FormControl;
  loading = false;

  constructor(
    private messageService: MessageService,
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
      this.store.dispatch(new LoadRole(this.id));
    });

    this.role$
    .pipe(takeUntil(this.componentDestroyed$), filter(r => !!r))
    .subscribe(role =>  this.role = role );
  }

  delete() {
    this.messageService.openConfirmationDialog('Etes vous sur de supprimer le role ? Cette suppression sera définive.')
    .afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new DeleteRole(this.role.id)).subscribe(() => {
          this.messageService.openSuccessMessage('deleted');
          this.router.navigate(['/role']);
        });
      }
    })
  }

  setType(value) {
    this.formGroup.patchValue({roleTypeId: value})
  }
}

