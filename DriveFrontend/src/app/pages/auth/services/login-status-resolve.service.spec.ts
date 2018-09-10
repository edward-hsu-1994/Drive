import { TestBed } from '@angular/core/testing';

import { LoginStatusResolveService } from './login-status-resolve.service';

describe('LoginStatusResolveService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LoginStatusResolveService = TestBed.get(LoginStatusResolveService);
    expect(service).toBeTruthy();
  });
});
