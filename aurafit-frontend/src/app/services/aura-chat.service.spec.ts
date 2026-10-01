import { TestBed } from '@angular/core/testing';

import { AuraChatService } from './aura-chat.service';

describe('AuraChatService', () => {
  let service: AuraChatService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuraChatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
