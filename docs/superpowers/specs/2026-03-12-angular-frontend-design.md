# Phase 2: Angular Frontend — Design Spec

**Project:** BDO Engagement Tracker
**Date:** 2026-03-12
**Status:** Approved

---

## Overview

Angular frontend for the Engagement Tracker backend (ASP.NET Core 8). Provides login, dashboard, engagement management, and time entry logging. Consumes 9 REST endpoints with JWT authentication. Role-based UI visibility (Associate, Manager, Partner) with server-side data filtering.

## Key Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Module system | NgModules with lazy loading | Enterprise familiarity for BDO |
| App shell layout | Top navigation bar | Only 3 nav items; maximizes content width |
| State management | BehaviorSubject services | Right-sized for ~10 API calls; no NgRx boilerplate |
| UI library | Angular Material | BDO navy custom theme |
| Responsive targets | Desktop + tablet | Consulting firms use laptops and tablets |

## Project Structure

```
src/engagement-tracker-ui/
  src/app/
    core/                              (singleton — imported once in AppModule)
      services/
        auth.service.ts
        engagement.service.ts
        time-entry.service.ts
      guards/
        auth.guard.ts
      interceptors/
        jwt.interceptor.ts
        error.interceptor.ts
      models/
        user.model.ts
        auth.model.ts
        engagement.model.ts
        dashboard.model.ts
        time-entry.model.ts
        api-error.model.ts
      core.module.ts
    features/                          (each lazy-loaded via router)
      auth/
        login/
        auth.module.ts
        auth-routing.module.ts
      dashboard/
        dashboard/
        widgets/
          stat-card/
          budget-donut/
          top-engagements-table/
        dashboard.module.ts
        dashboard-routing.module.ts
      engagements/
        engagement-list/
        engagement-detail/
        engagement-form-dialog/
        engagements.module.ts
        engagements-routing.module.ts
      time-entries/
        time-entry-list/
        log-time-dialog/
        time-entries.module.ts
        time-entries-routing.module.ts
    shared/                            (imported by each feature module)
      components/
        status-badge/
        budget-progress-bar/
        loading-skeleton/
        empty-state/
        error-state/
      pipes/
        hours.pipe.ts
      shared.module.ts
    app.component.ts                   (top nav shell)
    app.module.ts
    app-routing.module.ts
  src/environments/
    environment.ts                     (apiUrl: 'http://localhost:5000')
    environment.prod.ts                (apiUrl: '/api')
  src/styles.scss                      (Angular Material BDO theme)
  src/theme/
    _variables.scss
    _typography.scss
```

## Routing

```typescript
const routes: Routes = [
  { path: 'login', loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule) },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule) },
      { path: 'engagements', loadChildren: () => import('./features/engagements/engagements.module').then(m => m.EngagementsModule) },
      { path: 'time-entries', loadChildren: () => import('./features/time-entries/time-entries.module').then(m => m.TimeEntriesModule) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
```

## Authentication Flow

1. Login: POST `/api/auth/login` returns JWT + user profile (`LoginResponseDto`)
2. JWT stored in `localStorage`; user profile (from response's `User` field) stored in `localStorage` as JSON
3. `JwtInterceptor` attaches `Authorization: Bearer <token>` to all outgoing requests
4. `ErrorInterceptor` catches 401 (clear token, redirect to `/login`), 403 (snackbar), 5xx (snackbar)
5. `AuthGuard` checks `AuthService.isAuthenticated$`, redirects unauthenticated to `/login`
6. On app startup: `AuthService` checks `localStorage` for existing token and stored user profile JSON. If both exist, restores the `BehaviorSubject` from the stored profile (no HTTP call). If the token exists but profile is missing, calls `GET /api/auth/profile` to fetch it. If no token, user is unauthenticated.
7. Role checks for UI visibility (e.g., hiding "New Engagement" for Associates) use `AuthService.hasRole()` in components — no route-level `RoleGuard` needed since the API enforces access server-side.

## Services

### AuthService

```
State:
  currentUserSubject: BehaviorSubject<UserProfile | null>
  currentUser$: Observable<UserProfile | null>
  isAuthenticated$: Observable<boolean>

Methods:
  login(email, password): Observable<LoginResponse>
  getProfile(): Observable<UserProfile>    (calls GET /api/auth/profile — fallback for startup)
  logout(): void
  getCurrentUser(): UserProfile | null
  hasRole(role): boolean
```

### EngagementService

```
State:
  engagementsSubject: BehaviorSubject<EngagementSummary[]>
  loadingSubject: BehaviorSubject<boolean>
  engagements$, loading$: Observables

Methods:
  loadEngagements(filters?): void          (fetches + pushes to subject)
  getEngagementDetail(id): Observable<EngagementDetail>
  getDashboard(): Observable<Dashboard>
  createEngagement(dto): Observable<EngagementSummary>
  updateEngagement(id, dto): Observable<EngagementSummary>
```

### TimeEntryService

```
State:
  timeEntriesSubject: BehaviorSubject<TimeEntry[]>
  loadingSubject: BehaviorSubject<boolean>
  timeEntries$, loading$: Observables

Methods:
  loadTimeEntries(engagementId?): void
  createTimeEntry(dto): Observable<TimeEntry>
```

Pattern: `loadX()` methods are void — trigger HTTP call, push results into subject. After successful create, re-call `loadX()` to refresh the list.

## Screens

### Login (`/login`)
- Centered Material card on navy background
- Reactive form: email + password, "Sign In" button
- Validation: required fields, email format
- Error display for 401

### Dashboard (`/dashboard`)
- Data: `GET /api/engagements/dashboard` returning `DashboardDto`
- Top row: 4 stat cards mapping to `DashboardDto` fields:
  - "Total Engagements" → `TotalEngagements`
  - "Active" → `ActiveEngagements`
  - "Total Hours" → `TotalHoursLogged` / `TotalBudgetHours`
  - "Utilization" → `OverallUtilizationPercent`
- Middle: Budget donut chart (pure CSS) using `EngagementsOnTrack`, `EngagementsAtRisk`, `EngagementsOverBudget` as segments
- Bottom: Top engagements table with status badges and utilization bars
- Row click navigates to `/engagements/:id`

### Engagement List (`/engagements`)
- Data: `GET /api/engagements` with query params
- Toolbar: status mat-select filter + debounced search input (300ms). Date range filters (`StartDateFrom`/`StartDateTo`) are supported by the API but intentionally omitted from the UI to keep the interface simple for this portfolio project.
- mat-table: Client, Title, Status badge, Budget progress bar, Manager, Start Date
- "New Engagement" button visible for Manager/Partner only
- Row click navigates to `/engagements/:id`
- Create/Edit: MatDialog with reactive form. **Create mode:** client dropdown (populated from unique clients extracted from the loaded engagements list — avoids needing a separate `/api/clients` endpoint), title, description, budget hours, hourly rate, start date, optional end date. All fields required except end date. **Edit mode:** title, description, status dropdown (Planning/Active/OnHold/Completed/Cancelled), budget hours, hourly rate, optional end date. Client and start date are read-only/hidden (not in `UpdateEngagementDto`). Status field only appears in edit mode.

### Engagement Detail (`/engagements/:id`)
- Data: `GET /api/engagements/:id` returning `EngagementDetailDto`
- Header: title, client name, client industry, status badge, manager
- Budget card: progress bar, hours logged/budget, dollar amounts, utilization %
- Hours by user table (from `HoursByUser`)
- Recent time entries table (from `RecentTimeEntries`)
- "Log Time" button opens LogTimeDialog

### Time Entry List (`/time-entries`)
- Data: `GET /api/time-entries` with optional `?engagementId=`
- Toolbar: engagement filter mat-select
- mat-table: Date, Engagement, User (shown for Manager/Partner roles, hidden for Associate), Hours, Description
- "Log Time" FAB opens LogTimeDialog

## Shared Components

| Component | Inputs/Outputs |
|---|---|
| `StatusBadgeComponent` | `@Input() status: string` — Planning=gray, Active=blue, OnHold=amber, Completed=green, Cancelled=red |
| `BudgetProgressBarComponent` | `@Input() utilization: number` — green <80%, amber 80-99%, red 100%+ |
| `LoadingSkeletonComponent` | `@Input() rows: number`, `@Input() type: 'table' \| 'cards'` |
| `EmptyStateComponent` | `@Input() message`, `@Input() icon` |
| `ErrorStateComponent` | `@Input() message`, `@Output() retry` |
| `HoursPipe` | Formats decimal hours: `7.5` → `7h 30m` |

## Theme

```scss
$bdo-navy:    #1a2744;    // primary
$bdo-blue:    #5b9bd5;    // accent
$bdo-white:   #ffffff;    // card backgrounds
$bdo-gray:    #f5f6fa;    // page background
$bdo-text:    #333333;    // body text
$bdo-danger:  #c0392b;    // OverBudget, errors
$bdo-warning: #f39c12;    // AtRisk
$bdo-success: #27ae60;    // OnTrack
```

- Angular Material custom palette: navy primary, blue accent, danger warn
- Roboto font (Material default)
- 4px/8px spacing grid

## UX States

Every data-loading component implements all four states:
1. **Loading** → `LoadingSkeletonComponent` (animated gray bars)
2. **Empty** → `EmptyStateComponent` (icon + message + optional action)
3. **Error** → `ErrorStateComponent` (message + Retry button)
4. **Success** → rendered data

## Responsive Behavior

- **Desktop (>1024px):** Full top nav, 4-column stat cards, full table columns
- **Tablet (768-1024px):** Hamburger menu, 2-column stat cards, table hides Manager/Start Date columns
- No mobile target

## Accessibility

- Keyboard navigation: all interactive elements Tab-reachable, Enter/Space activation
- Angular Material ARIA roles on mat-table, mat-select, mat-dialog
- Color contrast: navy on white, white on navy
- Status badges use text labels alongside color

## Notifications

- Success snackbars: "Engagement created", "Time entry logged" — auto-dismiss 3s
- Error snackbars: from ErrorInterceptor — dismiss on click
- Via `MatSnackBar`

## API Endpoints Consumed

| Method | Endpoint | Used By |
|---|---|---|
| POST | `/api/auth/login` | LoginComponent |
| GET | `/api/auth/profile` | AuthService (startup) |
| GET | `/api/engagements/dashboard` | DashboardComponent |
| GET | `/api/engagements` | EngagementListComponent |
| GET | `/api/engagements/:id` | EngagementDetailComponent |
| POST | `/api/engagements` | EngagementFormDialog |
| PUT | `/api/engagements/:id` | EngagementFormDialog |
| GET | `/api/time-entries` | TimeEntryListComponent |
| POST | `/api/time-entries` | LogTimeDialog |
