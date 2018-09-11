import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-directory-tree',
  templateUrl: './directory-tree.component.html',
  styleUrls: ['./directory-tree.component.css']
})
export class DirectoryTreeComponent implements OnInit {
  expanded = true;
  constructor() {}

  ngOnInit() {}
}
