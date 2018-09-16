import { Component, OnInit } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({
    query: new FormControl('', Validators.required)
  });
  constructor(private router: Router) {}

  ngOnInit() {}

  search($event) {
    console.log($event);
    // alert();
  }
  getUserId() {
    return sessionStorage.userId;
  }
  logout() {
    sessionStorage.clear();
    localStorage.clear();

    this.router.navigateByUrl('/');
  }
}
