import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
import { NavigationWarningDialogComponent } from './navigation-warning-dialog/navigation-warning-dialog.component';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { AppSnackbarMessage } from './snackbar-message';


@Injectable()
export class MessageService {

  constructor(
    public snackBar: MatSnackBar,
    public dialog: MatDialog,
  ) { }


  openConfirmationDialog(message: string): MatDialogRef<ConfirmationDialogComponent, any> {
    return this.dialog.open(ConfirmationDialogComponent, {
      width: '350px',
      data: {message:  message}
    });
  }

  openNavigationWarningDialog(): MatDialogRef<NavigationWarningDialogComponent, any> {
    return this.dialog.open(NavigationWarningDialogComponent, {
      width: '670px'
    });
  }

  openSuccessMessage(message: string): MatSnackBarRef<AppSnackbarMessage>  {   
    return this.snackBar.openFromComponent(AppSnackbarMessage, {
      duration: 3000,
      data: {
        message: message,
        icon: 'check',
        color: 'success'
      }
    });
  }
  
  openErrorMessage(message: string): MatSnackBarRef<AppSnackbarMessage> {     
    return this.snackBar.openFromComponent(AppSnackbarMessage, {
      duration: 10000,
      data: {
        message: message,
        icon: 'remove_circle',
        color: 'warn'
       }
    });
  }
}