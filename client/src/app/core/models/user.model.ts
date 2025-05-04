

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
  role: string; // Role au singulier si c'est le cas dans l'API
  createdAt: string;
  updatedAt: string;
}

// DTO pour la mise à jour du profil utilisateur
export interface UpdateUserDto {
  name: string;
  email: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  tokenExpires: string;
  refreshTokenExpires: string;
  user?: UserDto;
}

export interface RefreshTokenResponse {
  token: string;
  refreshToken: string;
  tokenExpires: string;
}

// DTO pour l'enregistrement d'un nouvel utilisateur
export interface RegisterDto {
  name: string;
  email: string;
  password: string;
}

// DTO pour la connexion d'un utilisateur existant
export interface LoginDto {
  email: string;
  password: string;
}

// Réponse après une connexion réussie (contenant le token JWT)
export interface AuthResponseDto {
  token: string;
  refreshToken: string;
  tokenExpires: string;
  refreshTokenExpires: string;
  user: UserDto;
}

// Pour usage interne Angular
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

export interface TokenResponse {
  token: string;
  refreshToken: string;
  tokenExpires: string;
  refreshTokenExpires: string;
  user: UserDto;
}