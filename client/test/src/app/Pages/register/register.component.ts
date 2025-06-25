import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerData = {
    username: '',
    email: '',
    password: '',
  };

  showPassword: boolean = false;
  identityErrors: string[] = [];

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  registerUser() {
    console.log('resiter hitteddd hello');
    const { username, email, password } = this.registerData;

    this.authService.register({ username, email, password }).subscribe({
      next: () => {
        console.log('Success response');

        this.toastr.success('Registered successfully');
        this.router.navigate(['/login']);
      },
      error: (err) => {

        // storing the error 
        if (Array.isArray(err.error)) {
          this.identityErrors = err.error.map((e: any) => e.description);
        } else {
          this.identityErrors = ['Something went wrong'];
        }

        // console login and toastr
        console.error('Registration failed', err);
        this.toastr.error('Registration failed');
      },
    });
  }
}
