import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { AuthGuard } from './core/auth/guards/auth.guard';
import { RoleGuard } from './core/auth/guards/role.guard';
import { NgxsModule } from '@ngxs/store';
import { CallbackComponent } from './core/auth/callback/callback.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    CoreModule,
    SharedModule,
    NgxsModule.forRoot(),
    RouterModule.forRoot([
      {
        path: 'user',
        canActivate: [AuthGuard, RoleGuard],
        data: {
          authorizedRoles: ['ROOT', 'PARTNER']
        },
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
      },
      {
        path: 'api',
        canActivate: [AuthGuard, RoleGuard],
        data: {
          authorizedRoles: ['ROOT', 'PARTNER']
        },
        loadChildren: () => import('./api/api.module').then(m => m.ApiModule)
      },
      {
        path: 'callback',
        component: CallbackComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'user'
      }
    ]),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
