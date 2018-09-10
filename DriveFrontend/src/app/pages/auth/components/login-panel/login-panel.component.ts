import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login-panel',
  templateUrl: './login-panel.component.html',
  styleUrls: ['./login-panel.component.css']
})
export class LoginPanelComponent implements OnInit {
  error: string = null;
  constructor(private route: ActivatedRoute) {
    this.error = route.snapshot.data.tokenCheck;
  }

  ngOnInit() {}
}
