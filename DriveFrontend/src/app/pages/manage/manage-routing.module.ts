import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ManageComponent } from './components/manage/manage.component';

const routes: Routes = [
  {
    path: '',
    component: ManageComponent,
    children: [
      {
        path: 'user',
        loadChildren: './pages/user/user.module#UserModule'
      },
      {
        path: 'file',
        loadChildren: './pages/file/file.module#FileModule'
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'file'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManageRoutingModule {}
