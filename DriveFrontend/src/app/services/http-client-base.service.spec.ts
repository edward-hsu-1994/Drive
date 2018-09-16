import { TestBed } from '@angular/core/testing';

import { HttpClientBase } from './http-client-base.service';

describe('HttpClientBaseService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HttpClientBase = TestBed.get(HttpClientBase);
    expect(service).toBeTruthy();
  });
});
