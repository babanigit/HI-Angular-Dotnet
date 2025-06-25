import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoggedInUserComponent } from './logged-in-user.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: LoggedInUserComponent,
  },
];

@NgModule({
  declarations: [LoggedInUserComponent],
  imports: [CommonModule, RouterModule.forChild(routes)],
})
export class LoggedInUserModule {}
