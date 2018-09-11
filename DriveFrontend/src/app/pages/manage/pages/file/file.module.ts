import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FileRoutingModule } from './file-routing.module';
import { FileBrowserComponent } from './components/file-browser/file-browser.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

@NgModule({
  imports: [
    CommonModule,
    FileRoutingModule
  ],
  declarations: [FileBrowserComponent, BreadcrumbComponent]
})
export class FileModule { }
