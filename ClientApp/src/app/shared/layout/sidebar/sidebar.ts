import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { BehaviorSubject } from 'rxjs';
import { AppSidebarService } from './sidebar.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.scss'],
  encapsulation: ViewEncapsulation.None,
  host: { 'class': 'app-sidebar-container' }
})
export class AppSidebar implements OnInit, OnDestroy {

  @Input()
  width: number;

  @Input()
  name: string;

  @Input()
  position: 'left' | 'right' = 'left';

  @Input()
  set open(open) {
    this.opened.next(open);
  }

  opened: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  @Input()
  minimal: boolean = false;

  @Input()
  mode: 'over' | 'side' = 'side';

  private _firstMode: 'over' | 'side';

  constructor(
    public breakpointObserver: BreakpointObserver,
    private _sidebarService: AppSidebarService,
    private _mediaObserver: MediaObserver,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this._firstMode = this.mode;
    this._sidebarService.register(this.name, this);
    this._mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias == 'sm' || change.mqAlias == 'xs') {
        this.mode = 'over';
      } else if (this._firstMode != 'over') {
        this.mode = 'side';
      }
    });

    this.router.events.subscribe(() => {
      if (this.mode == 'over')
        this.close();
    });

  }

  ngOnDestroy(): void {
    this._sidebarService.unregister(this.name);
  }

  public close() {
    this.opened.next(false);
  }

  public toggle() {
    this.opened.next(!this.opened.getValue());
  }
}