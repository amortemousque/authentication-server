import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppBar, AppBarAvatar, AppAppBarHeadline, AppBarLogo } from './app-bar';

@NgModule({
  imports: [
    CommonModule
  ],
  exports: [
    AppBar,
    AppBarAvatar,
    AppAppBarHeadline,
    AppBarLogo
  ],
  declarations: [
    AppBar,
    AppBarAvatar,
    AppAppBarHeadline,
    AppBarLogo
  ],
})
export class AppBarModule { }