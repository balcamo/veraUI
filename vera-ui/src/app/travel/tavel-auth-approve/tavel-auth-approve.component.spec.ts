import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TavelAuthApproveComponent } from './tavel-auth-approve.component';

describe('TavelAuthApproveComponent', () => {
  let component: TavelAuthApproveComponent;
  let fixture: ComponentFixture<TavelAuthApproveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TavelAuthApproveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TavelAuthApproveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
