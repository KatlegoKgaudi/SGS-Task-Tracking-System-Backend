import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import { HttpClient } from '@angular/common/http';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message?: string;
  token?: string;
  user?: {
    id: number;
    username: string;
    email?: string;
    role: number;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:44368/api';
  private tokenKey = 'auth_token';
  private userKey = 'current_user';
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private apiConfig: ApiConfigService, private http: HttpClient) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage() {
    const userStr = localStorage.getItem(this.userKey);
    if (userStr) {
      try {
        const u = JSON.parse(userStr);
        this.currentUserSubject.next(u);
      } catch {
        localStorage.removeItem(this.userKey);
      }
    }
  }

  public getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  public setToken(token: string | null) {
    if (token) localStorage.setItem(this.tokenKey, token);
    else localStorage.removeItem(this.tokenKey);
  }

  public isLoggedIn(): boolean {
    return !!this.getToken();
  }

  public async login(request: LoginRequest): Promise<AuthResponse> {
    const baseUrl = await this.apiConfig.getApiUrl();
    const url = `${baseUrl}/api/auth/login`;

    try {
      const resp = await firstValueFrom(this.http.post<AuthResponse>(url, request));
      if (resp && resp.success && resp.token) {
        this.setToken(resp.token);
        if (resp.user) {
          localStorage.setItem(this.userKey, JSON.stringify(resp.user));
          this.currentUserSubject.next(resp.user);
        }
      }
      return resp ?? { success: false, message: 'Empty response from server.' };
    } catch (err) {
      // fallback port switch between 8080 and IIS (5106)
      const fallback = baseUrl.includes(':8080')
        ? baseUrl.replace(':8080', ':5106')
        : baseUrl.replace(':5106', ':8080');

      try {
        const resp2 = await firstValueFrom(this.http.post<AuthResponse>(`${fallback}/api/auth/login`, request));
        if (resp2 && resp2.success && resp2.token) {
          this.setToken(resp2.token);
          if (resp2.user) {
            localStorage.setItem(this.userKey, JSON.stringify(resp2.user));
            this.currentUserSubject.next(resp2.user);
          }
        }
        return resp2 ?? { success: false, message: 'Empty response from fallback server.' };
      } catch (err2) {
        return {
          success: false,
          message: 'Login failed: unable to contact backend.'
        };
      }
    }
  }

  public logout() {
    this.setToken(null);
    localStorage.removeItem(this.userKey);
    this.currentUserSubject.next(null);
  }

  public currentUser() {
    return this.currentUserSubject.value;
  }

  // Role helpers based on your DB roles:
  // 0 = user, 1 = admin, 2 = super admin
  public isSuperAdmin(): boolean {
    const u = this.currentUser();
    return !!u && u.role === 2;
  }

  public isAdmin(): boolean {
    const u = this.currentUser();
    return !!u && (u.role === 1 || u.role === 2);
  }

  public isUser(): boolean {
    const u = this.currentUser();
    return !!u && (u.role === 0 || this.isAdmin() || this.isSuperAdmin());
  }
}
