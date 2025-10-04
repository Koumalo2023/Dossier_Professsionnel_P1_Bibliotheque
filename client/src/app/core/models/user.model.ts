export interface User {
  id: string;
  name: string;
  email: string;
  roles: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface UserDto {
  id: string;
  name: string;
  email: string;
  role: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateUserDto {
  name: string;
  email: string;
}

export interface RegisterDto {
  name: string;
  email: string;
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: UserDto;
  expiresIn?: number;
}

export interface RefreshTokenRequest {
  token: string;
}

export interface CurrentUser {
  id: string;
  name: string;
  email: string;
  roles: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface UserInfo {
  isLoggedIn: boolean;
  username?: string;
  isAdmin?: boolean;
}