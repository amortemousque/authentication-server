import { Component, OnInit } from "@angular/core";
import { FormGroup, FormControl, FormBuilder } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { takeUntil } from "rxjs/operators";
import { BaseComponent } from "../../core/base.component";
import { Select, Store } from "@ngxs/store";
import { User } from "../../core/models";
import { LoadUser, UpdateUserPassword } from "./user-details.state";
import { MessageService } from "../../shared/message/message.service";
import { DeleteUser } from "../list/user-list.state";

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent extends BaseComponent implements OnInit {


  @Select(state => state.user.user) user$;
  user: User;

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
      this.store.dispatch(new LoadUser(this.id));
    });

    this.user$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(user2 => {
      if (user2) {
        this.user = user2;
      }
    });
  
  }

  delete() {
    this.messageService.openConfirmationDialog('Etes vous sur de supprimer le user ? Cette suppression sera définive.')
    .afterClosed()
    .subscribe((result) => {
        if (result) {
          this.store.dispatch(new DeleteUser(this.user)).subscribe(() => {
            this.messageService.openSuccessMessage('deleted');
            this.router.navigate(['/user']);
          });
        }
      });
  }


  generateNewPassword() {
    if (!this.user.emailConfirmed) {
      this.messageService.openErrorMessage('Impossible de réinitialiser le mot de passe si l\email n\'est pas vérifié')
    } else {
      this.messageService.openConfirmationDialog('Etes vous de réinitialiser la mot de passe ? Un email sera envoyé à l\'utilisateur')
      .afterClosed().subscribe(result => {
        if (result) {
          this.store.dispatch(new UpdateUserPassword(this.user.id))
          .subscribe(r => {
            this.loading = false;
            this.messageService.openSuccessMessage('saved');

          });
        }
      })
    }
  }

  setType(value) {
    this.formGroup.patchValue({userTypeId: value})
  }
}

