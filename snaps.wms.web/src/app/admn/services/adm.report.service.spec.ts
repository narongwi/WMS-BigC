import { TestBed } from '@angular/core/testing';

import { Adm.ReportService } from './adm.report.service';

describe('Adm.ReportService', () => {
  let service: Adm.ReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Adm.ReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
