// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
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
    // Attempt login against the first working host, with fallback built-into apiConfig
    const baseUrl = await this.apiConfig.getApiUrl();
    const url = `${baseUrl}/api/auth/login`;
    try {
      const resp = await this.http.post<AuthResponse>(url, request).toPromise();
      if (resp && resp.success && resp.token) {
        this.setToken(resp.token);
        if (resp.user) {
          localStorage.setItem(this.userKey, JSON.stringify(resp.user));
          this.currentUserSubject.next(resp.user);
        }
      }
      return resp;
    } catch (err) {
      // If first attempt failed, try the other host explicitly (defensive)
      const fallback = baseUrl.includes(':8080') ? baseUrl.replace(':8080', ':5106') : baseUrl.replace(':5106', ':8080');
      try {
        const resp2 = await this.http.post<AuthResponse>(`${fallback}/api/auth/login`, request).toPromise();
        if (resp2 && resp2.success && resp2.token) {
          this.setToken(resp2.token);
          if (resp2.user) {
            localStorage.setItem(this.userKey, JSON.stringify(resp2.user));
            this.currentUserSubject.next(resp2.user);
          }
        }
        // update cached base to fallback if success
        // Note: we don't change the ApiConfigService.baseUrl from here to avoid circular DI; ApiConfigService will detect the next time.
        return resp2;
      } catch (err2) {
        // return a structured error response
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

  // Role helpers. Adjust role numbers to your backend's source of truth.
  public isSuperAdmin(): boolean {
    const u = this.currentUser();
    return !!u && u.role === 1; // change to whatever value means super admin in backend
  }

  public isAdmin(): boolean {
    const u = this.currentUser();
    return !!u && (u.role === 2 || this.isSuperAdmin());
  }

  public isUser(): boolean {
    const u = this.currentUser();
    return !!u && (u.role === 3 || this.isAdmin() || this.isSuperAdmin());
  }
}
