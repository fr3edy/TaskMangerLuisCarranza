export type TaskStatus = 'Pending' | 'Completed';

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  dueDate: string;
  userId: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
}