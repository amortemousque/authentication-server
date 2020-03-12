import { Directive, ElementRef, Input, OnChanges, OnInit, Renderer2 } from '@angular/core';


@Directive({ selector: '[appLoadingForm]' })
export class LoadingFormDirective implements OnInit, OnChanges {

  @Input('appLoadingForm') appLoadingForm: string;

  constructor(
    private renderer: Renderer2,
    private el: ElementRef,
  ) {
  }

  ngOnInit() {
    this.renderer.addClass(this.el.nativeElement, 'loading-form');
    if (this.appLoadingForm) {
      this.renderer.addClass(this.el.nativeElement, 'is-loading');
    }

  }

  ngOnChanges(changes) {
    if (this.appLoadingForm) {
      this.renderer.addClass(this.el.nativeElement, 'is-loading');
    } else {
      this.renderer.removeClass(this.el.nativeElement, 'is-loading');
    }
  }
}
