import { Routes } from '@angular/router';
import { LayoutComponent } from '../layout/layout.component';
import { TenantListComponent } from './list/tenant-list.component';
import { TenantDetailsComponent } from './details/tenant-details.component';
import { AuthGuard } from '../auth/guards/auth.guard';
import { RoleGuard } from '../auth/guards/role.guard';

export const TenantRoutes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: {
      authorizedRoles: ['ROOT', 'PARTNER']
    },
    children: [
      {
        path: '',
        component: TenantListComponent
      },
      {
        path: 'details/:id',
        component: TenantDetailsComponent,
      }
    ]
  }
];
