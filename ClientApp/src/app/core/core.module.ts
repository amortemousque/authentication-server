import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { AuthModule } from "./auth/auth.module";
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FlexLayoutModule } from '@angular/flex-layout';
import { BrowserModule } from "@angular/platform-browser";
import { OAuthModule } from "angular-oauth2-oidc";

@NgModule({
  imports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    AuthModule,
    NgxDatatableModule,
    FlexLayoutModule,
    OAuthModule.forRoot()
  ],
  exports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    HttpClientModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    AuthModule,
    FlexLayoutModule
  ]
})
export class CoreModule {}
