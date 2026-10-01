import { TestBed } from '@angular/core/testing';

import { ExerciseCatalogService } from './exercise-catalog.service';

describe('ExerciseCatalogService', () => {
  let service: ExerciseCatalogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExerciseCatalogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
