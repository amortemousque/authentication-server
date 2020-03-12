import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from "@angular/forms";
import { Observable, pipe } from "rxjs";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { startWith, switchMap, map, catchError } from "rxjs/operators";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { TenantService, RoleService } from "../../core/services";
import { Store } from "@ngxs/store";
import { User, DomainError } from "../../core/models";
import { AddUser } from "../list/user-list.state";
import { BaseComponent } from "../../core/base.component";
import { MessageService } from "../../shared/message/message.service";

@Component({
  selector: 'app-user-create-form',
  templateUrl: './user-create-form.component.html',
  styleUrls: ['./user-create-form.component.scss']
})
export class UserCreateFormComponent extends BaseComponent implements OnInit {

  formGroup: FormGroup;

  loading = false;
  roles$: Observable<any>;
  tenants$: Observable<any>;
  roleCtrl = new FormControl();
  @ViewChild('roleInput', {static: true}) roleInput: ElementRef;

  constructor(
    private messageService: MessageService,
    private tenantService: TenantService,
    private formBuilder: FormBuilder,
    private roleService: RoleService,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<UserCreateFormComponent>) {
    super();
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      tenantId: [null, Validators.required],
      givenName: ['', Validators.required],
      familyName: ['', Validators.required],
      email: ['', Validators.required],
      roles: [[]]
    });
    this.tenants$ = this.tenantService.searchTenants('');

    this.roles$ = this.formGroup.get('roles').valueChanges
    pipe(startWith([])
      , switchMap(values =>
        this.roleService.searchRoles('').pipe(map(roles => {
          return roles.filter(r => !this.formGroup.controls.roles.value.includes(r.normalizedName));
        })
        )));
  }

  mapForm(formGroup): User {
    const user = new User();
    Object.assign(user, formGroup.value);

    return user;
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    const roles = [...this.formGroup.value.roles];
    roles.push(event.option.viewValue);
    this.formGroup.patchValue({ roles: roles });
    this.roleInput.nativeElement.value = '';
    this.roleCtrl.setValue(null);
  }

  setType(value) {
    this.formGroup.patchValue({ userTypeId: value })
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const user = this.mapForm(this.formGroup);
      this.store.dispatch(new AddUser(user))
        .pipe(catchError(err => {
          if (err instanceof DomainError) {
            this.formGroup.get(err.field).setErrors({
              notUnique: true
            });
            this.loading = false;
          }
          return err;
        }))
        .subscribe(r => {
          this.loading = false;
          this.messageService.openSuccessMessage('saved');
          this.dialogRef.close(user);
        });
    }
  }
}

