import {
  Component,
  OnInit,
  HostBinding,
  ViewChild,
  HostListener,
  AfterViewInit,
  ElementRef
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FileService } from '../../services/file.service';
import { SelectContainerComponent } from 'ngx-drag-to-select';
import { fromEvent } from 'rxjs';
import { ContextMenuComponent } from 'ngx-contextmenu';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-file-browser',
  templateUrl: './file-browser.component.html',
  styleUrls: ['./file-browser.component.css']
})
export class FileBrowserComponent implements OnInit, AfterViewInit {
  segments = [];
  paths = [];
  fullPath = null;

  query = null;

  filelist = [];
  selectedFiles = [];

  renameForm = new FormGroup({
    name: new FormControl('', Validators.required)
  });
  createDirectoryForm = new FormGroup({
    name: new FormControl('', Validators.required)
  });

  renameTarget;

  get selectedFilesCount() {
    return this.selectedFiles.filter(
      x => x.type !== 'Parent' && x.type !== 'LoadMore'
    ).length;
  }

  nextlistUrl = null;

  showDeleteDialog = false;
  showRenameDialog = false;
  showCreateDirectoryDialog = false;
  @ViewChild(SelectContainerComponent)
  fileListSelector: SelectContainerComponent;

  @ViewChild('fileListContainer')
  fileListContainer: ElementRef;

  @HostBinding('class.content-container')
  true;

  @ViewChild(ContextMenuComponent)
  basicMenu: ContextMenuComponent;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fileService: FileService
  ) {
    route.url.subscribe(x => {
      this.segments = ['root'].concat(x.map(y => y.path));
      this.paths = [''].concat(this.segments.slice(1));

      this.query = this.route.snapshot.queryParams.q;
      if (this.paths.join('/') !== this.fullPath) {
        this.fullPath = this.paths.join('/');
        this.load();
      }
    });
    route.queryParams.subscribe(x => {
      const old_query = this.query;

      this.query = this.route.snapshot.queryParams.q;

      if (old_query !== this.query) {
        this.load();
      }
    });
  }

  ngOnInit() {}

  ngAfterViewInit() {
    fromEvent(this.fileListContainer.nativeElement, 'scroll').subscribe(x => {
      this.fileListSelector.update();
    });
  }

  load() {
    this.fileService.list(this.paths.join('/')).subscribe(x => {
      if (this.paths.length > 1) {
        this.filelist = [{ name: '..', type: 'Parent' }].concat(x['result']);
      } else {
        this.filelist = x['result'];
      }

      if (x['hasNextPage']) {
        this.nextlistUrl = x['next'];
        this.filelist.push({
          name: '讀取更多',
          type: 'LoadMore'
        });
      } else {
        this.nextlistUrl = null;
      }
    });
  }

  loadMore() {
    this.fileService.http.get(this.nextlistUrl, {}).subscribe(x => {
      this.filelist = this.filelist
        .filter(y => y.type !== 'LoadMore')
        .concat(x['result']);
      if (x['hasNextPage']) {
        this.nextlistUrl = x['next'];
        this.filelist.push({
          name: '讀取更多',
          type: 'LoadMore'
        });
      } else {
        this.nextlistUrl = null;
      }
      console.log(this.filelist);
    });
  }

  openOrDownload(item) {
    if (item.type === 'Directory' || item.type === 'Parent') {
      this.router.navigate([item.name], { relativeTo: this.route });
    } else if (item.type === 'LoadMore') {
      this.loadMore();
    } else {
      window.open(item.downloadUrl);
    }
  }

  fileItemActionButton(item) {
    if (item.type === 'LoadMore') {
      this.loadMore();
    } else if (item.type === 'Parent') {
      this.router.navigate([item.name], { relativeTo: this.route });
    }
  }

  isLoadMore(item) {
    return item.type === 'LoadMore';
  }
  isParent(item) {
    return item.type === 'Parent';
  }
  isDirectory(item) {
    return item.type === 'Directory';
  }
  isImage(item) {
    return item.type === 'File' && item.contentType.indexOf('image') === 0;
  }

  selectAll() {
    this.fileListSelector.selectAll();
  }

  clearSelect() {
    this.fileListSelector.clearSelection();
  }

  deleteSelect() {
    if (this.selectedFiles.length === 0) {
      return;
    }
    this.showDeleteDialog = true;
  }

  renameAction(item) {
    this.renameTarget = item.relativePath;
    this.renameForm.setValue({ name: item.name });
    this.showRenameDialog = true;
  }

  rename(newName) {
    const newPath = this.renameTarget.split('/');
    newPath.splice(-1, 1);
    newPath.push(newName);

    this.fileService.move(this.renameTarget, newPath.join('/')).subscribe(x => {
      this.load();
      this.renameForm.reset();
      this.showRenameDialog = false;
    });
  }

  move() {}

  delete() {
    const targets = this.selectedFiles
      .filter(x => x.type !== 'Parent' && x.type !== 'LoadMore')
      .map(x => x.relativePath);

    this.fileService.delete(targets).subscribe(x => {
      this.load();
      this.showDeleteDialog = false;
    });
  }
  createDirectoryAction() {
    this.showCreateDirectoryDialog = true;
  }
  createDirectory() {
    this.fileService
      .createDirectory(this.fullPath, this.createDirectoryForm.value.name)
      .subscribe(x => {
        this.showCreateDirectoryDialog = false;
        this.load();
      });
  }

  uploadFile() {
    const inputElement = document.createElement('input');
    inputElement.type = 'file';
    inputElement.multiple = true;

    fromEvent(inputElement, 'change').subscribe(x => {
      this.fileService
        .upload(this.fullPath, inputElement.files)
        .subscribe(y => {
          this.load();
        });
    });
    inputElement.click();
  }
}
