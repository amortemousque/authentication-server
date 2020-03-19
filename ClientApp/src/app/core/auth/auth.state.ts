import { Action, State, StateContext, Selector, NgxsOnInit } from '@ngxs/store';
import { AuthUser } from './auth.model';
import { OAuthService } from 'angular-oauth2-oidc';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Injectable, Inject } from '@angular/core';
import { AUTH_CONFIG, AppAuthConfig } from './auth-config';
import { from } from 'rxjs';

export class LoginUser {
  static readonly type = '[Authentication] Login User';
}

export class RefreshToken {
  static readonly type = '[Authentication] Refresh Token';
}

export class LoadUser {
  static readonly type = '[Authentication] Load User';
}

export class AuthenticateUser {
  static readonly type = '[Authentication] Load Token';
}

export class LogoutUser {
  static readonly type = '[Authentication] Logout Currently Logged-in User';
}

export interface AuthenticationStateModel {
  user: AuthUser;
  accessToken: string;
  tokenExpiration: number;
}

const authStateDefaults: AuthenticationStateModel = {
  accessToken: null,
  user: null,
  tokenExpiration: null
};

@State<AuthenticationStateModel>({
  name: 'authentication',
  defaults: authStateDefaults
})
export class AuthenticationState implements NgxsOnInit {

  ngxsOnInit(ctx?: StateContext<any>) {
    this.oauthService.loadDiscoveryDocumentAndTryLogin({
      onTokenReceived: context => {
        const expiration = this.oauthService.getAccessTokenExpiration();
        const isValid = this.oauthService.hasValidAccessToken();
        ctx.setState({
          accessToken: context.accessToken,
          tokenExpiration: expiration,
          valid: isValid,
        });
        ctx.dispatch(new AuthenticateUser());
      }
    }).then(result => {
      if (!result) {
        ctx.dispatch(new LoginUser());
      }
    });
  }

  _config: AppAuthConfig
  constructor(private oauthService: OAuthService, @Inject(AUTH_CONFIG) config: AppAuthConfig, private http: HttpClient) {
    this._config = config;
    this.oauthService.configure(config);
  }

  private static isDateValid(timestamp: number | string): boolean {
    const currentDate = new Date();
    const time = currentDate.getTime();
    const number = Number(timestamp);
    return timestamp != null && number > time;
  }

  @Selector()
  static currentUser(state: AuthenticationStateModel) {
    return state.user;
  }

  @Selector()
  static valid(state: AuthenticationStateModel) {
    return AuthenticationState.isDateValid(state.tokenExpiration);
  }

  @Selector()
  static accessToken(state: AuthenticationStateModel) {
    return state.accessToken;
  }

  @Action(LoginUser)
  loginUser(_ctx: StateContext<AuthenticationStateModel>) {
    this.oauthService.initImplicitFlow();
  }

  @Action(LogoutUser)
  logoutUser({ setState }: StateContext<AuthenticationStateModel>) {
    this.oauthService.logOut();
    setState(authStateDefaults);
  }

  @Action(RefreshToken)
  refreshToken({ patchState }: StateContext<AuthenticationStateModel>) {
    return from(this.oauthService.silentRefresh()).pipe(
      tap(info => {
        patchState({
          accessToken: this.oauthService.getAccessToken()
        });
      })
    );
  }

  @Action(AuthenticateUser)
  authenticateUser({ patchState, getState }: StateContext<AuthenticationStateModel>) {
    let claims = this.oauthService.getIdentityClaims() as any;
    let user = <AuthUser>{
      id: claims.sub,
      name: claims.name,
      familyName: claims.family_name,
      email: claims.email,
      givenName: claims.given_name,
      tenantId: claims.tenant_id,
      tenantName: claims.tenant_name,
      role: claims.role,
      permission: claims.permission,
    }

    patchState({
      user: user
    });
    return user;
  }
}