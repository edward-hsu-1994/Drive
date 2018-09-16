import { Component } from '@angular/core';
import { HttpClientBase } from './services/http-client-base.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'DriveFrontend';
  loading = false;

  constructor(private http: HttpClientBase) {
    this.http.beforeProcess.subscribe(x => {
      this.loading = true;
    });
    this.http.afterProcess.subscribe(x => {
      this.loading = false;
    });
    this.http.onError.subscribe(x => {
      this.loading = false;
    });
  }
}
