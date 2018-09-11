import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FileRoutingModule } from './file-routing.module';
import { FileBrowserComponent } from './components/file-browser/file-browser.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { DirectoryTreeComponent } from './components/directory-tree/directory-tree.component';
import { ClarityModule } from '@clr/angular';

@NgModule({
  imports: [CommonModule, FileRoutingModule, ClarityModule],
  declarations: [
    FileBrowserComponent,
    BreadcrumbComponent,
    DirectoryTreeComponent
  ]
})
export class FileModule {}
