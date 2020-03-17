import { Routes } from '@angular/router';
import { ClientListComponent } from './list/client-list.component';
import { ClientDetailsComponent } from './details/client-details.component';
export const ClientRoutes: Routes = [
  {
    path: '',
    component: ClientListComponent
  },
  {
    path: 'details/:id',
    component: ClientDetailsComponent,
  }
]

