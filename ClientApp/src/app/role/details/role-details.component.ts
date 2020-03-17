import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { BaseComponent } from '../../core/base.component';
import { Role } from '../../core/models';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmModalComponent } from '../../core/components/modal/confirm-modal.component';
import { takeUntil, filter } from 'rxjs/operators';
import { LoadRole } from './role-details.state';
import { DeleteRole } from '../list/role-list.state';

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
    private utils: UtilsService,
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
    const dialogRef = this.dialog.open(ConfirmModalComponent, {
      width: '700px',
      panelClass: 'app-dialog',
      data: {
        message: 'Suppression',
        details: 'Etes vous sur de supprimer le role ? Cette suppression sera définive.'
     }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new DeleteRole(this.role.id)).subscribe(() => {
          this.utils.displaySnackMessage('deleted');
          this.router.navigate(['/role']);
        });
      }
    })
  }



  setType(value) {
    this.formGroup.patchValue({roleTypeId: value})
  }


}

