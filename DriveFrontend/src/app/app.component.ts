import {
  Component,
  ChangeDetectorRef,
  OnInit,
  AfterContentInit
} from '@angular/core';
import { HttpClientBase } from './services/http-client-base.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterContentInit {
  title = 'DriveFrontend';
  loading = false;

  constructor(private http: HttpClientBase, private cdr: ChangeDetectorRef) {}

  ngAfterContentInit(): void {
    this.http.beforeProcess.subscribe(x => {
      this.loading = true;
      this.cdr.detectChanges();
    });
    this.http.afterProcess.subscribe(x => {
      this.loading = false;
      this.cdr.detectChanges();
    });
    this.http.onError.subscribe(x => {
      this.loading = false;
      this.cdr.detectChanges();
    });
  }
}
