import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UserListComponent } from './list/user-list.component'
import { UserListState } from './list/user-list.state';
import { UserDetailsState } from './details/user-details.state';

import { UserRoutes } from './user.routing';

import { UserCreateFormComponent } from './create-form/user-create-form.component'
import { AuthModule } from '../core/auth/auth.module';
import { CoreModule } from '@angular/flex-layout';
import { NgxsModule } from '@ngxs/store';
import { UserDetailsComponent } from './details/user-details.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserSettingsFormComponent } from './settings-form/user-settings-form.component';


@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(UserRoutes),
    AuthModule.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    CoreModule,
    SharedModule,
    NgxsModule.forFeature([
        UserListState,
        UserDetailsState
    ])
  ],
  declarations: [
    UserListComponent,
    UserCreateFormComponent,
    UserDetailsComponent,
    UserSettingsFormComponent
  ],
  exports: [
    UserListComponent
  ],
  entryComponents: [
    UserCreateFormComponent
  ]
})

export class UserModule {}
