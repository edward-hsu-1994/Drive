import {
  Component,
  OnInit,
  HostBinding,
  ViewChild,
  HostListener,
  AfterViewInit,
  ElementRef
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

  query = null;

  filelist = [];
  nextlistUrl = null;

  @ViewChild(SelectContainerComponent)
  fileListSelector: SelectContainerComponent;

  @ViewChild('fileListContainer')
  fileListContainer: ElementRef;

  @HostBinding('class.content-container')
  true;

  constructor(private route: ActivatedRoute, private fileService: FileService) {
    route.url.subscribe(x => {
      this.segments = ['root'].concat(x.map(y => y.path));
      this.paths = [''];

      for (const path of this.segments.slice(1)) {
        const temp = this.paths[this.paths.length - 1] + '/' + path;
        this.paths.push(temp);
      }

      this.paths = this.paths.map(y => y.substring(1));

      this.query = this.route.snapshot.queryParams.q;

      this.load();
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
    console.log(this.fileListContainer.nativeElement);
    fromEvent(this.fileListContainer.nativeElement, 'scroll').subscribe(x => {
      this.fileListSelector.update();
    });
  }

  load() {
    this.fileService.list(this.paths.join('/')).subscribe(x => {
      this.filelist = x['result'];
      this.nextlistUrl = x['next'];
      this.loadMore();
    });
  }

  loadMore() {
    this.fileService.http.get(this.nextlistUrl, {}).subscribe(x => {
      this.filelist = this.filelist.concat(x['result']);
      this.nextlistUrl = x['next'];
      console.log(this.filelist);
    });
  }
}
