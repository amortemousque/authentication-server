import { Component, Directive, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Directive({
  selector: `[app-bar-headline], app-bar-headline, [appBarHeadline]`,
  host: {'class': 'app-bar-headline'}
})
export class AppAppBarHeadline {}


@Directive({
  selector: '[app-bar-avatar], [appBarAvatar]',
  host: { 'class': 'app-bar-avatar' }
})
export class AppBarAvatar { }

@Directive({
  selector: '[app-bar-logo], [appBarLogo]',
  host: { 'class': 'app-bar-logo' }
})
export class AppBarLogo { }

@Component({
  selector: 'app-bar',
  host: { 'class': 'app-bar' },
  templateUrl: './app-bar.html',
  styleUrls: ['./app-bar.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppBar {

  constructor() {

  }
}
