import { TestBed } from '@angular/core/testing';

import { MergehuService } from './mergehu.service';

describe('MergehuService', () => {
  let service: MergehuService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MergehuService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
