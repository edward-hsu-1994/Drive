import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { LoginPanelComponent } from './components/login-panel/login-panel.component';

@NgModule({
  imports: [CommonModule, AuthRoutingModule],
  declarations: [LoginPanelComponent]
})
export class AuthModule {}
