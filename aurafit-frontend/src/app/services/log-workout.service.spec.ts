import { TestBed } from '@angular/core/testing';

import { LogWorkoutService } from './log-workout.service';

describe('LogWorkoutService', () => {
  let service: LogWorkoutService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LogWorkoutService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
