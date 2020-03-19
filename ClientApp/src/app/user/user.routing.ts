import { Routes } from '@angular/router';
import { UserListComponent } from './list/user-list.component';
import { UserDetailsComponent } from './details/user-details.component';


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
