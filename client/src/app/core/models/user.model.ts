

export interface User {
    id: string; 
    name: string;
    email: string;
    role: string;
    createdAt: Date;
    updatedAt: Date;
  }
  
  // DTO pour la mise à jour du profil utilisateur
  export interface UpdateUserDto {
    name: string;
    email: string;
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
  export interface AuthResponse {
    token: string;
  }