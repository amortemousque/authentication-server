import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TenantListComponent } from './list/tenant-list.component'
import { TenantListState } from './list/tenant-list.state';
import { TenantDetailsState } from './details/tenant-details.state';

import { TenantRoutes } from './tenant.routing';

import { TenantCreateFormComponent } from './create-form/tenant-create-form.component'
import { TenantDetailsComponent } from './details/tenant-details.component';
import { TenantSettingsFormComponent } from './settings-form/tenant-settings-form.component';
import { CoreModule } from '../core/core.module';
import { NgxsModule } from '@ngxs/store';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(TenantRoutes),
    SharedModule,
    CoreModule,
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

export class TenantModule { }
