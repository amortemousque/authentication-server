import { Component, EventEmitter, Output } from '@angular/core';


@Component({
  selector: 'app-header',
  styleUrls: ['header.component.scss'],
  templateUrl: './header.component.html'
})
export class HeaderComponent {

  @Output() toggleSidenav = new EventEmitter<void>();

  constructor() {
  }

}
