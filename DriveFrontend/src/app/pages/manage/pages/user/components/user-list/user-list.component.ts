import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  userlist: any[];
  constructor(private route: ActivatedRoute) {
    this.userlist = this.route.snapshot.data['userlist'];
  }

  ngOnInit() {}
}
