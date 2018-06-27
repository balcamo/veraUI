import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelAuthComponent } from './travel-auth.component';

describe('TravelAuthComponent', () => {
  let component: TravelAuthComponent;
  let fixture: ComponentFixture<TravelAuthComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TravelAuthComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TravelAuthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
