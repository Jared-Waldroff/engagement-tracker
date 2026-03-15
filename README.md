# Engagement Tracker

A full-stack client engagement management portal where consulting associates log time, managers track budgets, and partners view firm-wide billing summaries. Built with **ASP.NET Core 8** and **Angular 21**.

---

## Features

- **Role-based access control** — three distinct roles (Associate, Manager, Partner) with JWT authentication and scoped data visibility
- **Engagement management** — create, update, and track client engagements with status lifecycle (Planning → Active → On Hold → Completed → Cancelled)
- **Time entry logging** — associates log hours against engagements; entries are filtered by role
- **Budget tracking** — real-time budget utilization with On Track / At Risk / Over Budget status indicators
- **Dashboard** — role-aware summary with stat cards, budget donut charts, and top engagements table
- **Structured logging** — Serilog with console and rolling file sinks
- **API documentation** — Swagger UI available in development mode

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core 8, C# 12 |
| **Frontend** | Angular 21, TypeScript 5.9, Angular Material |
| **Database** | SQLite (dev) with EF Core — swappable to MySQL/SQL Server |
| **Auth** | JWT Bearer tokens with BCrypt password hashing |
| **Validation** | FluentValidation |
| **Logging** | Serilog (structured, file + console) |
| **Testing** | xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing |

## Architecture

```
EngagementTracker.sln
├── src/
│   ├── EngagementTracker.Api          # Controllers, middleware, DI config
│   │   ├── Controllers/               # AuthController, EngagementsController, TimeEntriesController
│   │   ├── Extensions/                # Service registration, HttpContext claim helpers
│   │   └── Middleware/                # Global exception handler → consistent JSON errors
│   │
│   ├── EngagementTracker.Core         # Domain logic (no infrastructure dependencies)
│   │   ├── Dtos/                      # Request/response DTOs (never expose EF entities)
│   │   ├── Enums/                     # UserRole, EngagementStatus
│   │   ├── Exceptions/               # NotFoundException, ForbiddenException, ValidationException
│   │   ├── Interfaces/               # Service + repository contracts
│   │   ├── Services/                  # EngagementService, TimeEntryService, AuthService, BudgetCalculator
│   │   └── Validators/               # FluentValidation rules
│   │
│   ├── EngagementTracker.Infrastructure  # Data access layer
│   │   ├── Data/                      # AppDbContext, DbSeeder (realistic sample data)
│   │   ├── Entities/                  # EF Core entities (User, Client, Engagement, TimeEntry)
│   │   └── Repositories/             # Repository implementations
│   │
│   └── engagement-tracker-ui          # Angular SPA
│       └── src/app/
│           ├── core/                  # Services, guards, interceptors, models
│           ├── features/              # Auth, Dashboard, Engagements, Time Entries
│           └── shared/               # Reusable components (budget bar, status badge, loading skeleton)
│
└── tests/
    └── EngagementTracker.Tests
        ├── Unit/                      # Service + BudgetCalculator tests
        └── Integration/              # Full HTTP pipeline tests with TestWebApplicationFactory
```

The backend follows **Clean Architecture** — the Core project has zero infrastructure dependencies. All I/O flows through interfaces, wired up via dependency injection in `ServiceCollectionExtensions`.

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `/api/auth/login` | Public | Authenticate with email/password, returns JWT |
| `GET` | `/api/auth/profile` | Required | Current user profile |
| `GET` | `/api/engagements` | Required | List engagements (filtered by role) |
| `GET` | `/api/engagements/{id}` | Required | Engagement detail with time entries and budget breakdown |
| `POST` | `/api/engagements` | Manager+ | Create a new engagement |
| `PUT` | `/api/engagements/{id}` | Manager+ | Update an engagement |
| `GET` | `/api/engagements/dashboard` | Required | Role-aware dashboard statistics |
| `GET` | `/api/time-entries` | Required | List time entries (filtered by role, optional engagement filter) |
| `POST` | `/api/time-entries` | Required | Log a new time entry |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) and npm

### Run the API

```bash
cd src/EngagementTracker.Api
dotnet run
```

The API starts on `http://localhost:5062`. The database is automatically created and seeded with sample data on first run.

Swagger UI is available at `http://localhost:5062/swagger` in development mode.

### Run the Frontend

```bash
cd src/engagement-tracker-ui
npm install
npm start
```

The Angular app runs on `http://localhost:4200` and proxies API requests to the backend.

### Run Tests

```bash
dotnet test
```

Runs both unit tests (service logic, budget calculations) and integration tests (full HTTP pipeline with in-memory SQLite).

## Demo Credentials

The database seeder creates three users, one for each role:

| Role | Email | Password |
|------|-------|----------|
| Associate | `alice@example.com` | `password123` |
| Manager | `bob@example.com` | `password123` |
| Partner | `carol@example.com` | `password123` |

Each role sees different data:
- **Associates** see engagements they've logged time against
- **Managers** see engagements they manage, can create and edit engagements
- **Partners** see all engagements firm-wide

## License

This project was built as a portfolio demonstration.
