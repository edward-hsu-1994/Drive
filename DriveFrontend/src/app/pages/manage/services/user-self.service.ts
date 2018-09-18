import { Injectable } from '@angular/core';
import { HttpClientBase } from '../../../services/http-client-base.service';
import { driveApi } from '../../../../environments/driveApi';

@Injectable({
  providedIn: 'root'
})
export class UserSelfService {
  constructor(public http: HttpClientBase) {}

  changePassword(newPassword: string) {
    return this.http.put(
      driveApi.user.changePassword,
      {},
      JSON.stringify(newPassword),
      { 'Content-Type': 'application/json' }
    );
  }
}
