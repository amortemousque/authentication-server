import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';



import { LayoutModule } from '../layout/layout.module'

import { PermissionListComponent } from './list/permission-list.component'
import { PermissionListState } from './list/permission-list.state';

import { PermissionRoutes } from './permission.routing';

import { PermissionCreateFormComponent } from './create-form/permission-create-form.component'
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
    RouterModule.forChild(PermissionRoutes),
    AuthModule.forRoot(),
    CoreModule.forRoot(),
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
