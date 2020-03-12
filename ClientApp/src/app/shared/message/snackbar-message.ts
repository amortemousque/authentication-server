import { Component, Inject, Directive, ViewEncapsulation, ElementRef } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';

@Directive({
  selector: `app-snackbar-icon, [app-snackbar-icon], [appSnackbarIcon]`,
  host: {
    'class': 'app-snackbar-icon'
  }
})
export class AppSnackbarIcon { }

@Component({
  selector: 'app-snackbar-message',
  templateUrl: './snackbar-message.html',
  styleUrls: ['./snackbar-message.scss'],
  encapsulation: ViewEncapsulation.None,
  host: { 'class': 'app-snackbar-message mat-typography' },
})
export class AppSnackbarMessage {

  message: string;
  icon: string;

  constructor(
    elementRef: ElementRef,
    public snackBarRef: MatSnackBarRef<AppSnackbarMessage>,
    @Inject(MAT_SNACK_BAR_DATA) public data: any) {
    this.message = data.message;
    this.icon = data.icon;

    if (data.color)
      elementRef.nativeElement.classList.add(`app-snackbar-${data.color}`);

  }

  undo() {
    this.snackBarRef.dismissWithAction();
  }

  close() {
    this.snackBarRef.dismiss();
  }
}