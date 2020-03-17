import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { ClientListComponent } from './list/client-list.component'
import { ClientListState } from './list/client-list.state';
import { ClientDetailsState } from './details/client-details.state';

import { ClientRoutes } from './client.routing';

import { ClientCreateFormComponent } from './create-form/client-create-form.component'
import { ClientDetailsComponent } from './details/client-details.component';
import { ClientSettingsFormComponent } from './settings-form/client-settings-form.component';
import { CoreModule } from '../core/core.module';
import { NgxsModule } from '@ngxs/store';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ClientRoutes),
    SharedModule,
    CoreModule,
    NgxsModule.forFeature([
        ClientListState,
        ClientDetailsState
    ])
  ],
  declarations: [
    ClientListComponent,
    ClientCreateFormComponent,
    ClientDetailsComponent,
    ClientSettingsFormComponent
  ],
  exports: [
    ClientListComponent
  ],
  entryComponents: [
    ClientCreateFormComponent
  ]
})

export class ClientModule {}
