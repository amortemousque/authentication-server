import { Injectable } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { AuthUser  } from './models/authUser';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: "root"
})
export class AuthService {


  private _authConfig: AuthConfig = {
    issuer: environment.authConfig.issuer,
    requireHttps: false,
    redirectUri: environment.authConfig.redirectUri,
    clientId: environment.authConfig.clientId,
    scope:  environment.authConfig.scope,
  };

  private _user: AuthUser;

  constructor(
    private oauthService: OAuthService) 
  {

  }

  public loadDiscoveryDocumentAndTryLogin() {
  
    this.oauthService.configure(this._authConfig);
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then((ret) => {
      console.log('coucou', ret)

    })
    .catch(error => {
      console.log('coucou2', error)

    })
  }

  public login() {
    this.oauthService.initImplicitFlow();
  }

  public logoff() {
    this.oauthService.logOut();
  }

  public get user() {

    if (this._user == null) {

      const claims = this.oauthService.getIdentityClaims();
      console.log('test', claims)

      this._user = new AuthUser();
      if (claims != null) {
        this._user.mapClaims(claims);
      }
    }
    return this._user;
  }

  public get access_token(): string {
    return this.oauthService.getAccessToken();
  }

  public get tokenValid(): boolean {
    const expiresAt = this.oauthService.getAccessTokenExpiration();
    // Check if current time is past access token's expiration
    return Date.now() < expiresAt;
  }

  public isInRole(role: string): boolean {
    if (Array.isArray(this.user.role)) {
      return this.user.role.includes(role);
    } else if (this.user.role == role) {
      return true;
    }
    return false;
  }

  public setRegistrationComplete(date) {
    localStorage.setItem('registrationCompletedDate', JSON.stringify(date));
  }
}
