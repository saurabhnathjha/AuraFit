import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuraChatComponent } from './aura-chat.component';

describe('AuraChatComponent', () => {
  let component: AuraChatComponent;
  let fixture: ComponentFixture<AuraChatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuraChatComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuraChatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
