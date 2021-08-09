import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgaComponent } from './orga.component';

describe('OrgaComponent', () => {
  let component: OrgaComponent;
  let fixture: ComponentFixture<OrgaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrgaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrgaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
