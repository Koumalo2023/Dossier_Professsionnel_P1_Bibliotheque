import { Injectable } from "@angular/core";
import { User } from "../models/user.model";


// services/auth-converter.service.ts
@Injectable({ providedIn: 'root' })
export class AuthConverterService {
  constructor() {}

  fromDto(userDto: any): User {
    return {
      id: userDto.id,
      name: userDto.name,
      email: userDto.email,
      roles: userDto.roles || [], // Garantit un tableau
      createdAt: new Date(userDto.createdAt),
      updatedAt: new Date(userDto.updatedAt)
    };
  }

  toDto(user: Partial<User>): any {
    return {
      id: user.id,
      name: user.name,
      email: user.email,
      roles: user.roles?.join(',') || '', // Convertit en string pour l'API
      createdAt: user.createdAt?.toISOString(),
      updatedAt: user.updatedAt?.toISOString()
    };
  }
}