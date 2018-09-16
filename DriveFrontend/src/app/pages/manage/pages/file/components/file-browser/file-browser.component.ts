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
  nextlistUrl = null;

  @ViewChild(SelectContainerComponent)
  fileListSelector: SelectContainerComponent;

  @ViewChild('fileListContainer')
  fileListContainer: ElementRef;

  @HostBinding('class.content-container')
  true;

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

  goToDirectory(item) {
    if (
      item.type !== 'Directory' &&
      item.type !== 'Parent' &&
      item.type !== 'LoadMore'
    ) {
      return;
    }

    if (item.type === 'LoadMore') {
      this.loadMore();
    } else {
      this.router.navigate([item.name], {
        relativeTo: this.route
      });
    }
  }

  download(item) {
    if (item.type !== 'File') {
      return;
    }
    window.open(item.downloadUrl);
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
}
