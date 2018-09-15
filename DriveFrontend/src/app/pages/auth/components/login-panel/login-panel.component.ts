import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { environment } from '../../../../../environments/environment';
import { FormGroup, FormControl, Validators } from '@angular/forms';

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
    private loginService: LoginService
  ) {
    this.error = route.snapshot.data.tokenCheck;
  }

  ngOnInit() {}

  login() {
    console.log(this.loginForm);
    this.loginService
      .getToken(this.loginForm.value.id, this.loginForm.value.password)
      .subscribe(x => {
        environment.token = x;
        if (this.loginForm.value.remember) {
          localStorage.setItem('token', <string>x);
        }

        this.router.navigateByUrl('/manage');
      });
    return false;
  }
}
