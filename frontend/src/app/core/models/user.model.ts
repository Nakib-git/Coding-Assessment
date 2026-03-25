export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateUserDto {
  name: string;
  email: string;
  role: string;
  isActive: boolean;
}

export interface UpdateUserDto {
  name: string;
  email: string;
  role: string;
  isActive: boolean;
}
