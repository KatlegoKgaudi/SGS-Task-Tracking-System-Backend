export interface User {
  id: number;
  username: string;
  email: string;
  role: number;
  createdAt: string;
}

export interface UserCreateDto {
  username: string;
  email: string;
  password: string;
  role: number;
}

export interface UserResponseDto {
  id: number;
  username: string;
  email: string;
  role: number;
  createdAt: string;
}