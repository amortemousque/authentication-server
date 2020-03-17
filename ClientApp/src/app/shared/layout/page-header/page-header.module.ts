/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

import { NgModule } from '@angular/core';
import { PageHeader, PageHeaderActions, PageHeaderTitle, PageHeaderSubtitle } from './page-header';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';


@NgModule({
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule
  ],
  exports: [
    PageHeaderActions,
    PageHeaderTitle,
    PageHeaderSubtitle,
    PageHeader
  ],
  declarations: [
    PageHeaderActions,
    PageHeaderTitle,
    PageHeaderSubtitle,
    PageHeader
  ]
})
export class AppPageHeaderModule { }
