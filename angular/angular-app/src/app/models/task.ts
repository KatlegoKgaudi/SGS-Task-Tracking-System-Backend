export interface Task {
  id: number;
  title: string;
  description: string;
  status: number;
  dueDate: string;
  createdDate: string;
  assignedUserId: number;
  assignedUser?: string;
}

export interface TaskCreateDto {
  title: string;
  description: string;
  dueDate: string;
  assignedUserId?: number;
}

export interface TaskUpdateDto {
  title?: string;
  description?: string;
  status?: number;
  dueDate?: string;
  assignedUserId?: number;
}