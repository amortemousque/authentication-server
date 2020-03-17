import { Routes } from '@angular/router';
import { TenantListComponent } from './list/tenant-list.component';
import { TenantDetailsComponent } from './details/tenant-details.component';

export const TenantRoutes: Routes = [
  {
    path: '',
    component: TenantListComponent
  },
  {
    path: 'details/:id',
    component: TenantDetailsComponent,
  }
];
