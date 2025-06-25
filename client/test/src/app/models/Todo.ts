export interface ITodoModel {
  id: number;
  title: string;
  text: string;
  status: number; // 0 = pending, 1 = completed
  createdAt: string;
  updatedAt?: string | null;
  dueDate?: string | null;
  isImportant: boolean;
  completedAt?: string | null;
  isDeleted: boolean;
  appUserId?: string | null;
}

export type TabType = 'all' | 'pending' | 'completed';
export type SortType = 'created' | 'dueDate' | 'priority' | 'updated';
