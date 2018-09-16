import { Component, OnInit, HostBinding } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileService } from '../../services/file.service';

@Component({
  selector: 'app-file-browser',
  templateUrl: './file-browser.component.html',
  styleUrls: ['./file-browser.component.css']
})
export class FileBrowserComponent implements OnInit {
  segments = [];
  paths = [];

  query = null;

  filelist = [];
  nextlistUrl = null;

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
      this.query = this.route.snapshot.queryParams.q;

      this.load();
    });
  }

  ngOnInit() {}

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
