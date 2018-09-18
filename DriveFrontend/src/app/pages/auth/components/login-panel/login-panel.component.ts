import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { environment } from '../../../../../environments/environment';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AppComponent } from '../../../../app.component';

@Component({
  selector: 'app-login-panel',
  templateUrl: './login-panel.component.html',
  styleUrls: ['./login-panel.component.css']
})
export class LoginPanelComponent implements OnInit {
  error: string = null;

  loginForm = new FormGroup({
    id: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    remember: new FormControl('')
  });
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private loginService: LoginService,
    private app: AppComponent
  ) {
    this.error = route.snapshot.data.tokenCheck;
  }

  ngOnInit() {}

  login() {
    this.app.loading = true;
    this.loginService
      .getToken(this.loginForm.value.id, this.loginForm.value.password)
      .subscribe(
        x => {
          this.app.loading = false;

          sessionStorage.setItem('token', x);
          sessionStorage.userId = this.getUserIdFromToken(x);
          sessionStorage.role = this.getRoleFromToken(x);

          if (this.loginForm.value.remember) {
            localStorage.setItem('token', x);
            localStorage.userId = this.getUserIdFromToken(x);
            localStorage.role = this.getRoleFromToken(x);
          }

          this.router.navigateByUrl('/manage');
        },
        error => {
          this.app.loading = false;

          this.error = '帳號或密碼錯誤';
        }
      );
    return false;
  }

  getRoleFromToken(token: string): string {
    token = token.replace('bearer ', '');

    return JSON.parse(atob(token.split('.')[1]))[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    ];
  }

  getUserIdFromToken(token: string): string {
    token = token.replace('bearer ', '');

    return JSON.parse(atob(token.split('.')[1]))[
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
    ];
  }
}
