import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeterReadsComponent } from './meter-reads.component';

describe('MeterReadsComponent', () => {
  let component: MeterReadsComponent;
  let fixture: ComponentFixture<MeterReadsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeterReadsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeterReadsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
