import { Component, OnInit } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UriBuilder } from 'uribuilder';
import { UserSelfService } from '../../services/user-self.service';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({
    query: new FormControl('', Validators.required)
  });
  userForm = new FormGroup({
    password: new FormControl('', Validators.required)
  });

  showChangePasswordDialog = false;

  get isAdmin() {
    return (sessionStorage.role || localStorage.role) === 'Administrator';
  }

  constructor(
    private router: Router,
    private userSelfService: UserSelfService
  ) {}

  ngOnInit() {}

  search() {
    if (!this.searchForm.valid) {
      return;
    }
    if (this.router.url.indexOf('/manage/file') === -1) {
      this.router.navigateByUrl(
        '/manage/file/?q=' + encodeURIComponent(this.searchForm.value.query)
      );
    } else {
      const targetUrl = UriBuilder.updateQuery(this.router.url.substring(1), {
        q: this.searchForm.value.query
      });
      this.router.navigateByUrl('/' + targetUrl);
    }
  }
  getUserId() {
    return sessionStorage.userId;
  }
  logout() {
    sessionStorage.clear();
    localStorage.clear();

    this.router.navigateByUrl('/');
  }

  changePasswordAction() {
    this.showChangePasswordDialog = true;
  }

  changePassword(newPassword: string) {
    this.userSelfService.changePassword(newPassword).subscribe(x => {
      this.showChangePasswordDialog = false;
    });
  }
}
