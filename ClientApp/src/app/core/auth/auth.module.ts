import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from './auth.service';
import { RoleGuardDirective } from './directives/role-guard.directive';
import { OAuthModule } from 'angular-oauth2-oidc';

@NgModule({
  declarations: [
    RoleGuardDirective
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
