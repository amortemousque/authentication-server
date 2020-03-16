import { Component } from '@angular/core';
import { AuthService } from './core/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  constructor(authService: AuthService) {
    authService.loadDiscoveryDocumentAndTryLogin();
  }
}
