import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PermissionListComponent } from './list/permission-list.component'
import { PermissionListState } from './list/permission-list.state';

import { PermissionRoutes } from './permission.routing';

import { PermissionCreateFormComponent } from './create-form/permission-create-form.component'
import { CoreModule } from '../core/core.module';
import { NgxsModule } from '@ngxs/store';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(PermissionRoutes),
    SharedModule,
    CoreModule,
    NgxsModule.forFeature([
        PermissionListState
    ])
  ],
  declarations: [
    PermissionListComponent,
    PermissionCreateFormComponent
  ],
  exports: [
    PermissionListComponent
  ],
  entryComponents: [
    PermissionCreateFormComponent
  ]
})

export class PermissionModule {}
