import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserListResolve } from './services/user-list-resolve.service';

const routes: Routes = [
  {
    path: '',
    component: UserListComponent,
    resolve: {
      userlist: UserListResolve
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule {}
