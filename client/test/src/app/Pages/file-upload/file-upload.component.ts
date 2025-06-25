import { Component, OnInit } from '@angular/core';
import { ImageFile, UploadResponse } from 'src/app/models/FileUpload';
import { FileUploadServService } from 'src/app/services/file-upload-serv.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
})
export class FileUploadComponent implements OnInit {
  selectedFile: File | null = null;
  selectedFiles: File[] = [];
  uploading = false;
  uploadingMultiple = false;
  uploadProgress = 0;
  uploadResults: any[] = [];
  uploadedImages: ImageFile[] = [];

  constructor(private fileUploadService: FileUploadServService) {}

  ngOnInit() {
    this.loadImages();
    this.fileUploadService.uploadProgress$.subscribe(
      (progress) => (this.uploadProgress = progress)
    );
  }

  onSingleFileSelected(event: any) {
    const file = event.target.files[0];
    if (file && this.isValidImageFile(file)) {
      this.selectedFile = file;
    }
  }

  onMultipleFilesSelected(event: any) {
    const files = Array.from(event.target.files) as File[];
    this.selectedFiles = files.filter((file) => this.isValidImageFile(file));
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    (event.target as HTMLElement).classList.add('drag-over');
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    (event.target as HTMLElement).classList.remove('drag-over');
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    (event.target as HTMLElement).classList.remove('drag-over');

    const files = Array.from(event.dataTransfer?.files || []) as File[];
    if (files.length > 0) {
      const imageFile = files.find((file) => this.isValidImageFile(file));
      if (imageFile) {
        this.selectedFile = imageFile;
      }
    }
  }

  uploadSingleFile() {
    if (!this.selectedFile) return;

    this.uploading = true;
    this.fileUploadService.uploadSingleImage(this.selectedFile).subscribe({
      next: (response: UploadResponse) => {
        if (response.message) {
          console.log('Upload successful:', response);
          this.uploadResults = [
            {
              originalFileName: response.originalFileName,
              success: true,
            },
          ];
          this.selectedFile = null;
          this.loadImages();
        }
      },
      error: (error) => {
        console.error('Upload failed:', error);
        this.uploadResults = [
          {
            originalFileName: this.selectedFile?.name,
            success: false,
            error: error.error?.message || 'Upload failed',
          },
        ];
      },
      complete: () => {
        this.uploading = false;
      },
    });
  }

  uploadMultipleFiles() {
    if (this.selectedFiles.length === 0) return;

    this.uploadingMultiple = true;
    this.fileUploadService.uploadMultipleImages(this.selectedFiles).subscribe({
      next: (response) => {
        console.log('Multiple upload response:', response);
        this.uploadResults = response.results;
        this.selectedFiles = [];
        this.loadImages();
      },
      error: (error) => {
        console.error('Multiple upload failed:', error);
        this.uploadResults = this.selectedFiles.map((file) => ({
          originalFileName: file.name,
          success: false,
          error: 'Upload failed',
        }));
      },
      complete: () => {
        this.uploadingMultiple = false;
      },
    });
  }

  removeFile(fileToRemove: File) {
    this.selectedFiles = this.selectedFiles.filter(
      (file) => file !== fileToRemove
    );
  }

  loadImages() {
    this.fileUploadService.getUploadedImages().subscribe({
      next: (response) => {
        this.uploadedImages = response.images;
      },
      error: (error) => {
        console.error('Failed to load images:', error);
      },
    });
  }

  getImageUrl(fileName: string): string {
    return this.fileUploadService.getImageUrl(fileName);
  }

  onImageError(event: any, image: ImageFile) {
    console.error('Failed to load image:', image.fileName);
    // You could set a placeholder image here
    event.target.src =
      'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCIgZm9udC1zaXplPSIxNCIgZmlsbD0iIzk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkltYWdlIE5vdCBGb3VuZDwvdGV4dD48L3N2Zz4=';
  }

  copyImageUrl(image: ImageFile) {
    const url = this.getImageUrl(image.fileName);
    navigator.clipboard.writeText(url).then(() => {
      alert('Image URL copied to clipboard!');
    });
  }

  private isValidImageFile(file: File): boolean {
    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'image/gif',
      'image/bmp',
    ];
    return allowedTypes.includes(file.type);
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }
}
