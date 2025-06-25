import { TestBed } from '@angular/core/testing';

import { GetTodoService } from './get-todo.service';

describe('GetTodoService', () => {
  let service: GetTodoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GetTodoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
