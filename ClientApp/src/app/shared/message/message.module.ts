import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { FlexLayoutModule } from '@angular/flex-layout';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AppSnackbarIcon, AppSnackbarMessage } from './snackbar-message';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { MessageService } from './message.service';

@NgModule({
  imports: [
    CommonModule,
    MatDialogModule,
    MatDialogModule,
    MatIconModule,
    MatSnackBarModule,
    MatButtonModule,
    FlexLayoutModule
  ],
  declarations: [
    AppSnackbarIcon,
    AppSnackbarMessage,
    ConfirmationDialogComponent
  ],
  providers: [
    AppSnackbarIcon,
    AppSnackbarMessage,
    MessageService
  ],
  entryComponents: [
    AppSnackbarMessage,
    ConfirmationDialogComponent
  ]
})

export class AppMessageModule {}