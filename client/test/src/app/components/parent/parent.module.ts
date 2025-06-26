import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ParentComponent } from './parent.component';
import { ChildModule } from '../child/child.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [ParentComponent],
  imports: [CommonModule, ChildModule, FormsModule],
  exports: [ParentComponent],
})
export class ParentModule {}
