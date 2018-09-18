import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageRoutingModule } from './manage-routing.module';
import { ManageComponent } from './components/manage/manage.component';
import { ClarityModule, ClrInputModule } from '@clr/angular';
import { ReactiveFormsModule } from '@angular/forms';
import { ContextMenuModule } from 'ngx-contextmenu';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [
    CommonModule,
    ManageRoutingModule,
    ClarityModule,
    ReactiveFormsModule,
    ClrInputModule,
    ContextMenuModule,
    HttpClientModule
  ],
  declarations: [ManageComponent]
})
export class ManageModule {}
