import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import {
  ImageListResponse,
  MultipleUploadResponse,
  UploadResponse,
} from '../models/FileUpload';
import {
  HttpRequest,
  HttpEvent,
  HttpEventType,
  HttpClient,
} from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class FileUploadServService {
  private apiUrl = 'http://localhost:5288/api/FileUpload';
  private uploadProgressSubject = new BehaviorSubject<number>(0);
  public uploadProgress$ = this.uploadProgressSubject.asObservable();

  constructor(private http: HttpClient) {}

  uploadSingleImage(file: File): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file);

    const req = new HttpRequest(
      'POST',
      `${this.apiUrl}/upload-image`,
      formData,
      {
        reportProgress: true,
      }
    );

    return this.http.request<UploadResponse>(req).pipe(
      map((event: HttpEvent<UploadResponse>) => {
        if (event.type === HttpEventType.UploadProgress) {
          const progress = Math.round(
            (100 * event.loaded) / (event.total || 1)
          );
          this.uploadProgressSubject.next(progress);
        } else if (event.type === HttpEventType.Response) {
          this.uploadProgressSubject.next(0);
          return event.body!;
        }
        return {} as UploadResponse;
      })
    );
  }

  uploadMultipleImages(files: File[]): Observable<MultipleUploadResponse> {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append('files', file);
    });

    return this.http.post<MultipleUploadResponse>(
      `${this.apiUrl}/upload-multiple`,
      formData
    );
  }

  getUploadedImages(): Observable<ImageListResponse> {
    return this.http.get<ImageListResponse>(`${this.apiUrl}/list`);
  }

  checkFile(fileName: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/check-file/${fileName}`);
  }

  getImageUrl(fileName: string): string {
    return `http://localhost:5288/assets/images/${fileName}`;
  }
}
