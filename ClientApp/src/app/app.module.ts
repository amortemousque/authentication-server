import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { NgxsModule, Store } from '@ngxs/store';
import { NgxsRouterPluginModule } from '@ngxs/router-plugin';
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin';
import { NgxsLoggerPluginModule } from '@ngxs/logger-plugin';
import { NgxPermissionsModule, NgxPermissionsService, NgxRolesService, NgxPermissionsGuard } from "ngx-permissions";
import { PermissionInitializerFactory } from './core/permission/permission-initializer-factory';
import { AppAuthModule } from './core/auth/auth.module';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    NgxsModule.forRoot([], { developmentMode: true }),
    NgxsRouterPluginModule.forRoot(),
    NgxsReduxDevtoolsPluginModule.forRoot(),
    NgxsLoggerPluginModule.forRoot(),
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    AppAuthModule.forRoot({
      issuer: environment.authConfig.issuer,
      redirectUri: environment.authConfig.redirectUri,
      clientId: environment.authConfig.clientId,
      scope: environment.authConfig.scope,
      responseType: "id_token token"
    }),
    CoreModule,
    SharedModule,
    NgxPermissionsModule.forRoot(),
    RouterModule.forRoot([
      {
        path: 'api',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['authentications:write:root']
          }
        },
        loadChildren: () => import('./api/api.module').then(m => m.ApiModule)
      },
      {
        path: 'client',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['authentications:write:root']
          }
        },
        loadChildren: () => import('./client/client.module').then(m => m.ClientModule)
      },
      {
        path: 'tenant',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['tenants:write:root']
          }
        },
        loadChildren: () => import('./tenant/tenant.module').then(m => m.TenantModule)
      },
      {
        path: 'user',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['users:write:root']
          }
        },
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
      },
      {
        path: 'permission',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['permissions:write:root']
          }
        },
        loadChildren: () => import('./permission/permission.module').then(m => m.PermissionModule)
      },
      {
        path: 'role',
        canActivate: [NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['roles:write:root']
          }
        },
        loadChildren: () => import('./role/role.module').then(m => m.RoleModule)
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'user'
      }
    ]),
    BrowserAnimationsModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: PermissionInitializerFactory,
      deps: [NgxPermissionsService, NgxRolesService, HttpClient, Store],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
