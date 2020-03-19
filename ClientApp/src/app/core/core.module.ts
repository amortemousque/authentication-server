import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { OAuthModule } from "angular-oauth2-oidc";
import { AppAuthModule } from "./auth/auth.module";
import { environment } from "../../environments/environment";
import { NgxPermissionsModule } from "ngx-permissions";

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    NgxPermissionsModule,
    NgxDatatableModule,
    OAuthModule.forRoot()
  ],
  exports: [
    CommonModule,
    HttpClientModule,
    HttpClientModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    AppAuthModule,
    NgxPermissionsModule
  ]
})
export class CoreModule {}
