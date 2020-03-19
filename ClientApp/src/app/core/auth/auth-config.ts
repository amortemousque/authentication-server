import { InjectionToken } from '@angular/core';
import { AuthConfig } from 'angular-oauth2-oidc';

export const AUTH_CONFIG = new InjectionToken<AppAuthConfig>('qupAuth.config');
export class AppAuthConfig extends AuthConfig {
}