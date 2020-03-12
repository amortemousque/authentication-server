import { AuthService } from '../auth.service';
import { Directive, OnInit, OnChanges, Input, ViewContainerRef, TemplateRef } from '@angular/core';


@Directive({ selector: '[appRoleGuard]' })
export class RoleGuardDirective implements OnInit, OnChanges {

  @Input('appRoleGuard') authorizedRoles: string[];

  stepElements = [];

  constructor(
    private container: ViewContainerRef,
    private template: TemplateRef<any>,
    private auth: AuthService,
    
  ) {
  }

  ngOnInit() {
    let hasRight = false;
    const authorizedRoles = this.authorizedRoles;
    if (authorizedRoles) {
      hasRight = authorizedRoles.some(r => this.auth.isInRole(r));
    }

    if (hasRight) {
      this.container.createEmbeddedView(this.template);
    }
  }


  ngOnChanges(changes) {
  }
}
