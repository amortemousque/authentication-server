import { Directive, HostListener, Inject } from '@angular/core';

import { AppAccordionLinkDirective } from './accordionlink.directive';

@Directive({
  selector: '[appAccordionToggle]'
})
export class AppAccordionAnchorDirective {

  protected navlink: AppAccordionLinkDirective;

  constructor( @Inject(AppAccordionLinkDirective) navlink: AppAccordionLinkDirective) {
    this.navlink = navlink;
  }

  @HostListener('click', ['$event'])
  onClick(e: any) {
    this.navlink.toggle();
  }
}
