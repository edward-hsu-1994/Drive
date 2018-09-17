import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import { UserListComponent } from './components/user-list/user-list.component';
import { ClarityModule, ClrInputModule } from '@clr/angular';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    UserRoutingModule,
    ClarityModule,
    ReactiveFormsModule,
    FormsModule,
    ClrInputModule
  ],
  declarations: [UserListComponent]
})
export class UserModule {}
