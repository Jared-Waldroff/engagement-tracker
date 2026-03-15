import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.model';
import { UserProfile, UserRole } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'access_token';
  private readonly PROFILE_KEY = 'user_profile';
  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();
  isAuthenticated$ = this.currentUser$.pipe(map(u => u !== null));

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.restoreSession();
  }

  login(email: string, password: string): Observable<LoginResponse> {
    const body: LoginRequest = { email, password };
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, body).pipe(
      tap(response => {
        localStorage.setItem(this.TOKEN_KEY, response.accessToken);
        localStorage.setItem(this.PROFILE_KEY, JSON.stringify(response.user));
        this.currentUserSubject.next(response.user);
      })
    );
  }

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${environment.apiUrl}/api/auth/profile`).pipe(
      tap(profile => {
        localStorage.setItem(this.PROFILE_KEY, JSON.stringify(profile));
        this.currentUserSubject.next(profile);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.PROFILE_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getCurrentUser(): UserProfile | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  hasRole(role: UserRole): boolean {
    return this.currentUserSubject.value?.role === role;
  }

  private restoreSession(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const profileJson = localStorage.getItem(this.PROFILE_KEY);

    if (token && profileJson) {
      try {
        const profile: UserProfile = JSON.parse(profileJson);
        this.currentUserSubject.next(profile);
      } catch {
        this.logout();
      }
    } else if (token) {
      this.getProfile().subscribe({
        error: () => this.logout()
      });
    }
  }
}
