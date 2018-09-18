import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NonLoginResolve } from './services/non-login-resolve.service';

const routes: Routes = [
  {
    path: 'manage',
    loadChildren: './pages/manage/manage.module#ManageModule',
    resolve: {
      nonLogin: NonLoginResolve
    }
  },
  {
    path: 'auth',
    loadChildren: './pages/auth/auth.module#AuthModule'
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/auth'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
