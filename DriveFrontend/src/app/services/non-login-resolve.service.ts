import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NonLoginResolve {
  constructor(private router: Router) {}

  resolve() {
    if (sessionStorage.token || localStorage.token) {
      return;
    }
    this.router.navigateByUrl('/');
  }
}
