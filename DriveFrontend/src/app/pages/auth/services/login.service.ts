import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(private http: HttpClient) {}

  public getToken(id: string, password: string): Observable<string> {
    return this.http.post<string>('api/User/token', {
      id: id,
      password: password
    });
  }
}
