import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FileBrowserComponent } from './components/file-browser/file-browser.component';

const routes: Routes = [
  {
    path: '**',
    runGuardsAndResolvers: 'always',
    component: FileBrowserComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FileRoutingModule {}
