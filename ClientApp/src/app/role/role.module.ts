import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { LayoutModule } from '../layout/layout.module'

import { RoleListComponent } from './list/role-list.component'
import { RoleListState } from './list/role-list.state';
import { RoleDetailsState } from './details/role-details.state';
import { PermissionsState } from './permissions/role-permissions.state';

import { RoleRoutes } from './role.routing';

import { RoleCreateFormComponent } from './create-form/role-create-form.component'
import { RoleDetailsComponent } from './details/role-details.component';
import { RoleSettingsFormComponent } from './settings-form/role-settings-form.component';
import { RolePermissionsComponent } from './permissions/role-permissions.component';
import { AuthModule } from '../auth/auth.module';
import { CoreModule } from '../core/core.module';
import { NgxsModule } from '@ngxs/store';
import { AuthGuard } from '../auth/guards/auth.guard';
import { RoleGuard } from '../auth/guards/role.guard';

@NgModule({
  imports: [
    CommonModule,
    // InterviewModule,
    LayoutModule,
    RouterModule.forChild(RoleRoutes),
    AuthModule.forRoot(),
    CoreModule.forRoot(),
    NgxsModule.forFeature([
        RoleListState,
        RoleDetailsState,
        PermissionsState
    ])
  ],
  declarations: [
    RoleListComponent,
    RoleCreateFormComponent,
    RoleDetailsComponent,
    RoleSettingsFormComponent,
    RolePermissionsComponent
  ],
  exports: [
    RoleListComponent
  ],
  entryComponents: [
    RoleCreateFormComponent
  ]
})

export class RoleModule {}
