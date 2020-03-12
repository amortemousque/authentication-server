
import { filter } from 'rxjs/operators';
import { OnInit, AfterViewInit, AfterContentChecked, Directive, Input } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AppAccordionLinkDirective } from './accordionlink.directive';


@Directive({
  selector: '[appAccordion]',
})
export class AppAccordionDirective implements AfterContentChecked {

  _multi = false;
  @Input()
  multi: boolean
  set(multi) {
    if (multi !== false)
      this._multi = true;
  }

  protected navlinks: Array<AppAccordionLinkDirective> = [];

  closeOtherLinks(openLink: AppAccordionLinkDirective): void {
    if (this._multi)
      this.navlinks.forEach((link: AppAccordionLinkDirective) => {
        if (link !== openLink) {
          link.open = false;
        }
      });
  }

  addLink(link: AppAccordionLinkDirective): void {
    this.navlinks.push(link);
  }

  removeGroup(link: AppAccordionLinkDirective): void {
    const index = this.navlinks.indexOf(link);
    if (index !== -1) {
      this.navlinks.splice(index, 1);
    }
  }

  checkOpenLinks() {
    this.navlinks.forEach((link: AppAccordionLinkDirective) => {
      if (link.group) {
        const routeUrl = this.router.url;
        const currentUrl = routeUrl.split('/');
        if (currentUrl.indexOf(link.group) > 0) {
          link.open = true;
          this.closeOtherLinks(link);
        }
      }
    });
  }

  ngAfterContentChecked(): void {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(e => this.checkOpenLinks());
  }

  constructor(private router: Router) {
    setTimeout(() => this.checkOpenLinks());
  }
}
