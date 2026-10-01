import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MotivationalBannerComponent } from './motivational-banner.component';

describe('MotivationalBannerComponent', () => {
  let component: MotivationalBannerComponent;
  let fixture: ComponentFixture<MotivationalBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MotivationalBannerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MotivationalBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
