import { Store } from '@ngxs/store';
import { AuthenticationState } from './auth.state';

export abstract class AccessTokenHolder {
  abstract getAccessToken(): string;
}

export class DefaultAccessTokenHolder implements AccessTokenHolder {

  constructor(private store: Store) {}

  getAccessToken(): string {
    return this.store.selectSnapshot(AuthenticationState.accessToken);
  }
}