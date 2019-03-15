import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MetersReportsComponent } from './meters-reports.component';

describe('MetersReportsComponent', () => {
  let component: MetersReportsComponent;
  let fixture: ComponentFixture<MetersReportsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MetersReportsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MetersReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
