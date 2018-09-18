import {
  Component,
  OnInit,
  Input,
  ChangeDetectorRef,
  AfterViewInit,
  AfterContentChecked
} from '@angular/core';
import { FileNode } from '../../models/fileNode';
import { ClrLoading, LoadingListener } from '@clr/angular';
import { FileService } from '../../services/file.service';

@Component({
  selector: 'app-directory-tree',
  templateUrl: './directory-tree.component.html',
  styleUrls: ['./directory-tree.component.css']
})
export class DirectoryTreeComponent implements OnInit {
  @Input()
  children: FileNode[] = [];

  @Input()
  parent: FileNode = null;

  @Input()
  expanded = false;

  constructor(private fileService: FileService) {}

  ngOnInit() {
    if (this.parent) {
      let path = this.parent.relativePath;
      if (this.parent.relativePath[0] === '/') {
        path = path.substring(1);
      }
      this.fileService
        .list(path, null, 'Directory', 0, 2147483647)
        .subscribe(x => {
          this.children = x['result'];
        });
    }
  }
}
