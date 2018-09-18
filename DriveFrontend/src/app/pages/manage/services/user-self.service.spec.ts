import { TestBed } from '@angular/core/testing';

import { UserSelfService } from './user-self.service';

describe('UserSelfService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserSelfService = TestBed.get(UserSelfService);
    expect(service).toBeTruthy();
  });
});
