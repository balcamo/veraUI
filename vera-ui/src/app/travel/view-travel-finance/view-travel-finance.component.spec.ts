import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewTravelFinanceComponent } from './view-travel-finance.component';

describe('ViewTravelFinanceComponent', () => {
  let component: ViewTravelFinanceComponent;
  let fixture: ComponentFixture<ViewTravelFinanceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewTravelFinanceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewTravelFinanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
