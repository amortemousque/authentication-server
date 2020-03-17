import { Routes } from '@angular/router';
import { LayoutComponent } from '../layout/layout.component';
import { ClientListComponent } from './list/client-list.component';
import { ClientDetailsComponent } from './details/client-details.component';
import { AuthGuard } from '../auth/guards/auth.guard';
import { RoleGuard } from '../auth/guards/role.guard';

export const ClientRoutes: Routes = [
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
        component: ClientListComponent
      },
      {
        path: 'details/:id',
        component: ClientDetailsComponent,
      }
    ]
  }
];
