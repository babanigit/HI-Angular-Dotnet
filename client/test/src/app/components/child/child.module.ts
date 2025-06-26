import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChildComponent } from './child.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [ChildComponent],
  imports: [CommonModule, FormsModule],
  exports: [ChildComponent],
})
export class ChildModule {}
