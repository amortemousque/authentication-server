import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';

import { HeaderComponent } from './header/header.component';
import { MenuComponent } from './menu/menu.component';
import { LayoutComponent } from './layout.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { CoreModule } from '../../core/core.module';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AppSidebarModule } from './sidebar/sidebar.module';
import { AppBarModule } from './app-bar/app-bar.module';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    CoreModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatMenuModule,
    MatToolbarModule,
    AppSidebarModule,
    AppBarModule,
    MatButtonModule
  ],
  declarations: [
    LayoutComponent,
    HeaderComponent,
    MenuComponent
  ],
  exports: [
    LayoutComponent,
    HeaderComponent,
    MenuComponent
  ]
})

export class LayoutModule {}
