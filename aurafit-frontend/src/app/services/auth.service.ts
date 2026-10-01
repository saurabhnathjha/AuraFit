import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { ProfileService } from './profile.service';
import { GoalService } from './goal.service';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiBaseUrl;

  private router = inject(Router);
  private profileService = inject(ProfileService);
  private goalService = inject(GoalService);

  constructor(private http: HttpClient) { }

  login(dto: LoginRequest): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/auth/login`, dto);
  }

  register(dto: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register`, dto);
  }

  storeToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/landing']);
  }
  getCurrentUser(): Observable<{ id: number, username: string }> {
    return this.http.get<{ id: number, username: string }>(
      `${this.apiUrl}/users/me`,
      {
        headers: {
          Authorization: `Bearer ${this.getToken()}`
        }
      }
    );
  }

  /**
   * Redirect user after login based on profile and goal status
   */
  handlePostLoginRedirect(): void {
    this.profileService.getProfile().subscribe({
      next: () => {
        this.goalService.getGoal().subscribe({
          next: () => this.router.navigate(['/dashboard']),
          error: () => this.router.navigate(['/goal-setup'])
        });
      },
      error: () => this.router.navigate(['/profile-setup'])
    });
  }
}
