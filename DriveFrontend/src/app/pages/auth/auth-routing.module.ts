import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPanelComponent } from './components/login-panel/login-panel.component';
import { LoginStatusResolveService } from './services/login-status-resolve.service';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: LoginPanelComponent,
    resolve: {
      tokenCheck: LoginStatusResolveService
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule {}
