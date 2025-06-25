import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileUploadComponent } from './file-upload.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: FileUploadComponent,
  },
];

@NgModule({
  declarations: [FileUploadComponent],
  imports: [CommonModule, RouterModule.forChild(routes)],
})
export class FileUploadModule {}
