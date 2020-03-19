import { Component, Directive, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';


@Directive({
  selector: 'app-page-header-actions, [app-page-header-actions], [appPageHeaderActions]',
  host: { 'class': 'app-page-header-actions' }
})
export class PageHeaderActions { }

@Directive({
  selector: '[app-page-header-title], [appPageHeaderTitle]',
  host: { 'class': 'app-page-header-title' }
})
export class PageHeaderTitle { }

@Directive({
  selector: '[app-page-header-subtitle], [appPageHeaderSubtitle]',
  host: { 'class': 'app-page-header-subtitle' }
})
export class PageHeaderSubtitle { }

@Component({
  selector: 'app-page-header',
  styleUrls: ['page-header.scss'],
  templateUrl: './page-header.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'class': 'app-page-header' }
})
export class PageHeader {

  constructor() {
  }
}
