import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-menu',
  styleUrls: ['menu.component.scss'],
  templateUrl: './menu.component.html',
})
export class MenuComponent {
  currentLang = 'en';

  constructor(public dialog: MatDialog, public authService: AuthService) {
  }
}
