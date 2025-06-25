import { TestBed } from '@angular/core/testing';

import { FileUploadServService } from './file-upload-serv.service';

describe('FileUploadServService', () => {
  let service: FileUploadServService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FileUploadServService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
