import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgxsModule } from '@ngxs/store';
import { ApiListComponent } from './list/api-list.component'
import { ApiListState } from './list/api-list.state';
import { ApiDetailsState } from './details/api-details.state';
import { ApiPermissionsState } from './permissions/api-permissions.state';
import { ApiRoutes } from './api.routing';
import { ApiCreateFormComponent } from './create-form/api-create-form.component'
import { ApiDetailsComponent } from './details/api-details.component';
import { ApiSettingsFormComponent } from './settings-form/api-settings-form.component';
import { ApiPermissionsComponent } from './permissions/api-permissions.component';
import { ApiPermissionInlineFormComponent } from './permissions/inline-form/api-permission-inline-form.component';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ApiRoutes),
    SharedModule,
    CoreModule,
    NgxsModule.forFeature([
        ApiListState,
        ApiDetailsState,
        ApiPermissionsState
    ])
  ],
  declarations: [
    ApiListComponent,
    ApiCreateFormComponent,
    ApiDetailsComponent,
    ApiSettingsFormComponent,
    ApiPermissionsComponent,
    ApiPermissionInlineFormComponent
  ],
  exports: [
    ApiListComponent
  ],
  entryComponents: [
    ApiCreateFormComponent
  ]
})

export class ApiModule {}
