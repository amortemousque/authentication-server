import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';


import { LayoutModule } from '../layout/layout.module'

import { TenantListComponent } from './list/tenant-list.component'
import { TenantListState } from './list/tenant-list.state';
import { TenantDetailsState } from './details/tenant-details.state';

import { TenantRoutes } from './tenant.routing';

import { TenantCreateFormComponent } from './create-form/tenant-create-form.component'
import { TenantDetailsComponent } from './details/tenant-details.component';
import { TenantSettingsFormComponent } from './settings-form/tenant-settings-form.component';
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
    RouterModule.forChild(TenantRoutes),

    AuthModule.forRoot(),
    CoreModule.forRoot(),
    NgxsModule.forFeature([
        TenantListState,
        TenantDetailsState
    ])
  ],
  declarations: [
    TenantListComponent,
    TenantCreateFormComponent,
    TenantDetailsComponent,
    TenantSettingsFormComponent
  ],
  exports: [
    TenantListComponent
  ],
  entryComponents: [
    TenantCreateFormComponent
  ]
})

export class TenantModule {}
