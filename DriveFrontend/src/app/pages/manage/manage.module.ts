import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageRoutingModule } from './manage-routing.module';
import { ManageComponent } from './components/manage/manage.component';
import { ClarityModule } from '@clr/angular';

@NgModule({
  imports: [CommonModule, ManageRoutingModule, ClarityModule],
  declarations: [ManageComponent]
})
export class ManageModule {}
