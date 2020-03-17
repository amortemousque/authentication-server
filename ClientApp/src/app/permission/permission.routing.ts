import { Routes } from '@angular/router';
import { LayoutComponent } from '../layout/layout.component';
import { PermissionListComponent } from './list/permission-list.component';
import { AuthGuard } from '../auth/guards/auth.guard';
import { RoleGuard } from '../auth/guards/role.guard';

export const PermissionRoutes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: {
      authorizedRoles: ['ROOT']
    },
    children: [
      {
        path: '',
        component: PermissionListComponent
      }
    ]
  }
];
