import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from './auth.service';
import { RoleGuardDirective } from './directives/role-guard.directive';
import { OAuthModule } from 'angular-oauth2-oidc';
import { CallbackComponent } from './callback/callback.component';

@NgModule({
  declarations: [
    RoleGuardDirective,
    CallbackComponent
  ],
  imports: [
    CommonModule,
    OAuthModule
  ],
  exports: [
    RoleGuardDirective
  ]
})
export class AuthModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: AuthModule
    };
  }
}
