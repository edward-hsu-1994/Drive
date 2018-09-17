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
}
