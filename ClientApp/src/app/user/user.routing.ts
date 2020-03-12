import { Routes } from '@angular/router';
import { UserListComponent } from './list/user-list.component';
import { UserDetailsComponent } from './details/user-details.component';
import { AuthGuard } from '../core/auth/guards/auth.guard';
import { RoleGuard } from '../core/auth/guards/role.guard';


export const UserRoutes: Routes = [
  {
    path: 'list',
    component: UserListComponent
  },
  {
    path: 'details/:id',
    component: UserDetailsComponent,
  },
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full'
  }
];
