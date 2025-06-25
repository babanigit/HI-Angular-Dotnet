import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize, Observable, tap } from 'rxjs';
import { ILoggedIn_User } from '../models/User';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // private baseUrl = 'https://localhost:3000/api/auth';

  // private baseUrl = '/api/account'; // for coupled
  private baseUrl = 'http://localhost:5288/api/account'; //for decoupled

  constructor(private http: HttpClient) {}

  loggedIn_user(): Observable<ILoggedIn_User> {
    return this.http.get<ILoggedIn_User>(`${this.baseUrl}/user`).pipe(
      tap((res) => {
        console.log('Logged-in-user hitted , ', res);
      }),
      catchError((err) => {
        console.log('Logged-in-user serv  err :-  , ', err);
        throw err;
      }),
      finalize(() => {
        console.log('Logged-in-user finished');
      })
    );
  }

  login(credentials: {
    username: string;
    password: string;
  }): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/login`, credentials, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          console.log('login hitted , ', res);
          localStorage.setItem('token', res.token); // store token
        }),
        catchError((err) => {
          console.log('login serv  err :-  , ', err);
          throw err;
        }),
        finalize(() => {
          console.log('login finished');
        })
      );
  }

  register(data: {
    username: string;
    email: string;
    password: string;
  }): Observable<any> {
    return this.http
      .post(`${this.baseUrl}/register`, data, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.log('register serv  err :-  , ', err);
          throw err;
        }),
        finalize(() => {
          console.log('reister finished');
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
}
