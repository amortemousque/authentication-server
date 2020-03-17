import { Routes } from '@angular/router';
import { LayoutComponent } from '../layout/layout.component';
import { RoleListComponent } from './list/role-list.component';
import { RoleDetailsComponent } from './details/role-details.component';
import { AuthGuard } from '../auth/guards/auth.guard';
import { RoleGuard } from '../auth/guards/role.guard';

export const RoleRoutes: Routes = [
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
        component: RoleListComponent
      },
      {
        path: 'details/:id',
        component: RoleDetailsComponent,
      }
    ]
  }
];
