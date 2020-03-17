import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { BaseComponent } from '../../core/base.component';
import { Api } from '../../core/models';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { MatChipInputEvent } from '@angular/material/chips';
import { MessageService } from '../../shared/message/message.service';
import { deleteEmptyKeys } from '../../core/utils/global.utils';
import { UpdateApi } from '../list/api-list.state';


@Component({
  selector: 'app-api-settings-form',
  templateUrl: './api-settings-form.component.html',
  styleUrls: ['./api-settings-form.component.scss']
})
export class ApiSettingsFormComponent extends BaseComponent implements OnInit {

  @Select(state => state.api.api) api$;
  api: Api;
  formGroup: FormGroup;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  allowedScopes = [];
  // studyLevels = new Observable<StudyLevel[]>();
  loading = false;

  constructor(
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private store: Store,
    public snackBar: MatSnackBar) {
    super();
  }

  ngOnInit() {

    this.formGroup = this.formBuilder.group({
      id: [null],
      enabled: [true],
      name: ['', Validators.required],
      displayName: ['', Validators.required],
      description: ['', Validators.required],
      userClaims: new FormArray([])
    });


    this.api$
      .pipe(takeUntil(this.componentDestroyed$))
      .subscribe(api2 => {
        if (api2) {
          this.api = api2;

          // FIll the form
          const api = new Api();
          Object.assign(api, this.api);
          deleteEmptyKeys(api);
          this.formGroup.reset();
          this.formGroup.patchValue(api);
        }
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
        this.formGroup.patchValue({ [controlName]: uris });
      })
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  remove(uri: string, controlName: string): void {
    const uris = this.formGroup.value[controlName].filter(s => s !== uri)
    this.formGroup.patchValue({ [controlName]: uris });
  }

  mapForm(formGroup): Api {
    const api = new Api();
    Object.assign(api, formGroup.value);

    return api;
  }

  onSubmit() {
    if (this.formGroup.valid) {
      this.loading = true;
      const api = this.mapForm(this.formGroup);
      this.store.dispatch(new UpdateApi(api))
        .subscribe(r => {
          this.loading = false;
          this.messageService.openSuccessMessage('saved')
        });
    }
  }
}

