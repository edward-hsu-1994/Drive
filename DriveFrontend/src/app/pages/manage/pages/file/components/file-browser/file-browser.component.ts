import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-file-browser',
  templateUrl: './file-browser.component.html',
  styleUrls: ['./file-browser.component.css']
})
export class FileBrowserComponent implements OnInit {
  segments = [];
  paths = [];

  constructor(private route: ActivatedRoute) {
    route.url.subscribe(x => {
      this.segments = ['root'].concat(x.map(y => y.path));
      this.paths = [''];
      for (const path of this.segments.slice(1)) {
        const temp = this.paths[this.paths.length - 1] + '/' + path;
        this.paths.push(temp);
      }
      this.paths = this.paths.map(y => y.substring(1));
      console.log(this.paths);
    });
  }

  ngOnInit() {}
}
