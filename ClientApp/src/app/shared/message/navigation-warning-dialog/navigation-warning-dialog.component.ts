import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

export type NavigationWarningResult = 'Cancel' | 'Ignore' | 'Save';

@Component({ 
  selector: 'app-navigation-warning-dialog',
  templateUrl: './navigation-warning-dialog.component.html',
  styleUrls: ['./navigation-warning-dialog.component.scss']
})
export class NavigationWarningDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<NavigationWarningDialogComponent>) {
  }

  onCancel(): void {
    this.dialogRef.close('Cancel');
  }

  onIgnore(): void {
    this.dialogRef.close('Ignore');
  }

  onSave(): void {
    this.dialogRef.close('Save');
  }

}
