import { Routes } from '@angular/router';
import { RoleListComponent } from './list/role-list.component';
import { RoleDetailsComponent } from './details/role-details.component';

export const RoleRoutes: Routes = [
  {
    path: '',
    component: RoleListComponent
  },
  {
    path: 'details/:id',
    component: RoleDetailsComponent,
  }
];
