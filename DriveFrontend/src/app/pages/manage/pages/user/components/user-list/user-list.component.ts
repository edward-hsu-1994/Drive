import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  userlist: any[];
  userForm = new FormGroup({
    id: new FormControl('', Validators.required),
    password: new FormControl(''),
    isAdmin: new FormControl(false)
  });
  showUserDialog;
  showDeleteDialog;
  deleteTarget;

  isUpdate = false;
  constructor(private route: ActivatedRoute, private userService: UserService) {
    this.userlist = this.route.snapshot.data['userlist'];
  }

  ngOnInit() {}

  load() {
    this.userService.list().subscribe(x => {
      this.userlist = x;
    });
  }

  createUserAction() {
    this.userForm.reset();
    this.isUpdate = false;
    this.showUserDialog = true;
  }
  createUser(user) {
    if (!user.isAdmin) {
      user.isAdmin = false;
    }
    if (this.isUpdate) {
      this.userService.update(user).subscribe(x => {
        this.load();
      });
    } else {
      this.userService.create(user).subscribe(x => {
        this.load();
      });
    }
    this.showUserDialog = false;
  }

  updateUserAction(user) {
    this.isUpdate = true;
    this.userForm.setValue(user);
    this.showUserDialog = true;
  }

  deleteUserAction(user) {
    this.deleteTarget = user;
    this.showDeleteDialog = true;
  }
  deleteUser() {
    this.showDeleteDialog = false;
    this.userService.delete(this.deleteTarget).subscribe(x => {
      this.load();
    });
  }
}
