import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageRoutingModule } from './manage-routing.module';
import { ManageComponent } from './components/manage/manage.component';
import { ClarityModule } from '@clr/angular';
import { ReactiveFormsModule } from '@angular/forms';
import { ContextMenuModule } from 'ngx-contextmenu';

@NgModule({
  imports: [
    CommonModule,
    ManageRoutingModule,
    ClarityModule,
    ReactiveFormsModule,
    ContextMenuModule
  ],
  declarations: [ManageComponent]
})
export class ManageModule {}
