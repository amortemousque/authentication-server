import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export type NavigationWarningResult = 'Cancel' | 'Ignore' | 'Save';

@Component({ 
  selector: 'app-navigation-warning-dialog',
  templateUrl: './navigation-warning-dialog.component.html',
  styleUrls: ['./navigation-warning-dialog.component.scss']
})
export class NavigationWarningDialogComponent {
  message: string = '';
 
  constructor(

    public dialogRef: MatDialogRef<NavigationWarningDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
      this.message = data.message;
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
