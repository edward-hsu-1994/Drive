import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(private http: HttpClient) {}

  public getToken(id: string, password: string) {
    return this.http.post('api/User/token', {
      id: id,
      password: password
    });
  }
}
