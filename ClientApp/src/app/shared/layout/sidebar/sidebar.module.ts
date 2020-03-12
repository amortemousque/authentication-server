/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

import { NgModule } from '@angular/core';
import { AppSidebar } from './sidebar';
import { CommonModule } from '@angular/common';
import { AppAccordionDirective } from './menu-accordion/accordion.directive';
import { AppAccordionAnchorDirective } from './menu-accordion/accordionanchor.directive';
import { AppAccordionLinkDirective } from './menu-accordion/accordionlink.directive';


@NgModule({
  imports: [
    CommonModule
  ],
  exports: [
    AppSidebar,
    AppAccordionDirective,
    AppAccordionAnchorDirective,
    AppAccordionLinkDirective
  ],
  declarations: [
    AppSidebar,
    AppAccordionDirective,
    AppAccordionAnchorDirective,
    AppAccordionLinkDirective
  ]
})
export class AppSidebarModule { }
