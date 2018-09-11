import { Component, OnInit, Input, Output } from '@angular/core';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  @Input()
  segments: [];

  @Input()
  paths: [];

  constructor() {}

  ngOnInit() {}
}
