import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';
// import { GetJsonServerService } from 'src/app/services/get-json-server.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginData = {
    username: '',
    password: '',
  };

  showPassword: boolean = false;
  identityErrors: string[] = [];
  constructor(
    // private getjsonserv: GetJsonServerService,
    private authServ: AuthService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  loginUser() {
    const { username, password } = this.loginData;

    this.authServ.login({ username, password }).subscribe({
      next: () => this.router.navigate(['/todo']),
      error: (err) => {
        // storing the error
        if (Array.isArray(err.error)) {
          this.identityErrors = err.error.map((e: any) => e.description);
        } else {
          this.identityErrors = ['Something went wrong'];
        }

        console.error('Login failed', err);
        this.toastr.error(err.error, 'Login Failed');
      },
    });
  }
}
