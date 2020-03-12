import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { BaseComponent } from "../../core/base.component";
import { Select, Store } from "@ngxs/store";
import { User } from "../../core/models";
import { FormGroup, FormControl, FormBuilder, Validators } from "@angular/forms";
import { Observable } from "rxjs";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { startWith, switchMap, map, takeUntil } from "rxjs/operators";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { UpdateUser } from "../list/user-list.state";
import { MessageService } from "../../shared/message/message.service";
import { RoleService } from "../../core/services";


@Component({
  selector: 'app-user-settings-form',
  templateUrl: './user-settings-form.component.html',
  styleUrls: ['./user-settings-form.component.scss']
})
export class UserSettingsFormComponent extends BaseComponent implements OnInit {

  @Select(state => state.user.user) user$;
  user: User;
  formGroup: FormGroup;
  selectable = true;
  removable = true;
  addOnBlur = true;
  loading = false;

  roles$: Observable<any>;

  roleCtrl = new FormControl();
  @ViewChild('roleInput', {static: false}) roleInput: ElementRef;

  constructor(
    private messageService: MessageService,
    private roleService: RoleService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
      super();

  }


  ngOnInit() {

    this.formGroup = this.formBuilder.group({
      id: [null],
      tenantId: [null],
      givenName: ['', Validators.required],
      familyName: ['', Validators.required],
      email: ['', Validators.required],
      roles:  [[]],
      emailConfirmed: [false]
    });

    this.roles$ = this.formGroup.get('roles').valueChanges
    .pipe(startWith([]),
    switchMap(values =>
      this.roleService.searchRoles('').pipe(map(roles => {
        return roles.filter(r => !this.formGroup.controls.roles.value.includes(r.normalizedName));
      })
    )));

    this.user$
    .pipe(takeUntil(this.componentDestroyed$))
    .subscribe(user2 => {
      if (user2) {
        this.user = user2;

        const user = new User();
        Object.assign(user, this.user);
        this.formGroup.patchValue(user);
      }
    });
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    const val = event.option.viewValue;
    const roles = [...this.formGroup.get('roles').value, val];
    this.formGroup.patchValue({roles: roles})
    this.roleInput.nativeElement.value = '';
    this.roleCtrl.setValue(null);
  }

  remove(role) {
    const roles = this.formGroup.get('roles').value.filter(r => r !== role);
    this.formGroup.patchValue({roles: roles})
  }


  mapForm(formGroup): User {
    const user = new User();
    Object.assign(user, formGroup.value);

    return user;
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const user = this.mapForm(this.formGroup);
      this.store.dispatch(new UpdateUser(user))
      .subscribe(r => {
        this.loading = false;
        this.messageService.openSuccessMessage('saved')
      });
    }
  }
}

