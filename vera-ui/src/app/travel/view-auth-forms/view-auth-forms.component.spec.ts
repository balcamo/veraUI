import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAuthFormsComponent } from './view-auth-forms.component';

describe('ViewAuthFormsComponent', () => {
  let component: ViewAuthFormsComponent;
  let fixture: ComponentFixture<ViewAuthFormsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewAuthFormsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAuthFormsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
