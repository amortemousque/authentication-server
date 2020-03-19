import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OAuthModule } from 'angular-oauth2-oidc';
import { NgxsModule, Store } from '@ngxs/store';
import { AuthenticationState } from './auth.state';
import { AUTH_CONFIG, AppAuthConfig } from './auth-config';
import { AccessTokenHolder, DefaultAccessTokenHolder } from './access-token-holder';

@NgModule({
  imports: [
    CommonModule,
    OAuthModule,
    NgxsModule.forFeature([
      AuthenticationState
    ])
  ]
})
export class AppAuthModule {

  static forRoot(config: AppAuthConfig): ModuleWithProviders {
    return {
      ngModule: AppAuthModule,
      providers: [
        { provide: AUTH_CONFIG, useValue: config },
        { provide: AccessTokenHolder, useClass: DefaultAccessTokenHolder, deps: [Store] }
      ]
    }
  }
}