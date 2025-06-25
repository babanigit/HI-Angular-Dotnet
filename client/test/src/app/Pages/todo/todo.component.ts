import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ITodoModel, SortType, TabType } from 'src/app/models/Todo';
import { GetTodoService } from 'src/app/services/get-todo.service';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css'],
})
export class TodoComponent implements OnInit {
  todos: ITodoModel[] = [];

  newTodoTitle = '';
  newTodoDesc = '';
  isImportant = false;
  dueDate = '';
  showAddForm = false;
  editingTodo: ITodoModel | null = null;
  loading = false;
  error: string | null = null;

  activeTab: TabType = 'all';
  sortBy: SortType = 'created';
  sortDirection: 'asc' | 'desc' = 'desc';

  tabs = [
    { key: 'all' as TabType, label: 'All' },
    { key: 'pending' as TabType, label: 'Pending' },
    { key: 'completed' as TabType, label: 'Completed' },
  ];

  sortOptions = [
    { key: 'created' as SortType, label: 'Created', icon: 'fas fa-clock' },
    { key: 'updated' as SortType, label: 'Updated', icon: 'fas fa-sync' },
    { key: 'dueDate' as SortType, label: 'Due Date', icon: 'fas fa-calendar' },
    { key: 'priority' as SortType, label: 'Priority', icon: 'fas fa-flag' },
  ];

  constructor(private todoService: GetTodoService) {}

  ngOnInit() {
    this.loadTodos();
  }

  // to load the file
  loadTodos() {
    this.loading = true;
    this.error = null;

    this.todoService.getTodos().subscribe({
      next: (todos) => {
        this.todos = todos;
        this.loading = false;

        console.log('the Loaded Todo Data is :-  ', todos);
      },
      error: (error) => {
        console.error('Error loading todos:', error);
        this.error = 'Failed to load todos. Please try again.';
        this.loading = false;
      },
    });
  }

  // add todo
  addTodo() {
    if (!this.newTodoTitle.trim()) return;

    this.loading = true;
    this.error = null;

    // Create the todo object to send to API
    const todoToAdd = {
      title: this.newTodoTitle.trim(),
      text: this.newTodoDesc.trim(),
      isImportant: this.isImportant,
      dueDate: this.dueDate || null,
    };

    this.todoService
      .addTodo({
        title: todoToAdd.title,
        text: todoToAdd.text,
        isImportant: todoToAdd.isImportant,
        dueDate: todoToAdd.dueDate,
      })
      .subscribe({
        next: (newTodo) => {
          // If your API doesn't accept all fields in POST, you might need to update it after creation
          if (todoToAdd.text || todoToAdd.isImportant || todoToAdd.dueDate) {
            const updatedTodo = {
              ...newTodo,
              text: todoToAdd.text,
              isImportant: todoToAdd.isImportant,
              dueDate: todoToAdd.dueDate,
            };

            this.todoService.updateTodo(updatedTodo).subscribe({
              next: (finalTodo) => {
                this.todos.push(finalTodo);
                this.resetForm();
                this.loading = false;
              },
              error: (error) => {
                console.error('Error updating new todo:', error);
                // Still add the basic todo even if update fails
                this.todos.push(newTodo);
                this.resetForm();
                this.loading = false;
              },
            });
          } else {
            this.todos.push(newTodo);
            this.resetForm();
            this.loading = false;
          }
        },
        error: (error) => {
          console.error('Error adding todo:', error);
          this.error = 'Failed to add todo. Please try again.';
          this.loading = false;
        },
      });
  }

  editTodo(todo: ITodoModel) {
    this.editingTodo = { ...todo };
    this.newTodoTitle = todo.title;
    this.newTodoDesc = todo.text;
    this.isImportant = todo.isImportant;
    this.dueDate = todo.dueDate ? todo.dueDate.split('T')[0] : '';
    this.showAddForm = true;
  }

  updateTodo() {
    if (!this.editingTodo || !this.newTodoTitle.trim()) return;

    this.loading = true;
    this.error = null;

    const updatedTodo: ITodoModel = {
      ...this.editingTodo,
      title: this.newTodoTitle.trim(),
      text: this.newTodoDesc.trim(),
      isImportant: this.isImportant,
      dueDate: this.dueDate || null,
      updatedAt: new Date().toISOString(),
    };

    this.todoService.updateTodo(updatedTodo).subscribe({
      next: (updated) => {
        const index = this.todos.findIndex((t) => t.id === updated.id);
        if (index !== -1) {
          this.todos[index] = updated;
        }
        this.resetForm();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error updating todo:', error);
        this.error = 'Failed to update todo. Please try again.';
        this.loading = false;
      },
    });
  }

  deleteTodoBtn(id: number) {
    if (!confirm('Are you sure you want to delete this task?')) return;

    this.loading = true;
    this.error = null;

    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        // this.todos = this.todos.filter((todo) => todo.id !== id);
        this.loading = false;
        this.loadTodos();
      },
      error: (error) => {
        // console.error('Error deleting todo:', error);
        // this.error = 'Failed to delete todo. Please try again.';
        console.log(' bab :- ', error);
        this.loading = false;
        this.loadTodos();
      },
    });
  }

  // fixed
  toggleTodo(todo: ITodoModel) {
    const newStatus = todo.status === 1 ? 0 : 1; // Calculate new status first

    const updatedTodo: ITodoModel = {
      ...todo,
      status: newStatus,
      completedAt: newStatus === 1 ? new Date().toISOString() : null,
      updatedAt: new Date().toISOString(),
    };

    console.log('the before update status is:- ', updatedTodo);

    this.todoService.updateTodo(updatedTodo).subscribe({
      next: (updated) => {
        console.log('the updated todo is:- ', updated);
        this.loadTodos();
      },
      error: (error) => {
        console.error('Error toggling todo:', error);
        this.error = 'Failed to update todo status. Please try again.';
      },
    });
  }

  setActiveTab(tab: TabType) {
    this.activeTab = tab;
  }

  setSortBy(sort: SortType) {
    if (this.sortBy === sort) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = sort;
      this.sortDirection = 'asc';
    }
  }

  getFilteredTodos(): ITodoModel[] {
    return this.todos.filter((todo) => {
      switch (this.activeTab) {
        case 'pending':
          return todo.status === 0;
        case 'completed':
          return todo.status === 1;
        default:
          return true;
      }
    });
  }

  getSortedTodos(): ITodoModel[] {
    const filtered = this.getFilteredTodos();
    return [...filtered].sort((a, b) => {
      const multiplier = this.sortDirection === 'asc' ? 1 : -1;

      switch (this.sortBy) {
        case 'created':
          return (
            (new Date(a.createdAt).getTime() -
              new Date(b.createdAt).getTime()) *
            multiplier
          );
        case 'updated':
          const aUpdated = a.updatedAt || a.createdAt;
          const bUpdated = b.updatedAt || b.createdAt;
          return (
            (new Date(aUpdated).getTime() - new Date(bUpdated).getTime()) *
            multiplier
          );
        case 'dueDate':
          if (!a.dueDate && !b.dueDate) return 0;
          if (!a.dueDate) return 1;
          if (!b.dueDate) return -1;
          return (
            (new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime()) *
            multiplier
          );
        case 'priority':
          const aPriority = a.isImportant ? 1 : 0;
          const bPriority = b.isImportant ? 1 : 0;
          return (bPriority - aPriority) * multiplier;
        default:
          return 0;
      }
    });
  }

  getFilteredCount(tab: TabType): number {
    switch (tab) {
      case 'pending':
        return this.todos.filter((t) => t.status === 0).length;
      case 'completed':
        return this.todos.filter((t) => t.status === 1).length;
      default:
        return this.todos.length;
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  resetForm() {
    this.newTodoTitle = '';
    this.newTodoDesc = '';
    this.isImportant = false;
    this.dueDate = '';
    this.showAddForm = false;
    this.editingTodo = null;
  }

  cancelEdit() {
    this.resetForm();
  }
}
