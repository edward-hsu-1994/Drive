import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { ClarityModule } from '@clr/angular';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { ContextMenuModule } from 'ngx-contextmenu';
import { LoadingModule } from 'ngx-loading';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ClarityModule,
    HttpClientModule,
    BrowserAnimationsModule,
    DragToSelectModule.forRoot(),
    ContextMenuModule.forRoot(),
    LoadingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
