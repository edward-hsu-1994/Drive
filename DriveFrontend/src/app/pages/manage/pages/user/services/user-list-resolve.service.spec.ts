import { TestBed } from '@angular/core/testing';

import { UserListResolveService } from './user-list-resolve.service';

describe('UserListResolveService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserListResolveService = TestBed.get(UserListResolveService);
    expect(service).toBeTruthy();
  });
});
