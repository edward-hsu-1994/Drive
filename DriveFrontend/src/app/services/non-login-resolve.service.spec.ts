import { TestBed } from '@angular/core/testing';

import { NonLoginResolveService } from './non-login-resolve.service';

describe('NonLoginResolveService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: NonLoginResolveService = TestBed.get(NonLoginResolveService);
    expect(service).toBeTruthy();
  });
});
