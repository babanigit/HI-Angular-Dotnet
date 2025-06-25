import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, delay, finalize, Observable } from 'rxjs';
import { ITodoModel } from '../models/Todo';
// import { ITodoModel } from '../models/Todo';

@Injectable({
  providedIn: 'root',
})
export class GetTodoService {
  // private baseUrl = '/api/account'; // for coupled
  private baseUrl = 'http://localhost:5288/api/todo'; //for decoupled

  constructor(private http: HttpClient) {}

  getTodos(): Observable<ITodoModel[]> {
    return this.http.get<ITodoModel[]>(this.baseUrl).pipe(
      // delay(3000),
      catchError((err) => {
        console.log('gettodos serv  err :-  , ', err);
        throw err;
      }),
      finalize(() => {
        console.log('finished');
      })
    );
  }

  addTodo(todo: {
    title: string;
    text: string;
    isImportant: boolean;
    dueDate: string | null;
  }): Observable<ITodoModel> {
    return this.http.post<ITodoModel>(this.baseUrl, todo).pipe(
      // delay(2000),
      catchError((err) => {
        console.log('addtodo serv  err :-  , ', err);
        throw err;
      }),
      finalize(() => {
        console.log('finished');
      })
    );
  }

  updateTodo(updatedTodo: ITodoModel): Observable<ITodoModel> {
    return this.http
      .put<ITodoModel>(`${this.baseUrl}/${updatedTodo.id}`, updatedTodo)
      .pipe(
        // delay(2000),
        catchError((err) => {
          console.log('update todo serv  err :-  , ', err);
          throw err;
        }),
        finalize(() => {
          console.log('finished');
        })
      );
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
      // delay(2000),
      finalize(() => {
        console.log('finished');
      })
    );
  }
}
