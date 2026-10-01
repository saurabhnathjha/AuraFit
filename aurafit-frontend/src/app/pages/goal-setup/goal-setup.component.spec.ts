import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoalSetupComponent } from './goal-setup.component';

describe('GoalSetupComponent', () => {
  let component: GoalSetupComponent;
  let fixture: ComponentFixture<GoalSetupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoalSetupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GoalSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
