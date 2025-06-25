import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ILoggedIn_User } from 'src/app/models/User';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent implements OnInit {
  constructor(
    private authServe: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  Logged_in_user_Data: ILoggedIn_User | undefined;
  
  ngOnInit(): void {
    // this.get_user();
  }

  get_user() {
    this.authServe.loggedIn_user().subscribe({
      next: (res) => {
        this.Logged_in_user_Data = res;
        console.log('the logged in user is:- ', res.data);
      },
      error: (err) => {
        console.error('logged in user failed', err);
        // this.toastr.error(err.error, 'logged in user Failed');
      },
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    console.log('Token removed from localStorage');
    this.toastr.success('logged out ');
    this.router.navigate(['/login']); // Redirect to login if no token
    // Optionally navigate to login
  }
}
