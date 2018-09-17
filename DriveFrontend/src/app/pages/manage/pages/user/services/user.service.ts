import { Injectable } from '@angular/core';
import { HttpClientBase } from '../../../../../services/http-client-base.service';
import { driveApi } from '../../../../../../environments/driveApi';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(public http: HttpClientBase) {}

  list() {
    return this.http.get<any[]>(driveApi.user.list, {});
  }

  create(user) {
    return this.http.post(driveApi.user.create, {}, user);
  }

  update(user) {
    return this.http.put(driveApi.user.update, {}, user, {});
  }

  delete(user) {
    return this.http.delete(driveApi.user.delete, { userId: user.id });
  }
}
