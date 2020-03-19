import { Component } from '@angular/core';
import { Select } from '@ngxs/store';
import { AuthenticationState } from '../../../core/auth/auth.state';
import { Observable } from 'rxjs';
import { AuthUser } from '../../../core/auth/auth.model';

@Component({
  selector: 'app-menu',
  styleUrls: ['menu.component.scss'],
  templateUrl: './menu.component.html',
})
export class MenuComponent {
  @Select(AuthenticationState.currentUser) currentUser$: Observable<AuthUser>;

  constructor() {
  }
}
