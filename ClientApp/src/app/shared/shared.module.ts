import { NgModule } from '@angular/core';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatLineModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AppMessageModule } from './message/message.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LoadingFormDirective } from './loading-form/loading-form.directive';
import { ReferencePipe, ReferencesPipe, ParenthesisPipe } from './pipes';
import { LayoutModule } from './layout/layout.module';

@NgModule({
  declarations: [
    LoadingFormDirective,
    // pipesTenantService
    ReferencePipe,
    ReferencesPipe,
    ParenthesisPipe,
  ],
  imports: [
    MatIconModule,
    MatCardModule,
    MatInputModule,
    MatCheckboxModule,
    MatSidenavModule,
    MatProgressBarModule,
    MatToolbarModule,
    MatTooltipModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatRadioModule,
    MatLineModule,
    MatListModule,
    MatMenuModule,
    MatDialogModule,
    MatButtonToggleModule,
    MatGridListModule,
    MatExpansionModule,
    MatTabsModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatStepperModule,
    MatChipsModule,
    MatSnackBarModule,
    MatTableModule,
    MatRippleModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatButtonModule,
    AppMessageModule,
    NgxDatatableModule,
    LayoutModule
  ],
  exports: [
    MatIconModule,
    MatCardModule,
    MatInputModule,
    MatCheckboxModule,
    MatSidenavModule,
    MatProgressBarModule,
    MatToolbarModule,
    MatTooltipModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatRadioModule,
    MatLineModule,
    MatListModule,
    MatMenuModule,
    MatDialogModule,
    MatButtonToggleModule,
    MatGridListModule,
    MatExpansionModule,
    MatTabsModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatStepperModule,
    MatChipsModule,
    MatSnackBarModule,
    MatTableModule,
    MatRippleModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatButtonModule,
    AppMessageModule,
    NgxDatatableModule,
    LoadingFormDirective,
    ReferencePipe,
    ReferencesPipe,
    ParenthesisPipe,
    LayoutModule
  ]
})
export class SharedModule {}