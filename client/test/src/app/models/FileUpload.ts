export interface UploadResponse {
  message: string;
  fileName: string;
  originalFileName: string;
  fileSize: number;
  fileUrl: string;
  uploadedAt: string;
}

export interface MultipleUploadResponse {
  message: string;
  results: UploadResult[];
  totalFiles: number;
  successfulUploads: number;
}

export interface UploadResult {
  originalFileName: string;
  fileName?: string;
  fileSize?: number;
  fileUrl?: string;
  success: boolean;
  error?: string;
}

export interface ImageFile {
  fileName: string;
  fileUrl: string;
  fileSize: number;
  createdAt: string;
}

export interface ImageListResponse {
  images: ImageFile[];
}
