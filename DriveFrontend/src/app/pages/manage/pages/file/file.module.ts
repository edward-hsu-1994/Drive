import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FileRoutingModule } from './file-routing.module';
import { FileBrowserComponent } from './components/file-browser/file-browser.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { DirectoryTreeComponent } from './components/directory-tree/directory-tree.component';
import { ClarityModule } from '@clr/angular';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { ReactiveFormsModule } from '@angular/forms';
import { NgMarqueeModule } from 'ng-marquee';

@NgModule({
  imports: [
    CommonModule,
    FileRoutingModule,
    ClarityModule,
    DragToSelectModule,
    ReactiveFormsModule,
    NgMarqueeModule
  ],
  declarations: [
    FileBrowserComponent,
    BreadcrumbComponent,
    DirectoryTreeComponent
  ]
})
export class FileModule {}
