import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './util/auth.guard';

const routes: Routes = [
  {
  path: 'home',
    loadChildren: () =>
      import('./Pages/home/home.module').then(
        (m) => m.HomeModule
      ),
    // canActivate: [AuthGuard],

  },
  {
    path: 'file-uploader',
    loadChildren: () =>
      import('./Pages/file-upload/file-upload.module').then(
        (m) => m.FileUploadModule
      ),
    canActivate: [AuthGuard],
  },
  {
    path: 'logged-in-user',
    loadChildren: () =>
      import('./Pages/logged-in-user/logged-in-user.module').then(
        (m) => m.LoggedInUserModule
      ),
    canActivate: [AuthGuard],
  },
  {
    path: 'todo',
    loadChildren: () =>
      import('./Pages/todo/todo.module').then((m) => m.TodoModule),
    canActivate: [AuthGuard], // 👈 Protecting route
  },
  {
    path: 'login',
    loadChildren: () =>
      import('./Pages/login/login.module').then((m) => m.LoginModule),
  },
  {
    path: 'register',
    loadChildren: () =>
      import('./Pages/register/register.module').then((m) => m.RegisterModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
