import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageRoutingModule } from './manage-routing.module';
import { ManageComponent } from './components/manage/manage.component';
import { ClarityModule } from '@clr/angular';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    ManageRoutingModule,
    ClarityModule,
    ReactiveFormsModule
  ],
  declarations: [ManageComponent]
})
export class ManageModule {}
