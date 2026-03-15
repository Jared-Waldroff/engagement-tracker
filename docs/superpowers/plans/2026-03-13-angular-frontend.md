# Phase 2: Angular Frontend Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build an Angular frontend consuming the existing ASP.NET Core 8 backend API with JWT auth, role-based UI, and BDO-inspired Material theme.

**Architecture:** NgModule-based with lazy-loaded feature modules. BehaviorSubject services for state. Top navigation bar shell. Smart/dumb component pattern.

**Tech Stack:** Angular 17+, Angular Material, RxJS, TypeScript strict, SCSS

**Spec:** `docs/superpowers/specs/2026-03-12-angular-frontend-design.md`

---

## Chunk 1: Scaffold, Theme, Models

### Task 1: Scaffold Angular Project

**Files:**
- Create: `src/engagement-tracker-ui/` (entire Angular project via CLI)
- Modify: `src/engagement-tracker-ui/src/environments/environment.ts`
- Modify: `src/engagement-tracker-ui/src/environments/environment.prod.ts`
- Modify: `src/engagement-tracker-ui/tsconfig.json`

- [ ] **Step 1: Generate Angular project**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src
ng new engagement-tracker-ui --strict --routing --style=scss --skip-tests=false
```

Expected: Angular project created with strict TS, routing module, SCSS.

- [ ] **Step 2: Add Angular Material**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng add @angular/material --theme=custom --typography=true --animations=true
```

Select "Custom" theme when prompted. Accept defaults for typography and animations.

- [ ] **Step 3: Configure environments**

`src/engagement-tracker-ui/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

`src/engagement-tracker-ui/src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: '/api'
};
```

- [ ] **Step 4: Verify project builds**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng build
```

Expected: Build succeeds with 0 errors.

- [ ] **Step 5: Commit**

```bash
git add src/engagement-tracker-ui/
git commit -m "feat: scaffold Angular project with Material and strict TS"
```

---

### Task 2: Theme & Global Styles

**Files:**
- Create: `src/engagement-tracker-ui/src/theme/_variables.scss`
- Create: `src/engagement-tracker-ui/src/theme/_typography.scss`
- Modify: `src/engagement-tracker-ui/src/styles.scss`

- [ ] **Step 1: Create theme variables**

`src/engagement-tracker-ui/src/theme/_variables.scss`:
```scss
// BDO-inspired color palette
$bdo-navy:    #1a2744;
$bdo-blue:    #5b9bd5;
$bdo-white:   #ffffff;
$bdo-gray:    #f5f6fa;
$bdo-text:    #333333;
$bdo-danger:  #c0392b;
$bdo-warning: #f39c12;
$bdo-success: #27ae60;

// Spacing scale (4px base)
$space-xs: 4px;
$space-sm: 8px;
$space-md: 16px;
$space-lg: 24px;
$space-xl: 32px;

// Breakpoints
$breakpoint-tablet: 1024px;
```

- [ ] **Step 2: Create typography**

`src/engagement-tracker-ui/src/theme/_typography.scss`:
```scss
@use './variables' as *;

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: $bdo-navy;
  margin-bottom: $space-md;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: $bdo-navy;
  margin-bottom: $space-sm;
}

.label {
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #888;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: $bdo-navy;
}
```

- [ ] **Step 3: Configure Material theme in styles.scss**

`src/engagement-tracker-ui/src/styles.scss`:
```scss
@use '@angular/material' as mat;
@use './theme/variables' as *;
@use './theme/typography';

// Define BDO palettes
$bdo-primary: mat.m2-define-palette((
  50: #e3e7ed, 100: #b9c3d2, 200: #8d9db5,
  300: #617798, 400: #3d4f6e, 500: $bdo-navy,
  600: #162240, 700: #121d38, 800: #0e1830, 900: #080e20,
  contrast: (50: #000, 100: #000, 200: #000, 300: #fff,
    400: #fff, 500: #fff, 600: #fff, 700: #fff, 800: #fff, 900: #fff)
));

$bdo-accent: mat.m2-define-palette((
  50: #ebf3fb, 100: #cde1f5, 200: #adcded,
  300: #8db9e5, 400: #74aadf, 500: $bdo-blue,
  600: #538dcb, 700: #497dc0, 800: #406db5, 900: #0d47a1,
  contrast: (50: #000, 100: #000, 200: #000, 300: #000,
    400: #000, 500: #fff, 600: #fff, 700: #fff, 800: #fff, 900: #fff)
));

$bdo-warn: mat.m2-define-palette((
  50: #f8e5e3, 100: #edbeb9, 200: #e0948c,
  300: #d46a5f, 400: #ca4b3d, 500: $bdo-danger,
  600: #b53526, 700: #a22d21, 800: #90261c, 900: #701b13,
  contrast: (50: #000, 100: #000, 200: #000, 300: #fff,
    400: #fff, 500: #fff, 600: #fff, 700: #fff, 800: #fff, 900: #fff)
));

$bdo-theme: mat.m2-define-light-theme((
  color: (primary: $bdo-primary, accent: $bdo-accent, warn: $bdo-warn),
  typography: mat.m2-define-typography-config($font-family: 'Roboto, sans-serif'),
));

@include mat.all-component-themes($bdo-theme);

// Global styles
html, body {
  height: 100%;
  margin: 0;
  font-family: Roboto, 'Helvetica Neue', sans-serif;
  background-color: $bdo-gray;
  color: $bdo-text;
}

// Utility classes
.content-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: $space-md $space-lg;
}

.card {
  background: $bdo-white;
  border-radius: 8px;
  padding: $space-md;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
}

// Snackbar overrides
.success-snackbar {
  --mdc-snackbar-container-color: #{$bdo-success};
  --mdc-snackbar-supporting-text-color: #fff;
}

.error-snackbar {
  --mdc-snackbar-container-color: #{$bdo-danger};
  --mdc-snackbar-supporting-text-color: #fff;
}
```

- [ ] **Step 4: Verify build**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng build
```

- [ ] **Step 5: Commit**

```bash
git add src/engagement-tracker-ui/src/theme/ src/engagement-tracker-ui/src/styles.scss
git commit -m "feat: add BDO navy Material theme with custom palettes"
```

---

### Task 3: TypeScript Models

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/models/user.model.ts`
- Create: `src/engagement-tracker-ui/src/app/core/models/auth.model.ts`
- Create: `src/engagement-tracker-ui/src/app/core/models/engagement.model.ts`
- Create: `src/engagement-tracker-ui/src/app/core/models/dashboard.model.ts`
- Create: `src/engagement-tracker-ui/src/app/core/models/time-entry.model.ts`
- Create: `src/engagement-tracker-ui/src/app/core/models/api-error.model.ts`

- [ ] **Step 1: Create user model**

`src/engagement-tracker-ui/src/app/core/models/user.model.ts`:
```typescript
export type UserRole = 'Associate' | 'Manager' | 'Partner';

export interface UserProfile {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}
```

- [ ] **Step 2: Create auth model**

`src/engagement-tracker-ui/src/app/core/models/auth.model.ts`:
```typescript
import { UserProfile } from './user.model';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserProfile;
}
```

- [ ] **Step 3: Create engagement model**

`src/engagement-tracker-ui/src/app/core/models/engagement.model.ts`:
```typescript
export type EngagementStatus = 'Planning' | 'Active' | 'OnHold' | 'Completed' | 'Cancelled';
export type BudgetStatus = 'OnTrack' | 'AtRisk' | 'OverBudget';

export interface EngagementSummary {
  id: number;
  clientName: string;
  title: string;
  status: EngagementStatus;
  budgetHours: number;
  hoursLogged: number;
  budgetUtilizationPercent: number;
  budgetStatus: BudgetStatus;
  managerName: string;
  startDate: string;
  endDate: string | null;
}

export interface EngagementDetail {
  id: number;
  clientName: string;
  clientIndustry: string;
  title: string;
  description: string;
  status: EngagementStatus;
  budgetHours: number;
  hourlyRate: number;
  hoursLogged: number;
  budgetUtilizationPercent: number;
  budgetStatus: BudgetStatus;
  totalBudgetDollars: number;
  spentDollars: number;
  managerName: string;
  startDate: string;
  endDate: string | null;
  recentTimeEntries: TimeEntrySummary[];
  hoursByUser: UserHoursSummary[];
}

export interface TimeEntrySummary {
  id: number;
  engagementId: number;
  engagementTitle: string;
  userName: string;
  hours: number;
  date: string;
  description: string;
}

export interface UserHoursSummary {
  userId: number;
  userName: string;
  role: string;
  totalHours: number;
}

export interface EngagementFilters {
  status?: EngagementStatus | '';
  search?: string;
}

export interface CreateEngagement {
  title: string;
  description: string;
  clientId: number;
  budgetHours: number;
  hourlyRate: number;
  startDate: string;
  endDate?: string;
}

export interface UpdateEngagement {
  title?: string;
  description?: string;
  status?: EngagementStatus;
  budgetHours?: number;
  hourlyRate?: number;
  endDate?: string;
}
```

- [ ] **Step 4: Create dashboard model**

`src/engagement-tracker-ui/src/app/core/models/dashboard.model.ts`:
```typescript
import { EngagementSummary } from './engagement.model';

export interface Dashboard {
  totalEngagements: number;
  activeEngagements: number;
  totalHoursLogged: number;
  totalBudgetHours: number;
  overallUtilizationPercent: number;
  engagementsOnTrack: number;
  engagementsAtRisk: number;
  engagementsOverBudget: number;
  topEngagements: EngagementSummary[];
}
```

- [ ] **Step 5: Create time-entry model**

`src/engagement-tracker-ui/src/app/core/models/time-entry.model.ts`:
```typescript
export interface TimeEntry {
  id: number;
  engagementId: number;
  engagementTitle: string;
  userName: string;
  hours: number;
  date: string;
  description: string;
}

export interface CreateTimeEntry {
  engagementId: number;
  hours: number;
  date: string;
  description: string;
}
```

- [ ] **Step 6: Create api-error model**

`src/engagement-tracker-ui/src/app/core/models/api-error.model.ts`:
```typescript
export interface ApiError {
  error: {
    code: string;
    message: string;
    timestamp: string;
    traceId: string;
  };
}
```

- [ ] **Step 7: Create barrel export**

`src/engagement-tracker-ui/src/app/core/models/index.ts`:
```typescript
export * from './user.model';
export * from './auth.model';
export * from './engagement.model';
export * from './dashboard.model';
export * from './time-entry.model';
export * from './api-error.model';
```

- [ ] **Step 8: Verify build**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng build
```

- [ ] **Step 9: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/models/
git commit -m "feat: add TypeScript model interfaces matching backend DTOs"
```

---

## Chunk 2: Core Services & Auth Infrastructure

### Task 4: AuthService

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/services/auth.service.ts`
- Create: `src/engagement-tracker-ui/src/app/core/services/auth.service.spec.ts`

- [ ] **Step 1: Write AuthService tests**

`src/engagement-tracker-ui/src/app/core/services/auth.service.spec.ts`:
```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from './auth.service';
import { LoginResponse } from '../models';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const mockLoginResponse: LoginResponse = {
    accessToken: 'mock-jwt-token',
    tokenType: 'Bearer',
    expiresIn: 3600,
    user: {
      id: 1,
      email: 'alice@example.com',
      firstName: 'Alice',
      lastName: 'Johnson',
      role: 'Associate'
    }
  };

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, MatSnackBarModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should start as unauthenticated', () => {
    expect(service.getCurrentUser()).toBeNull();
  });

  it('should login and store token', () => {
    service.login('alice@example.com', 'password123').subscribe(response => {
      expect(response.accessToken).toBe('mock-jwt-token');
      expect(service.getCurrentUser()?.email).toBe('alice@example.com');
      expect(localStorage.getItem('access_token')).toBe('mock-jwt-token');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'alice@example.com', password: 'password123' });
    req.flush(mockLoginResponse);
  });

  it('should logout and clear storage', () => {
    localStorage.setItem('access_token', 'mock-jwt-token');
    localStorage.setItem('user_profile', JSON.stringify(mockLoginResponse.user));
    service.logout();
    expect(service.getCurrentUser()).toBeNull();
    expect(localStorage.getItem('access_token')).toBeNull();
  });

  it('should restore user from localStorage on init', () => {
    // Set localStorage before creating the service
    localStorage.setItem('access_token', 'mock-jwt-token');
    localStorage.setItem('user_profile', JSON.stringify(mockLoginResponse.user));

    // Re-create the service via TestBed to trigger constructor with proper DI
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, MatSnackBarModule],
      providers: [AuthService]
    });
    const freshService = TestBed.inject(AuthService);
    expect(freshService.getCurrentUser()?.email).toBe('alice@example.com');
  });

  it('should check role correctly', () => {
    service.login('alice@example.com', 'password123').subscribe(() => {
      expect(service.hasRole('Associate')).toBeTrue();
      expect(service.hasRole('Manager')).toBeFalse();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    req.flush(mockLoginResponse);
  });
});
```

- [ ] **Step 2: Implement AuthService**

`src/engagement-tracker-ui/src/app/core/services/auth.service.ts`:
```typescript
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
```

- [ ] **Step 3: Run tests**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng test --watch=false --browsers=ChromeHeadless
```

- [ ] **Step 4: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/services/auth.service*
git commit -m "feat: add AuthService with JWT login, session restore, and role checks"
```

---

### Task 5: JWT Interceptor

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/interceptors/jwt.interceptor.ts`

- [ ] **Step 1: Implement JWT interceptor**

`src/engagement-tracker-ui/src/app/core/interceptors/jwt.interceptor.ts`:
```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();
    const isApiUrl = request.url.startsWith(environment.apiUrl);

    if (token && isApiUrl) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(request);
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/interceptors/jwt.interceptor.ts
git commit -m "feat: add JWT interceptor for API requests"
```

---

### Task 6: Error Interceptor

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/interceptors/error.interceptor.ts`

- [ ] **Step 1: Implement Error interceptor**

`src/engagement-tracker-ui/src/app/core/interceptors/error.interceptor.ts`:
```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        switch (error.status) {
          case 401:
            this.authService.logout();
            break;
          case 403:
            this.snackBar.open('Access denied.', 'Dismiss', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
            break;
          case 0:
          case 500:
          case 502:
          case 503:
            this.snackBar.open('Something went wrong. Please try again.', 'Dismiss', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
            break;
        }
        return throwError(() => error);
      })
    );
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/interceptors/error.interceptor.ts
git commit -m "feat: add error interceptor with snackbar notifications"
```

---

### Task 7: AuthGuard

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/guards/auth.guard.ts`

- [ ] **Step 1: Implement AuthGuard**

`src/engagement-tracker-ui/src/app/core/guards/auth.guard.ts`:
```typescript
import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.isAuthenticated$.pipe(
      take(1),
      map(isAuth => isAuth || this.router.createUrlTree(['/login']))
    );
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/guards/auth.guard.ts
git commit -m "feat: add AuthGuard redirecting unauthenticated users to login"
```

---

### Task 8: EngagementService

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/services/engagement.service.ts`

- [ ] **Step 1: Implement EngagementService**

`src/engagement-tracker-ui/src/app/core/services/engagement.service.ts`:
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  EngagementSummary, EngagementDetail, EngagementFilters,
  CreateEngagement, UpdateEngagement
} from '../models/engagement.model';
import { Dashboard } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class EngagementService {
  private engagementsSubject = new BehaviorSubject<EngagementSummary[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  engagements$ = this.engagementsSubject.asObservable();
  loading$ = this.loadingSubject.asObservable();

  private readonly baseUrl = `${environment.apiUrl}/api/engagements`;

  constructor(private http: HttpClient) {}

  loadEngagements(filters?: EngagementFilters): void {
    this.loadingSubject.next(true);
    let params = new HttpParams();
    if (filters?.status) {
      params = params.set('status', filters.status);
    }
    if (filters?.search) {
      params = params.set('search', filters.search);
    }

    this.http.get<EngagementSummary[]>(this.baseUrl, { params }).subscribe({
      next: data => {
        this.engagementsSubject.next(data);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  getEngagementDetail(id: number): Observable<EngagementDetail> {
    return this.http.get<EngagementDetail>(`${this.baseUrl}/${id}`);
  }

  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.baseUrl}/dashboard`);
  }

  createEngagement(dto: CreateEngagement): Observable<EngagementSummary> {
    return this.http.post<EngagementSummary>(this.baseUrl, dto).pipe(
      tap(() => this.loadEngagements())
    );
  }

  updateEngagement(id: number, dto: UpdateEngagement): Observable<EngagementSummary> {
    return this.http.put<EngagementSummary>(`${this.baseUrl}/${id}`, dto).pipe(
      tap(() => this.loadEngagements())
    );
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/services/engagement.service.ts
git commit -m "feat: add EngagementService with BehaviorSubject state management"
```

---

### Task 9: TimeEntryService

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/services/time-entry.service.ts`

- [ ] **Step 1: Implement TimeEntryService**

`src/engagement-tracker-ui/src/app/core/services/time-entry.service.ts`:
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TimeEntry, CreateTimeEntry } from '../models/time-entry.model';

@Injectable({ providedIn: 'root' })
export class TimeEntryService {
  private timeEntriesSubject = new BehaviorSubject<TimeEntry[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  timeEntries$ = this.timeEntriesSubject.asObservable();
  loading$ = this.loadingSubject.asObservable();

  private readonly baseUrl = `${environment.apiUrl}/api/time-entries`;
  private lastEngagementFilter?: number;

  constructor(private http: HttpClient) {}

  loadTimeEntries(engagementId?: number): void {
    this.loadingSubject.next(true);
    this.lastEngagementFilter = engagementId;

    let params = new HttpParams();
    if (engagementId) {
      params = params.set('engagementId', engagementId.toString());
    }

    this.http.get<TimeEntry[]>(this.baseUrl, { params }).subscribe({
      next: data => {
        this.timeEntriesSubject.next(data);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  createTimeEntry(dto: CreateTimeEntry): Observable<TimeEntry> {
    return this.http.post<TimeEntry>(this.baseUrl, dto).pipe(
      tap(() => this.loadTimeEntries(this.lastEngagementFilter))
    );
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/services/time-entry.service.ts
git commit -m "feat: add TimeEntryService with BehaviorSubject state management"
```

---

### Task 10: CoreModule

**Files:**
- Create: `src/engagement-tracker-ui/src/app/core/core.module.ts`

- [ ] **Step 1: Create CoreModule with interceptor providers**

`src/engagement-tracker-ui/src/app/core/core.module.ts`:
```typescript
import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it only in AppModule.');
    }
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/core/core.module.ts
git commit -m "feat: add CoreModule with interceptor registration and import guard"
```

---

## Chunk 3: Shared Module

### Task 11: Shared Components, Pipe, and Module

**Files:**
- Create: `src/engagement-tracker-ui/src/app/shared/components/status-badge/status-badge.component.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/components/status-badge/status-badge.component.html`
- Create: `src/engagement-tracker-ui/src/app/shared/components/status-badge/status-badge.component.scss`
- Create: `src/engagement-tracker-ui/src/app/shared/components/budget-progress-bar/budget-progress-bar.component.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/components/budget-progress-bar/budget-progress-bar.component.html`
- Create: `src/engagement-tracker-ui/src/app/shared/components/budget-progress-bar/budget-progress-bar.component.scss`
- Create: `src/engagement-tracker-ui/src/app/shared/components/loading-skeleton/loading-skeleton.component.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/components/loading-skeleton/loading-skeleton.component.html`
- Create: `src/engagement-tracker-ui/src/app/shared/components/loading-skeleton/loading-skeleton.component.scss`
- Create: `src/engagement-tracker-ui/src/app/shared/components/empty-state/empty-state.component.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/components/empty-state/empty-state.component.html`
- Create: `src/engagement-tracker-ui/src/app/shared/components/empty-state/empty-state.component.scss`
- Create: `src/engagement-tracker-ui/src/app/shared/components/error-state/error-state.component.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/components/error-state/error-state.component.html`
- Create: `src/engagement-tracker-ui/src/app/shared/components/error-state/error-state.component.scss`
- Create: `src/engagement-tracker-ui/src/app/shared/pipes/hours.pipe.ts`
- Create: `src/engagement-tracker-ui/src/app/shared/shared.module.ts`

- [ ] **Step 1: Create StatusBadgeComponent**

`status-badge.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  templateUrl: './status-badge.component.html',
  styleUrls: ['./status-badge.component.scss']
})
export class StatusBadgeComponent {
  @Input() status = '';

  get colorClass(): string {
    switch (this.status) {
      case 'Active': return 'badge-active';
      case 'Planning': return 'badge-planning';
      case 'OnHold': return 'badge-onhold';
      case 'Completed': return 'badge-completed';
      case 'Cancelled': return 'badge-cancelled';
      case 'OnTrack': return 'badge-ontrack';
      case 'AtRisk': return 'badge-atrisk';
      case 'OverBudget': return 'badge-overbudget';
      default: return 'badge-planning';
    }
  }
}
```

`status-badge.component.html`:
```html
<span class="badge" [ngClass]="colorClass">{{ status }}</span>
```

`status-badge.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.badge {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  white-space: nowrap;
}

.badge-active, .badge-ontrack { background: rgba($bdo-blue, 0.15); color: $bdo-blue; }
.badge-planning { background: #eee; color: #666; }
.badge-onhold, .badge-atrisk { background: rgba($bdo-warning, 0.15); color: darken($bdo-warning, 10%); }
.badge-completed { background: rgba($bdo-success, 0.15); color: $bdo-success; }
.badge-cancelled, .badge-overbudget { background: rgba($bdo-danger, 0.15); color: $bdo-danger; }
```

- [ ] **Step 2: Create BudgetProgressBarComponent**

`budget-progress-bar.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-budget-progress-bar',
  templateUrl: './budget-progress-bar.component.html',
  styleUrls: ['./budget-progress-bar.component.scss']
})
export class BudgetProgressBarComponent {
  @Input() utilization = 0;

  get colorClass(): string {
    if (this.utilization >= 100) return 'bar-danger';
    if (this.utilization >= 80) return 'bar-warning';
    return 'bar-success';
  }

  get clampedWidth(): number {
    return Math.min(this.utilization, 100);
  }
}
```

`budget-progress-bar.component.html`:
```html
<div class="progress-container">
  <div class="progress-bar" [ngClass]="colorClass" [style.width.%]="clampedWidth"></div>
</div>
<span class="progress-label">{{ utilization | number:'1.0-1' }}%</span>
```

`budget-progress-bar.component.scss`:
```scss
@use '../../../../theme/variables' as *;

:host { display: flex; align-items: center; gap: 8px; }

.progress-container {
  flex: 1;
  height: 8px;
  background: #e9ecef;
  border-radius: 4px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  border-radius: 4px;
  transition: width 0.3s ease;
}

.bar-success { background: $bdo-success; }
.bar-warning { background: $bdo-warning; }
.bar-danger { background: $bdo-danger; }

.progress-label { font-size: 13px; font-weight: 500; min-width: 45px; text-align: right; }
```

- [ ] **Step 3: Create LoadingSkeletonComponent**

`loading-skeleton.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-skeleton',
  templateUrl: './loading-skeleton.component.html',
  styleUrls: ['./loading-skeleton.component.scss']
})
export class LoadingSkeletonComponent {
  @Input() rows = 5;
  @Input() type: 'table' | 'cards' = 'table';

  get rowArray(): number[] {
    return Array.from({ length: this.rows }, (_, i) => i);
  }
}
```

`loading-skeleton.component.html`:
```html
<div *ngIf="type === 'cards'" class="skeleton-cards">
  <div *ngFor="let _ of rowArray" class="skeleton-card">
    <div class="skeleton-line short"></div>
    <div class="skeleton-line long"></div>
  </div>
</div>
<div *ngIf="type === 'table'" class="skeleton-table">
  <div *ngFor="let _ of rowArray" class="skeleton-row">
    <div class="skeleton-line" *ngFor="let w of [60, 80, 40, 70, 50]" [style.width.%]="w"></div>
  </div>
</div>
```

`loading-skeleton.component.scss`:
```scss
.skeleton-line {
  height: 14px;
  background: linear-gradient(90deg, #eee 25%, #e0e0e0 50%, #eee 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 4px;
  margin-bottom: 8px;

  &.short { width: 40%; }
  &.long { width: 75%; }
}

.skeleton-cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
}

.skeleton-card {
  background: #fff;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}

.skeleton-row {
  display: flex;
  gap: 16px;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
```

- [ ] **Step 4: Create EmptyStateComponent**

`empty-state.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  templateUrl: './empty-state.component.html',
  styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent {
  @Input() message = 'No data found.';
  @Input() icon = 'inbox';
}
```

`empty-state.component.html`:
```html
<div class="empty-state">
  <mat-icon class="empty-icon">{{ icon }}</mat-icon>
  <p>{{ message }}</p>
</div>
```

`empty-state.component.scss`:
```scss
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 16px;
  color: #999;
}

.empty-icon { font-size: 48px; width: 48px; height: 48px; margin-bottom: 16px; color: #ccc; }
```

- [ ] **Step 5: Create ErrorStateComponent**

`error-state.component.ts`:
```typescript
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-error-state',
  templateUrl: './error-state.component.html',
  styleUrls: ['./error-state.component.scss']
})
export class ErrorStateComponent {
  @Input() message = 'Something went wrong.';
  @Output() retry = new EventEmitter<void>();
}
```

`error-state.component.html`:
```html
<div class="error-state">
  <mat-icon class="error-icon">error_outline</mat-icon>
  <p>{{ message }}</p>
  <button mat-stroked-button color="primary" (click)="retry.emit()">Retry</button>
</div>
```

`error-state.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 16px;
  color: $bdo-danger;
}

.error-icon { font-size: 48px; width: 48px; height: 48px; margin-bottom: 16px; }
```

- [ ] **Step 6: Create HoursPipe**

`src/engagement-tracker-ui/src/app/shared/pipes/hours.pipe.ts`:
```typescript
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'hours' })
export class HoursPipe implements PipeTransform {
  transform(value: number): string {
    if (value == null) return '';
    const h = Math.floor(value);
    const m = Math.round((value - h) * 60);
    if (m === 0) return `${h}h`;
    if (h === 0) return `${m}m`;
    return `${h}h ${m}m`;
  }
}
```

- [ ] **Step 7: Create SharedModule**

`src/engagement-tracker-ui/src/app/shared/shared.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { StatusBadgeComponent } from './components/status-badge/status-badge.component';
import { BudgetProgressBarComponent } from './components/budget-progress-bar/budget-progress-bar.component';
import { LoadingSkeletonComponent } from './components/loading-skeleton/loading-skeleton.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { ErrorStateComponent } from './components/error-state/error-state.component';
import { HoursPipe } from './pipes/hours.pipe';

const COMPONENTS = [
  StatusBadgeComponent,
  BudgetProgressBarComponent,
  LoadingSkeletonComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  HoursPipe
];

@NgModule({
  declarations: COMPONENTS,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  exports: COMPONENTS
})
export class SharedModule {}
```

- [ ] **Step 8: Verify build**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng build
```

- [ ] **Step 9: Commit**

```bash
git add src/engagement-tracker-ui/src/app/shared/
git commit -m "feat: add SharedModule with status badge, progress bar, skeleton, empty/error states, hours pipe"
```

---

## Chunk 4: Auth & Dashboard Features

### Task 12: Auth Feature Module (Login)

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/auth/login/login.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/auth/login/login.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/auth/login/login.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/auth/auth-routing.module.ts`
- Create: `src/engagement-tracker-ui/src/app/features/auth/auth.module.ts`

- [ ] **Step 1: Create LoginComponent**

`login.component.ts`:
```typescript
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = null;
    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.status === 401
          ? 'Invalid email or password.'
          : 'Unable to sign in. Please try again.';
      }
    });
  }
}
```

`login.component.html`:
```html
<div class="login-container">
  <mat-card class="login-card">
    <mat-card-header>
      <mat-card-title>Engagement Tracker</mat-card-title>
      <mat-card-subtitle>Sign in to continue</mat-card-subtitle>
    </mat-card-header>
    <mat-card-content>
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Email</mat-label>
          <input matInput formControlName="email" type="email" autocomplete="email">
          <mat-error *ngIf="loginForm.get('email')?.hasError('required')">Email is required</mat-error>
          <mat-error *ngIf="loginForm.get('email')?.hasError('email')">Enter a valid email</mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Password</mat-label>
          <input matInput formControlName="password"
                 [type]="hidePassword ? 'password' : 'text'"
                 autocomplete="current-password">
          <button mat-icon-button matSuffix type="button"
                  (click)="hidePassword = !hidePassword">
            <mat-icon>{{ hidePassword ? 'visibility_off' : 'visibility' }}</mat-icon>
          </button>
          <mat-error>Password is required</mat-error>
        </mat-form-field>

        <div class="error-message" *ngIf="errorMessage">
          <mat-icon>error</mat-icon>
          <span>{{ errorMessage }}</span>
        </div>

        <button mat-raised-button color="primary" type="submit"
                class="full-width login-button"
                [disabled]="loginForm.invalid || isLoading">
          <span *ngIf="!isLoading">Sign In</span>
          <mat-spinner *ngIf="isLoading" diameter="20"></mat-spinner>
        </button>
      </form>

      <div class="demo-credentials">
        <p class="label">Demo Credentials</p>
        <p>alice@example.com / password123 (Associate)</p>
        <p>bob@example.com / password123 (Manager)</p>
        <p>carol@example.com / password123 (Partner)</p>
      </div>
    </mat-card-content>
  </mat-card>
</div>
```

`login.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.login-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: $bdo-navy;
}

.login-card {
  width: 100%;
  max-width: 400px;
  padding: $space-lg;
}

mat-card-title { color: $bdo-navy; font-size: 24px !important; }

.full-width { width: 100%; }

.login-button { margin-top: $space-md; height: 48px; font-size: 16px; }

.error-message {
  display: flex;
  align-items: center;
  gap: 8px;
  color: $bdo-danger;
  margin-bottom: $space-sm;
  font-size: 14px;
  mat-icon { font-size: 18px; width: 18px; height: 18px; }
}

.demo-credentials {
  margin-top: $space-lg;
  padding-top: $space-md;
  border-top: 1px solid #eee;
  font-size: 12px;
  color: #888;
  p { margin: 2px 0; }
  .label { margin-bottom: 4px; }
}
```

- [ ] **Step 2: Create auth routing and module**

`auth-routing.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  { path: '', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule {}
```

`auth.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './login/login.component';

@NgModule({
  declarations: [LoginComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AuthRoutingModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ]
})
export class AuthModule {}
```

- [ ] **Step 3: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/auth/
git commit -m "feat: add login screen with reactive form, validation, and demo credentials"
```

---

### Task 13: Dashboard Feature Module

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/dashboard/dashboard.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/dashboard/dashboard.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/dashboard/dashboard.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/stat-card/stat-card.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/stat-card/stat-card.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/stat-card/stat-card.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/budget-donut/budget-donut.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/budget-donut/budget-donut.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/budget-donut/budget-donut.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/top-engagements-table/top-engagements-table.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/top-engagements-table/top-engagements-table.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/widgets/top-engagements-table/top-engagements-table.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/dashboard-routing.module.ts`
- Create: `src/engagement-tracker-ui/src/app/features/dashboard/dashboard.module.ts`

- [ ] **Step 1: Create StatCardComponent (dumb)**

`stat-card.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  templateUrl: './stat-card.component.html',
  styleUrls: ['./stat-card.component.scss']
})
export class StatCardComponent {
  @Input() label = '';
  @Input() value: string | number = '';
  @Input() icon = '';
  @Input() color = '';
}
```

`stat-card.component.html`:
```html
<div class="stat-card card">
  <div class="stat-header">
    <span class="label">{{ label }}</span>
    <mat-icon *ngIf="icon" [style.color]="color">{{ icon }}</mat-icon>
  </div>
  <div class="stat-value" [style.color]="color">{{ value }}</div>
</div>
```

`stat-card.component.scss`:
```scss
@use '../../../../../theme/variables' as *;

.stat-card { padding: $space-md; }
.stat-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: $space-sm; }
.stat-value { font-size: 28px; font-weight: 700; color: $bdo-navy; }
```

- [ ] **Step 2: Create BudgetDonutComponent (dumb)**

`budget-donut.component.ts`:
```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-budget-donut',
  templateUrl: './budget-donut.component.html',
  styleUrls: ['./budget-donut.component.scss']
})
export class BudgetDonutComponent {
  @Input() onTrack = 0;
  @Input() atRisk = 0;
  @Input() overBudget = 0;

  get total(): number {
    return this.onTrack + this.atRisk + this.overBudget;
  }

  get segments(): string {
    if (this.total === 0) return 'conic-gradient(#eee 0deg 360deg)';
    const onTrackDeg = (this.onTrack / this.total) * 360;
    const atRiskDeg = (this.atRisk / this.total) * 360;
    return `conic-gradient(
      #27ae60 0deg ${onTrackDeg}deg,
      #f39c12 ${onTrackDeg}deg ${onTrackDeg + atRiskDeg}deg,
      #c0392b ${onTrackDeg + atRiskDeg}deg 360deg
    )`;
  }
}
```

`budget-donut.component.html`:
```html
<div class="donut-container card">
  <h3 class="section-title">Budget Status</h3>
  <div class="donut-layout">
    <div class="donut" [style.background]="segments">
      <div class="donut-hole">
        <span class="donut-total">{{ total }}</span>
        <span class="donut-label">Total</span>
      </div>
    </div>
    <div class="legend">
      <div class="legend-item">
        <span class="dot dot-success"></span>
        <span>On Track ({{ onTrack }})</span>
      </div>
      <div class="legend-item">
        <span class="dot dot-warning"></span>
        <span>At Risk ({{ atRisk }})</span>
      </div>
      <div class="legend-item">
        <span class="dot dot-danger"></span>
        <span>Over Budget ({{ overBudget }})</span>
      </div>
    </div>
  </div>
</div>
```

`budget-donut.component.scss`:
```scss
@use '../../../../../theme/variables' as *;

.donut-container { padding: $space-md; }

.donut-layout { display: flex; align-items: center; gap: $space-lg; }

.donut {
  width: 160px; height: 160px;
  border-radius: 50%;
  position: relative;
  flex-shrink: 0;
}

.donut-hole {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 100px; height: 100px;
  border-radius: 50%;
  background: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.donut-total { font-size: 28px; font-weight: 700; color: $bdo-navy; }
.donut-label { font-size: 12px; color: #888; }

.legend { display: flex; flex-direction: column; gap: 8px; }
.legend-item { display: flex; align-items: center; gap: 8px; font-size: 14px; }
.dot { width: 12px; height: 12px; border-radius: 50%; flex-shrink: 0; }
.dot-success { background: $bdo-success; }
.dot-warning { background: $bdo-warning; }
.dot-danger { background: $bdo-danger; }
```

- [ ] **Step 3: Create TopEngagementsTableComponent (dumb)**

`top-engagements-table.component.ts`:
```typescript
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EngagementSummary } from '../../../../core/models';

@Component({
  selector: 'app-top-engagements-table',
  templateUrl: './top-engagements-table.component.html',
  styleUrls: ['./top-engagements-table.component.scss']
})
export class TopEngagementsTableComponent {
  @Input() engagements: EngagementSummary[] = [];
  @Output() rowClick = new EventEmitter<EngagementSummary>();

  displayedColumns = ['clientName', 'title', 'status', 'utilization'];
}
```

`top-engagements-table.component.html`:
```html
<div class="card">
  <h3 class="section-title">Top Engagements</h3>
  <table mat-table [dataSource]="engagements" class="engagement-table">
    <ng-container matColumnDef="clientName">
      <th mat-header-cell *matHeaderCellDef>Client</th>
      <td mat-cell *matCellDef="let e">{{ e.clientName }}</td>
    </ng-container>
    <ng-container matColumnDef="title">
      <th mat-header-cell *matHeaderCellDef>Engagement</th>
      <td mat-cell *matCellDef="let e">{{ e.title }}</td>
    </ng-container>
    <ng-container matColumnDef="status">
      <th mat-header-cell *matHeaderCellDef>Status</th>
      <td mat-cell *matCellDef="let e"><app-status-badge [status]="e.status"></app-status-badge></td>
    </ng-container>
    <ng-container matColumnDef="utilization">
      <th mat-header-cell *matHeaderCellDef>Budget</th>
      <td mat-cell *matCellDef="let e"><app-budget-progress-bar [utilization]="e.budgetUtilizationPercent"></app-budget-progress-bar></td>
    </ng-container>
    <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
    <tr mat-row *matRowDef="let row; columns: displayedColumns;"
        (click)="rowClick.emit(row)" class="clickable-row"></tr>
  </table>
</div>
```

`top-engagements-table.component.scss`:
```scss
.engagement-table { width: 100%; }
.clickable-row { cursor: pointer; }
.clickable-row:hover { background: rgba(0, 0, 0, 0.04); }
```

- [ ] **Step 4: Create DashboardComponent (smart)**

`dashboard.component.ts`:
```typescript
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EngagementService } from '../../../core/services/engagement.service';
import { Dashboard } from '../../../core/models';
import { EngagementSummary } from '../../../core/models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  dashboard: Dashboard | null = null;
  isLoading = true;
  error: string | null = null;

  constructor(
    private engagementService: EngagementService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  onEngagementClick(engagement: EngagementSummary): void {
    this.router.navigate(['/engagements', engagement.id]);
  }

  onRetry(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {
    this.isLoading = true;
    this.error = null;
    this.engagementService.getDashboard().subscribe({
      next: data => {
        this.dashboard = data;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load dashboard. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
```

`dashboard.component.html`:
```html
<div class="content-container">
  <h2 class="page-title">Dashboard</h2>

  <app-loading-skeleton *ngIf="isLoading" type="cards" [rows]="4"></app-loading-skeleton>
  <app-error-state *ngIf="error" [message]="error" (retry)="onRetry()"></app-error-state>

  <ng-container *ngIf="dashboard && !isLoading && !error">
    <!-- Stat Cards -->
    <div class="stat-grid">
      <app-stat-card label="Total Engagements" [value]="dashboard.totalEngagements"
                     icon="folder" color="#1a2744"></app-stat-card>
      <app-stat-card label="Active" [value]="dashboard.activeEngagements"
                     icon="trending_up" color="#5b9bd5"></app-stat-card>
      <app-stat-card label="Total Hours" [value]="dashboard.totalHoursLogged + ' / ' + dashboard.totalBudgetHours"
                     icon="schedule" color="#1a2744"></app-stat-card>
      <app-stat-card label="Utilization" [value]="dashboard.overallUtilizationPercent + '%'"
                     icon="speed" [color]="dashboard.overallUtilizationPercent >= 100 ? '#c0392b' : dashboard.overallUtilizationPercent >= 80 ? '#f39c12' : '#27ae60'"></app-stat-card>
    </div>

    <!-- Budget Donut -->
    <div class="chart-row">
      <app-budget-donut
        [onTrack]="dashboard.engagementsOnTrack"
        [atRisk]="dashboard.engagementsAtRisk"
        [overBudget]="dashboard.engagementsOverBudget">
      </app-budget-donut>
    </div>

    <!-- Top Engagements -->
    <app-top-engagements-table
      [engagements]="dashboard.topEngagements"
      (rowClick)="onEngagementClick($event)">
    </app-top-engagements-table>
  </ng-container>
</div>
```

`dashboard.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.stat-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: $space-md;
  margin-bottom: $space-lg;

  @media (max-width: $breakpoint-tablet) {
    grid-template-columns: repeat(2, 1fr);
  }
}

.chart-row { margin-bottom: $space-lg; }
```

- [ ] **Step 5: Create dashboard routing and module**

`dashboard-routing.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [{ path: '', component: DashboardComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule {}
```

`dashboard.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { SharedModule } from '../../shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { StatCardComponent } from './widgets/stat-card/stat-card.component';
import { BudgetDonutComponent } from './widgets/budget-donut/budget-donut.component';
import { TopEngagementsTableComponent } from './widgets/top-engagements-table/top-engagements-table.component';

@NgModule({
  declarations: [
    DashboardComponent,
    StatCardComponent,
    BudgetDonutComponent,
    TopEngagementsTableComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule,
    MatIconModule,
    MatTableModule
  ]
})
export class DashboardModule {}
```

- [ ] **Step 6: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/dashboard/
git commit -m "feat: add dashboard with stat cards, budget donut chart, and top engagements table"
```

---

## Chunk 5: Engagements Feature

### Task 14: Engagement List Component

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-list/engagement-list.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-list/engagement-list.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-list/engagement-list.component.scss`

- [ ] **Step 1: Create EngagementListComponent (smart)**

`engagement-list.component.ts`:
```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementSummary, EngagementStatus } from '../../../core/models';
import { EngagementFormDialogComponent } from '../engagement-form-dialog/engagement-form-dialog.component';

@Component({
  selector: 'app-engagement-list',
  templateUrl: './engagement-list.component.html',
  styleUrls: ['./engagement-list.component.scss']
})
export class EngagementListComponent implements OnInit, OnDestroy {
  engagements$ = this.engagementService.engagements$;
  loading$ = this.engagementService.loading$;
  error: string | null = null;

  statusFilter: EngagementStatus | '' = '';
  searchQuery = '';
  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  displayedColumns = ['clientName', 'title', 'status', 'utilization', 'managerName', 'startDate'];

  statuses: EngagementStatus[] = ['Planning', 'Active', 'OnHold', 'Completed', 'Cancelled'];

  get canCreate(): boolean {
    return this.authService.hasRole('Manager') || this.authService.hasRole('Partner');
  }

  constructor(
    private engagementService: EngagementService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(search => {
      this.searchQuery = search;
      this.loadData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  onStatusChange(): void {
    this.loadData();
  }

  onRowClick(engagement: EngagementSummary): void {
    this.router.navigate(['/engagements', engagement.id]);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(EngagementFormDialogComponent, {
      width: '600px',
      data: { mode: 'create', clients: this.getUniqueClients() }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadData();
    });
  }

  onRetry(): void {
    this.loadData();
  }

  private loadData(): void {
    this.error = null;
    this.engagementService.loadEngagements({
      status: this.statusFilter,
      search: this.searchQuery
    });
  }

  private getUniqueClients(): { id: number; name: string }[] {
    const seen = new Map<string, number>();
    // Extract unique clients from current engagement list
    // Note: clientId is not in EngagementSummary, so we use clientName as key
    // For create, we need clientId. This is a known limitation — for the portfolio
    // demo with seed data, we hardcode the 5 clients from DbSeeder.
    return [
      { id: 1, name: 'Maple Energy Corp' },
      { id: 2, name: 'Northern Financial Group' },
      { id: 3, name: 'Pacific Health Systems' },
      { id: 4, name: 'TechVault Solutions' },
      { id: 5, name: 'Prairie Retail Holdings' }
    ];
  }
}
```

`engagement-list.component.html`:
```html
<div class="content-container">
  <div class="page-header">
    <h2 class="page-title">Engagements</h2>
    <button mat-raised-button color="primary" *ngIf="canCreate" (click)="openCreateDialog()">
      <mat-icon>add</mat-icon> New Engagement
    </button>
  </div>

  <!-- Filters -->
  <div class="filters">
    <mat-form-field appearance="outline" class="filter-field">
      <mat-label>Status</mat-label>
      <mat-select [(value)]="statusFilter" (selectionChange)="onStatusChange()">
        <mat-option value="">All</mat-option>
        <mat-option *ngFor="let s of statuses" [value]="s">{{ s }}</mat-option>
      </mat-select>
    </mat-form-field>
    <mat-form-field appearance="outline" class="search-field">
      <mat-label>Search</mat-label>
      <input matInput placeholder="Client or engagement name..."
             (input)="onSearchChange($any($event.target).value)">
      <mat-icon matSuffix>search</mat-icon>
    </mat-form-field>
  </div>

  <!-- Loading -->
  <app-loading-skeleton *ngIf="loading$ | async" type="table" [rows]="6"></app-loading-skeleton>

  <!-- Error -->
  <app-error-state *ngIf="error" [message]="error" (retry)="onRetry()"></app-error-state>

  <!-- Data -->
  <ng-container *ngIf="(engagements$ | async) as engagements">
    <app-empty-state *ngIf="engagements.length === 0 && !(loading$ | async)"
                     message="No engagements found. Adjust your filters."
                     icon="folder_off">
    </app-empty-state>

    <div class="card" *ngIf="engagements.length > 0">
      <table mat-table [dataSource]="engagements" class="full-width">
        <ng-container matColumnDef="clientName">
          <th mat-header-cell *matHeaderCellDef>Client</th>
          <td mat-cell *matCellDef="let e">{{ e.clientName }}</td>
        </ng-container>
        <ng-container matColumnDef="title">
          <th mat-header-cell *matHeaderCellDef>Engagement</th>
          <td mat-cell *matCellDef="let e">{{ e.title }}</td>
        </ng-container>
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let e"><app-status-badge [status]="e.status"></app-status-badge></td>
        </ng-container>
        <ng-container matColumnDef="utilization">
          <th mat-header-cell *matHeaderCellDef>Budget</th>
          <td mat-cell *matCellDef="let e">
            <app-budget-progress-bar [utilization]="e.budgetUtilizationPercent"></app-budget-progress-bar>
          </td>
        </ng-container>
        <ng-container matColumnDef="managerName">
          <th mat-header-cell *matHeaderCellDef class="hide-tablet">Manager</th>
          <td mat-cell *matCellDef="let e" class="hide-tablet">{{ e.managerName }}</td>
        </ng-container>
        <ng-container matColumnDef="startDate">
          <th mat-header-cell *matHeaderCellDef class="hide-tablet">Start Date</th>
          <td mat-cell *matCellDef="let e" class="hide-tablet">{{ e.startDate | date:'mediumDate' }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"
            (click)="onRowClick(row)" class="clickable-row"></tr>
      </table>
    </div>
  </ng-container>
</div>
```

`engagement-list.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.page-header { display: flex; justify-content: space-between; align-items: center; }

.filters { display: flex; gap: $space-md; margin-bottom: $space-md; }
.filter-field { width: 200px; }
.search-field { flex: 1; max-width: 400px; }

.full-width { width: 100%; }
.clickable-row { cursor: pointer; }
.clickable-row:hover { background: rgba(0, 0, 0, 0.04); }

@media (max-width: $breakpoint-tablet) {
  .hide-tablet { display: none; }
}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/engagements/engagement-list/
git commit -m "feat: add engagement list with filters, status badges, and budget bars"
```

---

### Task 15: Engagement Detail Component

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-detail/engagement-detail.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-detail/engagement-detail.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-detail/engagement-detail.component.scss`

- [ ] **Step 1: Create EngagementDetailComponent (smart)**

`engagement-detail.component.ts`:
```typescript
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementDetail } from '../../../core/models';
import { EngagementFormDialogComponent } from '../engagement-form-dialog/engagement-form-dialog.component';
import { LogTimeDialogComponent } from '../../time-entries/log-time-dialog/log-time-dialog.component';

@Component({
  selector: 'app-engagement-detail',
  templateUrl: './engagement-detail.component.html',
  styleUrls: ['./engagement-detail.component.scss']
})
export class EngagementDetailComponent implements OnInit {
  engagement: EngagementDetail | null = null;
  isLoading = true;
  error: string | null = null;

  userColumns = ['userName', 'role', 'totalHours'];
  timeColumns = ['date', 'userName', 'hours', 'description'];

  get canEdit(): boolean {
    return this.authService.hasRole('Manager') || this.authService.hasRole('Partner');
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private engagementService: EngagementService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail(id);
  }

  openEditDialog(): void {
    if (!this.engagement) return;
    const dialogRef = this.dialog.open(EngagementFormDialogComponent, {
      width: '600px',
      data: { mode: 'edit', engagement: this.engagement }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDetail(this.engagement!.id);
    });
  }

  openLogTimeDialog(): void {
    if (!this.engagement) return;
    const dialogRef = this.dialog.open(LogTimeDialogComponent, {
      width: '500px',
      data: { engagementId: this.engagement.id, engagementTitle: this.engagement.title }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDetail(this.engagement!.id);
    });
  }

  onRetry(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail(id);
  }

  goBack(): void {
    this.router.navigate(['/engagements']);
  }

  private loadDetail(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.engagementService.getEngagementDetail(id).subscribe({
      next: data => {
        this.engagement = data;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load engagement details.';
        this.isLoading = false;
      }
    });
  }
}
```

`engagement-detail.component.html`:
```html
<div class="content-container">
  <button mat-button (click)="goBack()" class="back-button">
    <mat-icon>arrow_back</mat-icon> Back to Engagements
  </button>

  <app-loading-skeleton *ngIf="isLoading" type="cards" [rows]="3"></app-loading-skeleton>
  <app-error-state *ngIf="error" [message]="error" (retry)="onRetry()"></app-error-state>

  <ng-container *ngIf="engagement && !isLoading && !error">
    <!-- Header -->
    <div class="detail-header">
      <div>
        <h2 class="page-title">{{ engagement.title }}</h2>
        <p class="subtitle">{{ engagement.clientName }} &middot; {{ engagement.clientIndustry }}</p>
      </div>
      <div class="header-actions">
        <app-status-badge [status]="engagement.status"></app-status-badge>
        <span class="manager-label">Manager: {{ engagement.managerName }}</span>
        <button mat-stroked-button *ngIf="canEdit" (click)="openEditDialog()">
          <mat-icon>edit</mat-icon> Edit
        </button>
        <button mat-raised-button color="primary" (click)="openLogTimeDialog()">
          <mat-icon>add</mat-icon> Log Time
        </button>
      </div>
    </div>

    <!-- Budget Card -->
    <div class="card budget-card">
      <h3 class="section-title">Budget Overview</h3>
      <div class="budget-grid">
        <div>
          <span class="label">Hours</span>
          <p class="budget-value">{{ engagement.hoursLogged | number:'1.1-1' }} / {{ engagement.budgetHours }}</p>
        </div>
        <div>
          <span class="label">Budget</span>
          <p class="budget-value">{{ engagement.spentDollars | currency }} / {{ engagement.totalBudgetDollars | currency }}</p>
        </div>
        <div>
          <span class="label">Rate</span>
          <p class="budget-value">{{ engagement.hourlyRate | currency }}/hr</p>
        </div>
        <div>
          <span class="label">Status</span>
          <app-status-badge [status]="engagement.budgetStatus"></app-status-badge>
        </div>
      </div>
      <app-budget-progress-bar [utilization]="engagement.budgetUtilizationPercent"></app-budget-progress-bar>
    </div>

    <!-- Hours by User -->
    <div class="card">
      <h3 class="section-title">Hours by Team Member</h3>
      <table mat-table [dataSource]="engagement.hoursByUser" class="full-width">
        <ng-container matColumnDef="userName">
          <th mat-header-cell *matHeaderCellDef>Name</th>
          <td mat-cell *matCellDef="let u">{{ u.userName }}</td>
        </ng-container>
        <ng-container matColumnDef="role">
          <th mat-header-cell *matHeaderCellDef>Role</th>
          <td mat-cell *matCellDef="let u">{{ u.role }}</td>
        </ng-container>
        <ng-container matColumnDef="totalHours">
          <th mat-header-cell *matHeaderCellDef>Hours</th>
          <td mat-cell *matCellDef="let u">{{ u.totalHours | number:'1.1-1' }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="userColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: userColumns;"></tr>
      </table>
    </div>

    <!-- Recent Time Entries -->
    <div class="card">
      <h3 class="section-title">Recent Time Entries</h3>
      <app-empty-state *ngIf="engagement.recentTimeEntries.length === 0"
                       message="No time entries yet." icon="schedule"></app-empty-state>
      <table mat-table [dataSource]="engagement.recentTimeEntries"
             *ngIf="engagement.recentTimeEntries.length > 0" class="full-width">
        <ng-container matColumnDef="date">
          <th mat-header-cell *matHeaderCellDef>Date</th>
          <td mat-cell *matCellDef="let t">{{ t.date | date:'mediumDate' }}</td>
        </ng-container>
        <ng-container matColumnDef="userName">
          <th mat-header-cell *matHeaderCellDef>User</th>
          <td mat-cell *matCellDef="let t">{{ t.userName }}</td>
        </ng-container>
        <ng-container matColumnDef="hours">
          <th mat-header-cell *matHeaderCellDef>Hours</th>
          <td mat-cell *matCellDef="let t">{{ t.hours | number:'1.1-1' }}</td>
        </ng-container>
        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let t">{{ t.description }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="timeColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: timeColumns;"></tr>
      </table>
    </div>
  </ng-container>
</div>
```

`engagement-detail.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.back-button { margin-bottom: $space-sm; }
.subtitle { color: #666; margin-top: -8px; margin-bottom: $space-md; }

.detail-header {
  display: flex; justify-content: space-between; align-items: flex-start;
  margin-bottom: $space-lg; flex-wrap: wrap; gap: $space-md;
}

.header-actions {
  display: flex; align-items: center; gap: $space-sm; flex-wrap: wrap;
}

.manager-label { font-size: 14px; color: #666; }

.budget-card { margin-bottom: $space-lg; }

.budget-grid {
  display: grid; grid-template-columns: repeat(4, 1fr);
  gap: $space-md; margin-bottom: $space-md;
  @media (max-width: $breakpoint-tablet) { grid-template-columns: repeat(2, 1fr); }
}

.budget-value { font-size: 18px; font-weight: 600; color: $bdo-navy; margin: 4px 0 0; }

.card { margin-bottom: $space-md; }
.full-width { width: 100%; }
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/engagements/engagement-detail/
git commit -m "feat: add engagement detail with budget overview, team hours, and time entries"
```

---

### Task 16: Engagement Form Dialog

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-form-dialog/engagement-form-dialog.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-form-dialog/engagement-form-dialog.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagement-form-dialog/engagement-form-dialog.component.scss`

- [ ] **Step 1: Create EngagementFormDialogComponent**

`engagement-form-dialog.component.ts`:
```typescript
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EngagementService } from '../../../core/services/engagement.service';
import { EngagementDetail, EngagementStatus } from '../../../core/models';

export interface EngagementFormData {
  mode: 'create' | 'edit';
  clients?: { id: number; name: string }[];
  engagement?: EngagementDetail;
}

@Component({
  selector: 'app-engagement-form-dialog',
  templateUrl: './engagement-form-dialog.component.html',
  styleUrls: ['./engagement-form-dialog.component.scss']
})
export class EngagementFormDialogComponent {
  form: FormGroup;
  isSubmitting = false;
  statuses: EngagementStatus[] = ['Planning', 'Active', 'OnHold', 'Completed', 'Cancelled'];

  get isEdit(): boolean { return this.data.mode === 'edit'; }
  get title(): string { return this.isEdit ? 'Edit Engagement' : 'New Engagement'; }

  constructor(
    private fb: FormBuilder,
    private engagementService: EngagementService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<EngagementFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EngagementFormData
  ) {
    if (this.isEdit && data.engagement) {
      this.form = this.fb.group({
        title: [data.engagement.title, Validators.required],
        description: [data.engagement.description, Validators.required],
        status: [data.engagement.status, Validators.required],
        budgetHours: [data.engagement.budgetHours, [Validators.required, Validators.min(1)]],
        hourlyRate: [data.engagement.hourlyRate, [Validators.required, Validators.min(1)]],
        endDate: [data.engagement.endDate || '']
      });
    } else {
      this.form = this.fb.group({
        clientId: ['', Validators.required],
        title: ['', Validators.required],
        description: ['', Validators.required],
        budgetHours: ['', [Validators.required, Validators.min(1)]],
        hourlyRate: ['', [Validators.required, Validators.min(1)]],
        startDate: ['', Validators.required],
        endDate: ['']
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const value = this.form.value;

    if (this.isEdit && this.data.engagement) {
      this.engagementService.updateEngagement(this.data.engagement.id, {
        title: value.title,
        description: value.description,
        status: value.status,
        budgetHours: value.budgetHours,
        hourlyRate: value.hourlyRate,
        endDate: value.endDate || undefined
      }).subscribe({
        next: () => {
          this.snackBar.open('Engagement updated.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
          this.dialogRef.close(true);
        },
        error: () => { this.isSubmitting = false; }
      });
    } else {
      this.engagementService.createEngagement({
        clientId: value.clientId,
        title: value.title,
        description: value.description,
        budgetHours: value.budgetHours,
        hourlyRate: value.hourlyRate,
        startDate: value.startDate,
        endDate: value.endDate || undefined
      }).subscribe({
        next: () => {
          this.snackBar.open('Engagement created.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
          this.dialogRef.close(true);
        },
        error: () => { this.isSubmitting = false; }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
```

`engagement-form-dialog.component.html`:
```html
<h2 mat-dialog-title>{{ title }}</h2>
<mat-dialog-content>
  <form [formGroup]="form" class="form-grid">
    <!-- Create-only fields -->
    <mat-form-field appearance="outline" *ngIf="!isEdit" class="full-width">
      <mat-label>Client</mat-label>
      <mat-select formControlName="clientId">
        <mat-option *ngFor="let c of data.clients" [value]="c.id">{{ c.name }}</mat-option>
      </mat-select>
      <mat-error>Client is required</mat-error>
    </mat-form-field>

    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Title</mat-label>
      <input matInput formControlName="title">
      <mat-error>Title is required</mat-error>
    </mat-form-field>

    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Description</mat-label>
      <textarea matInput formControlName="description" rows="3"></textarea>
      <mat-error>Description is required</mat-error>
    </mat-form-field>

    <!-- Edit-only: Status -->
    <mat-form-field appearance="outline" *ngIf="isEdit" class="full-width">
      <mat-label>Status</mat-label>
      <mat-select formControlName="status">
        <mat-option *ngFor="let s of statuses" [value]="s">{{ s }}</mat-option>
      </mat-select>
    </mat-form-field>

    <div class="row">
      <mat-form-field appearance="outline">
        <mat-label>Budget Hours</mat-label>
        <input matInput type="number" formControlName="budgetHours">
        <mat-error>Must be at least 1</mat-error>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Hourly Rate ($)</mat-label>
        <input matInput type="number" formControlName="hourlyRate">
        <mat-error>Must be at least 1</mat-error>
      </mat-form-field>
    </div>

    <div class="row">
      <mat-form-field appearance="outline" *ngIf="!isEdit">
        <mat-label>Start Date</mat-label>
        <input matInput [matDatepicker]="startPicker" formControlName="startDate">
        <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
        <mat-datepicker #startPicker></mat-datepicker>
        <mat-error>Start date is required</mat-error>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>End Date (optional)</mat-label>
        <input matInput [matDatepicker]="endPicker" formControlName="endDate">
        <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
        <mat-datepicker #endPicker></mat-datepicker>
      </mat-form-field>
    </div>
  </form>
</mat-dialog-content>
<mat-dialog-actions align="end">
  <button mat-button (click)="onCancel()">Cancel</button>
  <button mat-raised-button color="primary" (click)="onSubmit()"
          [disabled]="form.invalid || isSubmitting">
    {{ isEdit ? 'Update' : 'Create' }}
  </button>
</mat-dialog-actions>
```

`engagement-form-dialog.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.form-grid { display: flex; flex-direction: column; min-width: 500px; }
.full-width { width: 100%; }
.row { display: flex; gap: $space-md; }
.row mat-form-field { flex: 1; }
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/engagements/engagement-form-dialog/
git commit -m "feat: add engagement create/edit dialog with reactive form"
```

---

### Task 17: Engagements Module Wiring

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagements-routing.module.ts`
- Create: `src/engagement-tracker-ui/src/app/features/engagements/engagements.module.ts`

- [ ] **Step 1: Create routing and module**

`engagements-routing.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EngagementListComponent } from './engagement-list/engagement-list.component';
import { EngagementDetailComponent } from './engagement-detail/engagement-detail.component';

const routes: Routes = [
  { path: '', component: EngagementListComponent },
  { path: ':id', component: EngagementDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EngagementsRoutingModule {}
```

`engagements.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { SharedModule } from '../../shared/shared.module';
import { EngagementsRoutingModule } from './engagements-routing.module';
import { EngagementListComponent } from './engagement-list/engagement-list.component';
import { EngagementDetailComponent } from './engagement-detail/engagement-detail.component';
import { EngagementFormDialogComponent } from './engagement-form-dialog/engagement-form-dialog.component';

@NgModule({
  declarations: [
    EngagementListComponent,
    EngagementDetailComponent,
    EngagementFormDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    EngagementsRoutingModule,
    SharedModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ]
})
export class EngagementsModule {}
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/engagements/engagements*.ts
git commit -m "feat: wire engagements feature module with list, detail, and form dialog"
```

---

## Chunk 6: Time Entries Feature & App Shell

### Task 18: Log Time Dialog

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/log-time-dialog/log-time-dialog.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/log-time-dialog/log-time-dialog.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/log-time-dialog/log-time-dialog.component.scss`

- [ ] **Step 1: Create LogTimeDialogComponent**

`log-time-dialog.component.ts`:
```typescript
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TimeEntryService } from '../../../core/services/time-entry.service';
import { EngagementService } from '../../../core/services/engagement.service';
import { EngagementSummary } from '../../../core/models';

export interface LogTimeData {
  engagementId?: number;
  engagementTitle?: string;
}

@Component({
  selector: 'app-log-time-dialog',
  templateUrl: './log-time-dialog.component.html',
  styleUrls: ['./log-time-dialog.component.scss']
})
export class LogTimeDialogComponent implements OnInit {
  form: FormGroup;
  isSubmitting = false;
  engagements: EngagementSummary[] = [];

  get hasPreselected(): boolean {
    return !!this.data?.engagementId;
  }

  constructor(
    private fb: FormBuilder,
    private timeEntryService: TimeEntryService,
    private engagementService: EngagementService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<LogTimeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LogTimeData
  ) {
    this.form = this.fb.group({
      engagementId: [data?.engagementId || '', Validators.required],
      hours: ['', [Validators.required, Validators.min(0.25), Validators.max(24)]],
      date: [new Date(), Validators.required],
      description: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (!this.hasPreselected) {
      this.engagementService.engagements$.subscribe(e => this.engagements = e);
      this.engagementService.loadEngagements();
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;
    const value = this.form.value;

    this.timeEntryService.createTimeEntry({
      engagementId: value.engagementId,
      hours: value.hours,
      date: value.date instanceof Date ? value.date.toISOString().split('T')[0] : value.date,
      description: value.description
    }).subscribe({
      next: () => {
        this.snackBar.open('Time entry logged.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
        this.dialogRef.close(true);
      },
      error: () => { this.isSubmitting = false; }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
```

`log-time-dialog.component.html`:
```html
<h2 mat-dialog-title>Log Time</h2>
<mat-dialog-content>
  <form [formGroup]="form" class="form-grid">
    <mat-form-field appearance="outline" class="full-width" *ngIf="!hasPreselected">
      <mat-label>Engagement</mat-label>
      <mat-select formControlName="engagementId">
        <mat-option *ngFor="let e of engagements" [value]="e.id">
          {{ e.clientName }} — {{ e.title }}
        </mat-option>
      </mat-select>
      <mat-error>Select an engagement</mat-error>
    </mat-form-field>

    <div *ngIf="hasPreselected" class="preselected">
      <span class="label">Engagement</span>
      <p>{{ data.engagementTitle }}</p>
    </div>

    <div class="row">
      <mat-form-field appearance="outline">
        <mat-label>Hours</mat-label>
        <input matInput type="number" formControlName="hours" step="0.25" min="0.25" max="24">
        <mat-error>0.25 - 24 hours</mat-error>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Date</mat-label>
        <input matInput [matDatepicker]="datePicker" formControlName="date">
        <mat-datepicker-toggle matSuffix [for]="datePicker"></mat-datepicker-toggle>
        <mat-datepicker #datePicker></mat-datepicker>
        <mat-error>Date is required</mat-error>
      </mat-form-field>
    </div>

    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Description</mat-label>
      <textarea matInput formControlName="description" rows="3"
                placeholder="What did you work on?"></textarea>
      <mat-error>Description is required</mat-error>
    </mat-form-field>
  </form>
</mat-dialog-content>
<mat-dialog-actions align="end">
  <button mat-button (click)="onCancel()">Cancel</button>
  <button mat-raised-button color="primary" (click)="onSubmit()"
          [disabled]="form.invalid || isSubmitting">Log Time</button>
</mat-dialog-actions>
```

`log-time-dialog.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.form-grid { display: flex; flex-direction: column; min-width: 400px; }
.full-width { width: 100%; }
.row { display: flex; gap: $space-md; }
.row mat-form-field { flex: 1; }
.preselected { margin-bottom: $space-md; p { font-weight: 500; color: $bdo-navy; } }
```

- [ ] **Step 2: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/time-entries/log-time-dialog/
git commit -m "feat: add log time dialog with engagement picker and validation"
```

---

### Task 19: Time Entry List & Module

**Files:**
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/time-entry-list/time-entry-list.component.ts`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/time-entry-list/time-entry-list.component.html`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/time-entry-list/time-entry-list.component.scss`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/time-entries-routing.module.ts`
- Create: `src/engagement-tracker-ui/src/app/features/time-entries/time-entries.module.ts`

- [ ] **Step 1: Create TimeEntryListComponent (smart)**

`time-entry-list.component.ts`:
```typescript
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimeEntryService } from '../../../core/services/time-entry.service';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementSummary } from '../../../core/models';
import { LogTimeDialogComponent } from '../log-time-dialog/log-time-dialog.component';

@Component({
  selector: 'app-time-entry-list',
  templateUrl: './time-entry-list.component.html',
  styleUrls: ['./time-entry-list.component.scss']
})
export class TimeEntryListComponent implements OnInit {
  timeEntries$ = this.timeEntryService.timeEntries$;
  loading$ = this.timeEntryService.loading$;
  engagements: EngagementSummary[] = [];
  selectedEngagementId: number | undefined;

  get displayedColumns(): string[] {
    const cols = ['date', 'engagementTitle'];
    if (this.authService.hasRole('Manager') || this.authService.hasRole('Partner')) {
      cols.push('userName');
    }
    cols.push('hours', 'description');
    return cols;
  }

  constructor(
    private timeEntryService: TimeEntryService,
    private engagementService: EngagementService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.timeEntryService.loadTimeEntries();
    this.engagementService.engagements$.subscribe(e => this.engagements = e);
    this.engagementService.loadEngagements();
  }

  onEngagementFilter(engagementId: number | undefined): void {
    this.selectedEngagementId = engagementId;
    this.timeEntryService.loadTimeEntries(engagementId);
  }

  openLogTimeDialog(): void {
    const dialogRef = this.dialog.open(LogTimeDialogComponent, {
      width: '500px',
      data: {}
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.timeEntryService.loadTimeEntries(this.selectedEngagementId);
    });
  }

  onRetry(): void {
    this.timeEntryService.loadTimeEntries(this.selectedEngagementId);
  }
}
```

`time-entry-list.component.html`:
```html
<div class="content-container">
  <div class="page-header">
    <h2 class="page-title">Time Entries</h2>
    <button mat-fab color="primary" (click)="openLogTimeDialog()" aria-label="Log time">
      <mat-icon>add</mat-icon>
    </button>
  </div>

  <!-- Filter -->
  <div class="filters">
    <mat-form-field appearance="outline" class="filter-field">
      <mat-label>Engagement</mat-label>
      <mat-select [value]="selectedEngagementId"
                  (selectionChange)="onEngagementFilter($event.value)">
        <mat-option [value]="undefined">All Engagements</mat-option>
        <mat-option *ngFor="let e of engagements" [value]="e.id">
          {{ e.clientName }} — {{ e.title }}
        </mat-option>
      </mat-select>
    </mat-form-field>
  </div>

  <app-loading-skeleton *ngIf="loading$ | async" type="table" [rows]="8"></app-loading-skeleton>

  <ng-container *ngIf="(timeEntries$ | async) as entries">
    <app-empty-state *ngIf="entries.length === 0 && !(loading$ | async)"
                     message="No time entries found." icon="schedule"></app-empty-state>

    <div class="card" *ngIf="entries.length > 0">
      <table mat-table [dataSource]="entries" class="full-width">
        <ng-container matColumnDef="date">
          <th mat-header-cell *matHeaderCellDef>Date</th>
          <td mat-cell *matCellDef="let t">{{ t.date | date:'mediumDate' }}</td>
        </ng-container>
        <ng-container matColumnDef="engagementTitle">
          <th mat-header-cell *matHeaderCellDef>Engagement</th>
          <td mat-cell *matCellDef="let t">{{ t.engagementTitle }}</td>
        </ng-container>
        <ng-container matColumnDef="userName">
          <th mat-header-cell *matHeaderCellDef>User</th>
          <td mat-cell *matCellDef="let t">{{ t.userName }}</td>
        </ng-container>
        <ng-container matColumnDef="hours">
          <th mat-header-cell *matHeaderCellDef>Hours</th>
          <td mat-cell *matCellDef="let t">{{ t.hours | number:'1.1-1' }}</td>
        </ng-container>
        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let t">{{ t.description }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
    </div>
  </ng-container>
</div>
```

`time-entry-list.component.scss`:
```scss
@use '../../../../theme/variables' as *;

.page-header { display: flex; justify-content: space-between; align-items: center; }
.filters { margin-bottom: $space-md; }
.filter-field { width: 350px; }
.full-width { width: 100%; }
```

- [ ] **Step 2: Create time-entries routing and module**

`time-entries-routing.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TimeEntryListComponent } from './time-entry-list/time-entry-list.component';

const routes: Routes = [{ path: '', component: TimeEntryListComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TimeEntriesRoutingModule {}
```

`time-entries.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { SharedModule } from '../../shared/shared.module';
import { TimeEntriesRoutingModule } from './time-entries-routing.module';
import { TimeEntryListComponent } from './time-entry-list/time-entry-list.component';
import { LogTimeDialogComponent } from './log-time-dialog/log-time-dialog.component';

@NgModule({
  declarations: [TimeEntryListComponent, LogTimeDialogComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TimeEntriesRoutingModule,
    SharedModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ]
})
export class TimeEntriesModule {}
```

- [ ] **Step 3: Commit**

```bash
git add src/engagement-tracker-ui/src/app/features/time-entries/
git commit -m "feat: add time entries list with engagement filter and log time FAB"
```

---

### Task 20: App Shell (Top Nav) & Root Wiring

**Files:**
- Modify: `src/engagement-tracker-ui/src/app/app.component.ts`
- Create: `src/engagement-tracker-ui/src/app/app.component.html`
- Create: `src/engagement-tracker-ui/src/app/app.component.scss`
- Modify: `src/engagement-tracker-ui/src/app/app-routing.module.ts`
- Modify: `src/engagement-tracker-ui/src/app/app.module.ts`

- [ ] **Step 1: Create AppComponent (top nav shell)**

`app.component.ts`:
```typescript
import { Component } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  onLogout(): void {
    this.authService.logout();
  }

  isLoginPage(): boolean {
    return this.router.url === '/login';
  }
}
```

`app.component.html`:
```html
<!-- Top Nav — hidden on login page -->
<mat-toolbar *ngIf="!isLoginPage() && (authService.isAuthenticated$ | async)" color="primary" class="top-nav">
  <span class="brand">Engagement Tracker</span>

  <nav class="nav-links">
    <a mat-button routerLink="/dashboard" routerLinkActive="active-link">
      <mat-icon>dashboard</mat-icon> Dashboard
    </a>
    <a mat-button routerLink="/engagements" routerLinkActive="active-link">
      <mat-icon>folder</mat-icon> Engagements
    </a>
    <a mat-button routerLink="/time-entries" routerLinkActive="active-link">
      <mat-icon>schedule</mat-icon> Time Entries
    </a>
  </nav>

  <span class="spacer"></span>

  <ng-container *ngIf="authService.currentUser$ | async as user">
    <span class="user-info">{{ user.firstName }} ({{ user.role }})</span>
    <button mat-icon-button (click)="onLogout()" aria-label="Logout">
      <mat-icon>logout</mat-icon>
    </button>
  </ng-container>
</mat-toolbar>

<router-outlet></router-outlet>
```

`app.component.scss`:
```scss
@use '../theme/variables' as *;

.top-nav {
  position: sticky;
  top: 0;
  z-index: 100;
}

.brand {
  font-weight: 700;
  font-size: 18px;
  margin-right: 32px;
}

.nav-links {
  display: flex;
  gap: 4px;

  a {
    color: rgba(255, 255, 255, 0.8);
    mat-icon { margin-right: 4px; font-size: 18px; height: 18px; width: 18px; }
  }

  .active-link {
    color: #fff;
    background: rgba(255, 255, 255, 0.1);
  }
}

.spacer { flex: 1; }

.user-info {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.7);
  margin-right: 8px;
}

@media (max-width: $breakpoint-tablet) {
  .nav-links a span { display: none; } // hide text, show only icons
  .user-info { display: none; }
  .brand { font-size: 14px; margin-right: 16px; }
}
```

- [ ] **Step 2: Update AppRoutingModule**

`app-routing.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
      },
      {
        path: 'engagements',
        loadChildren: () => import('./features/engagements/engagements.module').then(m => m.EngagementsModule)
      },
      {
        path: 'time-entries',
        loadChildren: () => import('./features/time-entries/time-entries.module').then(m => m.TimeEntriesModule)
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
```

- [ ] **Step 3: Update AppModule**

`app.module.ts`:
```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { AppRoutingModule } from './app-routing.module';
import { CoreModule } from './core/core.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
```

- [ ] **Step 4: Verify full build**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng build
```

- [ ] **Step 5: Commit**

```bash
git add src/engagement-tracker-ui/src/app/app.component* src/engagement-tracker-ui/src/app/app.module.ts src/engagement-tracker-ui/src/app/app-routing.module.ts
git commit -m "feat: add top nav shell, app routing with lazy-loaded features, and AppModule wiring"
```

---

### Task 21: Smoke Test

- [ ] **Step 1: Start backend**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker
dotnet run --project src/EngagementTracker.Api
```

- [ ] **Step 2: Start frontend**

```bash
cd /Users/jaredwaldroff/clawd/jobs/engagement-tracker/src/engagement-tracker-ui
ng serve
```

- [ ] **Step 3: Manual verification checklist**

Open `http://localhost:4200` and verify:
- [ ] Login page loads with BDO navy background
- [ ] Login with alice@example.com / password123 succeeds
- [ ] Dashboard shows stat cards, donut chart, top engagements
- [ ] Top nav shows "Alice (Associate)" and navigation links
- [ ] Engagements list shows filtered data, status badges, budget bars
- [ ] Click engagement row navigates to detail view
- [ ] Detail view shows budget overview, team hours, time entries
- [ ] Time Entries list shows entries, engagement filter works
- [ ] Log Time FAB opens dialog, submit creates entry
- [ ] Logout clears session, redirects to login

- [ ] **Step 4: Final commit**

```bash
git add src/engagement-tracker-ui/
git commit -m "feat: complete Phase 2 Angular frontend with all features wired"
```
