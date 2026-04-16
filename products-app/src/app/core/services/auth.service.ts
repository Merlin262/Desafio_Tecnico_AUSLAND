import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { isPlatformBrowser } from '@angular/common';

export interface AuthResponse {
  token: string;
  username: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'jwt_token';
  private readonly platformId = inject(PLATFORM_ID);
  //private readonly http = inject(HttpClient);
  //private readonly router = inject(Router);

  constructor(private http: HttpClient, private router: Router) {}

  private get isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  login(username: string, password: string) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(tap(res => this.saveToken(res.token)));
  }

  register(username: string, password: string, confirmPassword: string) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, {
      username, password, confirmPassword
    }).pipe(tap(res => this.saveToken(res.token)));
  }

  logout() {
    if (this.isBrowser) localStorage.removeItem(this.TOKEN_KEY);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (!this.isBrowser) return null; // no servidor, sempre null
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    return !this.isTokenExpired(token);
  }

  private saveToken(token: string) {
    if (this.isBrowser) localStorage.setItem(this.TOKEN_KEY, token);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return Date.now() >= payload.exp * 1000;
    } catch {
      return true;
    }
  }
}