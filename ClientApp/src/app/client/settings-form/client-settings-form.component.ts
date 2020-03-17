import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, OnInit } from '@angular/core';

import * as clientActions from '../list/client-list.state';
import * as detailsActions from '../details/client-details.state';
import { BaseComponent } from '../../core/base.component';
import { Select, Store } from '@ngxs/store';
import { Client } from '../../core/models';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { ReferenceService } from '../../core/services';
import { UtilsService } from '../../core/utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmModalComponent } from '../../core/components/modal/confirm-modal.component';
import { takeUntil } from 'rxjs/operators';
import { MatChipInputEvent } from '@angular/material/chips';


@Component({
  selector: 'app-client-settings-form',
  templateUrl: './client-settings-form.component.html',
  styleUrls: ['./client-settings-form.component.scss']
})
export class ClientSettingsFormComponent extends BaseComponent implements OnInit {

  @Select(state => state.client.client) client$;
  client: Client;
  formGroup: FormGroup;
  selectable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  allowedScopes = [];
  // studyLevels = new Observable<StudyLevel[]>();
  loading = false;
  secretIsDisplayed = false;

  constructor(
    private referenceService: ReferenceService,
    private utils: UtilsService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
      super();
  }

  ngOnInit() {

    this.formGroup = this.formBuilder.group({
      id: [null],
      clientName: ['', Validators.required],
      clientTypeId: ['', Validators.required],
      enabled: [true, Validators.required],
      clientId: [''],
      description: [''],
      clientUri: [''],
      logoUri: [''],
      requireConsent: [false],
      alwaysIncludeUserClaimsInIdToken: [false],
      allowAccessTokensViaBrowser: [false],
      identityTokenLifetime: [0],
      requireClientSecret: [false],
      redirectUris: [[]],
      allowedCorsOrigins: [[]],
      allowedScopes: new FormArray([]),
      resourceOwnerEnabled: [false],
    });

    this.referenceService.getReference('allowedScopes').subscribe(scopes => {
      scopes.forEach(scope => {
        this.allowedScopes.push(scope);
        (this.formGroup.get('allowedScopes') as FormArray).push(new FormControl(scope.name));
      });


      this.client$
      .pipe(takeUntil(this.componentDestroyed$))
      .subscribe(client2 => {
        if (client2) {
          this.client = client2;

        // FIll the form
          const client = new Client();
          Object.assign(client, this.client);
          this.utils.deleteEmptyKeys(client);
          this.formGroup.reset();
          this.formGroup.patchValue(client);
          this.formGroup.patchValue({
            allowedScopes: this.allowedScopes.map(scope => (client.allowedScopes || []).find(a => a === scope.name))
          });
          this.formGroup.patchValue({
            resourceOwnerEnabled: client.allowedGrantTypes.includes("password")
          });
        }
      });
    });
  }


  add(event: MatChipInputEvent, controlName: string): void {
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || '').trim()) {
      const valueArray = value.split(',');
      valueArray.forEach(uri => {
        if (this.formGroup.value[controlName] == null) {
          this.formGroup.value[controlName] = [];
        }
        const uris = [...this.formGroup.value[controlName], uri.trim()];
        this.formGroup.patchValue({[controlName]: uris});
      })
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  remove(uri: string, controlName: string): void {
    const uris = this.formGroup.value[controlName].filter(s => s !== uri)
    this.formGroup.patchValue({[controlName]: uris});
  }


  generateNewClientSecret() {
    const dialogRef = this.dialog.open(ConfirmModalComponent, {
      width: '700px',
      panelClass: 'app-dialog',
      data: {
        message: 'Suppression',
        details: 'Etes vous sur de régérer le secret ?'
     }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(new detailsActions.UpdateClientSecret(this.client.id));
      }
    })
  }


  mapForm(formGroup): Client {
    const client = new Client();
    Object.assign(client, formGroup.value);

    const scopes = this.formGroup.value.allowedScopes
    .map((v, i) => v ? this.allowedScopes[i].name : null)
    .filter(v => v !== null);

    client.allowedScopes = scopes;
    return client;
  }

  get allowedScopesControls(): FormArray {
    return this.formGroup.get('allowedScopes') as FormArray;
  }
  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const client = this.mapForm(this.formGroup);
      this.store.dispatch(new clientActions.UpdateClient(client))
      .subscribe(r => {
        this.loading = false;
        this.utils.displaySnackMessage('saved')
      });
    }
  }
}

