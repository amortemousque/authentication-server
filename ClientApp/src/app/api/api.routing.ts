import { Routes } from '@angular/router';
import { ApiListComponent } from './list/api-list.component';
import { ApiDetailsComponent } from './details/api-details.component';


export const ApiRoutes: Routes = [
  {
    path: '',
    component: ApiListComponent
  },
  {
    path: 'details/:id',
    component: ApiDetailsComponent,
  }
]
