import {
  Directive, HostBinding, Inject, Input, OnInit, OnDestroy
} from '@angular/core';

import { AppAccordionDirective } from './accordion.directive';


@Directive({
  selector: '[appAccordionLink]'
})
export class AppAccordionLinkDirective implements OnInit, OnDestroy {

  @Input() public group: any;

  @HostBinding('class.open')
  @Input()
  get open(): boolean {
    return this._open;
  }

  set open(value: boolean) {
    this._open = value;
    if (value) {
      this.nav.closeOtherLinks(this);
    }
  }

  protected _open: boolean;
  protected nav: AppAccordionDirective;

  constructor(@Inject(AppAccordionDirective) nav: AppAccordionDirective) {
    this.nav = nav;
  }

  ngOnInit(): any {
    this.nav.addLink(this);
  }

  ngOnDestroy(): any {
    this.nav.removeGroup(this);
  }

  toggle(): any {
    this.open = !this.open;
  }
}
