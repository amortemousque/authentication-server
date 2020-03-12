import { Component, ElementRef, NgZone, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { AppSidebarService } from './sidebar/sidebar.service';


const SMALL_WIDTH_BREAKPOINT = 960;

@Component({
  selector: 'app-layout',
  styleUrls: ['layout.component.scss'],
  templateUrl: './layout.component.html'
})
export class LayoutComponent implements OnInit {

  private _router: Subscription;

  url: string;
  sidePanelOpened = true;
  options = {
    collapsed: false,
    compact: false,
    boxed: false,
    dark: false,
    dir: 'ltr'
  };

  @ViewChild('sidemenu', {static: true}) sidemenu;


  constructor(
    private _element: ElementRef,
    private sidebarService: AppSidebarService,
    private router: Router,
    zone: NgZone) {
 
  }

  ngOnInit(): void {

    this.url = this.router.url;

  }

  toggleSidebar() {
    this.sidebarService.getSidebar('sidebar').toggle();
  }
}
