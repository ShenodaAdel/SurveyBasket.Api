# 📊 Survey Basket API

A robust and scalable **Survey Management System** built with **.NET 8** and **ASP.NET Core Web API**.
This project allows administrators to create and manage surveys, while users can participate and view results.
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen)
![Tests](https://img.shields.io/badge/Tests-xUnit-informational)

A robust and scalable **Survey Management System** built with **.NET 8** and **ASP.NET Core Web API**, following **Clean Architecture** principles.
Administrators create and manage polls, questions, and users; registered users participate in active polls and view live results.

---

## 📑 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Data Model](#-data-model)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Authentication & Authorization](#-authentication--authorization)
- [Cross-Cutting Concerns](#-cross-cutting-concerns)
- [Testing](#-testing)
- [Author](#-author)

---

## 🚀 Features

### 🔐 Authentication & Authorization
- JWT Authentication
- ASP.NET Core Identity
- Role-based Authorization (Admin / User)
- Account Registration & Login
- Email Confirmation (optional)
- Password Management
- JWT Bearer authentication with **refresh tokens** (rotate & revoke)
- ASP.NET Core Identity (users & roles)
- **Role-based** (Admin / User) **and permission-based** author
- Registration & login
- Email confirmation & resend confirmation
- Forgot / reset password & change password

### 👥 User Management (Admin)
- Create / Update Users
- Lock / Unlock Accounts
- Manage Roles
- Reset Password
### 👥 User & Role Management (Admin)
- Create / update users
- Lock / unlock accounts, enable / disable
- Manage roles & their permissions
- Assign roles to users

### 🗳 Poll Management
- Create, Update, Delete Polls
- Activate / Deactivate Polls
- Set Start & End Dates
- Send Notifications (optional extension)
- Create, update, delete polls
- Publish / unpublish (toggle publish status)
- Start & end dates with validation
- **API versioning** on the public poll endpoints (v1 & v2)

### ❓ Questions & Answers
- Add Questions to Poll
- Add Multiple Answers per Question
- Update / Delete Questions & Answers
- Add questions to a poll
- Multiple answers per question
- Update questions & toggle their status

### ✅ Voting System
- Users can participate in active polls
- Prevent duplicate voting
- Store vote history
- Track voting timestamp
- Users vote in active polls
- Prevents duplicate voting
- Stores vote history with timestamps

### 📊 Results
- View vote counts per answer
- Calculate percentage per answer
- Total participants count
### 📊 Results & Analytics
- Vote counts per answer
- Votes per day
- Votes per question

### ⚙️ Platform Features
- 📨 **Background jobs & notifications** (Hangfire) — daily "new polls" notification email
- 📧 **Email sending** (MailKit) with HTML templates
- 🚦 **Rate limiting** (per-IP, per-user, and concurrency)
- 🩺 **Health checks** (database, Hangfire, mail provider)
- 🧾 **Structured logging** (Serilog → console + rolling JSON files)
- ⚡ **Caching** (distributed memory cache)
- 📄 **Pagination, filtering & sorting** (dynamic LINQ)
- 🧯 **Global exception handling** (RFC 7807 ProblemDetails)
- 📘 **Swagger / OpenAPI** documentation with JWT support

---

## 🛠 Tech Stack

| Category | Technology | Version |
|---|---|---|
| Runtime | .NET / ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core (SQL Server) | 8.0.24 |
| Identity | Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.24 |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.24 |
| Validation | FluentValidation + SharpGrip AutoValidation | 12.1.1 / 2.0.0 |
| Mapping | Mapster + Mapster.DependencyInjection | 7.4.0 / 1.0
| Background Jobs | Hangfire (Core, AspNetCore, SqlServer) | 1.8.23 |
| Email | MailKit | 4.16.0 |
| Logging | Serilog.AspNetCore | 10.0.0 |
| API Versioning | Asp.Versioning | 8.1.1 |
| API Docs | Swashbuckle.AspNetCore (Swagger) | 6.6.2 |
| Health Checks | AspNetCore.HealthChecks (Hangfire / EF Core / UI) | 8.0.x |
| Dynamic Queries | System.Linq.Dynamic.Core | 1.7.2 |
| **Testing** | **xUnit · Moq · FluentAssertions** | **2.5.3 · 4.20.72 · 7.2.0** |

---

## 🏛 Architecture

The solution follows **Clean Architecture**, keeping business logic independent of frameworks and infrastructure. Dependencies always point **inward**:

```
        ┌─────────────────────────────────────────────┐
        │                 SurveyBasket.API             │  ← Con
        │            (Presentation / Web API)          │    Program.cs, Swagger, health
        └───────────────┬──────────────┬──────────────┘
                        │              │
                        ▼              ▼
        ┌───────────────────────┐  ┌──────────────────────────┐
        │ SurveyBasket.          │  │ SurveyBasket.            │
        │ Application            │  │ Infrastructure           │  ← EF Core, Identity,
        │ (Business logic,       │◄─┤ (Repositories, DbContext, │    JWT, repositories
        │  services, DTOs,       │  │  UnitOfWork, migrations)  │
        │  validators, mapping)  │  └──────────────┬───────────┘
        └───────────┬───────────┘                 │
                    │                              │
                    ▼                              ▼
        ┌─────────────────────────────────────────────┐
        │              SurveyBasket.Domain             │  ← Entities, BaseEntity
        │            (Enterprise / core model)         │    (no external dependencies)
        └─────────────────────────────────────────────┘
```

- **Domain** — pure entities and core model; depends on nothing (except Identity base types).
- **Application** — use cases / services, DTOs, FluentValidatio and **interfaces** (`IUnitOfWork`, `IPollRepository`, …). Depends
only on Domain.
- **Infrastructure** — EF Core `DbContext`, repository & `UnitOity & JWT setup. Implements the Application's interfaces.
- **API** — thin controllers, middleware pipeline, and the composition root (`Program.cs`).

> **Patterns used:** Repository + Unit of Work, Dependency Injection, standardized response envelope (`ApiResponse<T>`), options pattern, permission-based poli
cy provider.

---
e

```
SurveyBasket/
├── SurveyBasket.Domain/               # Core entities & domain model
│   ├── Common/BaseEntity.cs           #   Id, audit fields, soft-delete flag
│   └── Entities/                      #   Poll, Question, Answer, Vote, VoteAnswer,
│                                      #   ApplicationUser, ApplicationRole, RefreshToken
│
├── SurveyBasket.Application/          # Business logic (framework-independent)
│   ├── Services/                      #   Poll, Auth, Question, Vote, Result, User,
│   │                                  #   Role, Email, Notification, Caching
│   ├── RepositoriesInterfaces/        #   IPollRepository, IUserRepository, ...
│   ├── UnitOfWorkInterfaces/          #   IUnitOfWork
│   ├── Validations/                   #   FluentValidation validators
│   ├── Mapping/                       #   Mapster configurations
│   ├── Responses/                     #   ApiResponse<T> envelope
│   ├── Helpers/                       #   Permissions, DefaultRoles, PaginatedList, ...
│   └── DependencyInjection/           #   AddApplicationServices()
│
├── SurveyBasket.Infrastructure/      # EF Core, Identity, JWT,
│   ├── Persistence/                   #   ApplicationDbContext, Migrations, EntityConfigurations
│   ├── Repositories/                  #   PollRepository, UserRepository, RoleRepository, ...
│   ├── Identity/                      #   JWT provider & identity infrastructure
│   └── DependencyInjection/           #   AddInfrastructureServices()
│
├── SurveyBasket.API/                 # Presentation layer
│   ├── Controllers/                   #   Auth, Account, Polls, Question, Vote,
│   │                                  #   Result, Users, Roles
│   ├── Swagger/                       #   Versioned Swagger + JWT auth config
│   ├── Health/                        #   Custom health checks (mail provider)
│   ├── Middleware/                    #   GlobalExceptionHandler
│   ├── Extensions/                    #   UserExtension (GetUserId)
│   ├── Templates/                     #   Email HTML templates
│   └── Program.cs                     #   Middleware pipeline & DI composition root
│
└── SurveyBasket.Application.Tests/   # Unit tests (xUnit + Moq + FluentAssertions)
```

---

## 🗃 Data Model

| Entity | Description | Key Relationships |
|---|---|---|
| **Poll** | A survey | 1‑to‑many **Questions**, 1‑to‑many **Votes** |
| **Question** | A question in a poll | belongs to **Poll**, 1‑to‑many **Answers** |
| **Answer** | A possible answer | belongs to **Question** |
| **Vote** | A user's participation in a poll | belongs to **Poll** & **User**, 1‑to‑many **VoteAnswers** |
| **VoteAnswer** | A single selected answer within a vote | links **Vote** → **Question** → **Answer** |
| **ApplicationUser** | Identity user | 1‑to‑many **Votes** & **RefreshTokens** |
| **ApplicationRole** | Identity role | many‑to‑many with users |
| **RefreshToken** | JWT refresh token (owned) | belongs to **ApplicationUser** |

> Every entity derives from `BaseEntity`, which provides `Id`, audit fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`) and a **soft-delete** flag (`I
sDeleted`, `DeletedAt`).

---

## ⚡ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express, or full) — the default connection targets the local instance (`Server=.`)
- (Optional) An SMTP account for email — the project is pre-configured for [Ethereal](https://ethereal.email/) test inboxes

### 1. Clone
```bash
git clone <your-repo-url>
cd SurveyBasket.Api/SurveyBasket
```

### 2. Configure secrets
Sensitive values are **not** stored in `appsettings.json`. Set s://learn.microsoft.com/aspnet/core/security/app-secrets) (run insi
de `SurveyBasket.API`):

```bash
cd SurveyBasket.API

# JWT signing key (must be long & random — at least 32 characters)
dotnet user-secrets set "Jwt:Key" "THIS_IS_A_SUPER_SECRET_KEY_CHANGE_ME_32+"

# SMTP password for the mail account
dotnet user-secrets set "MailSettings:Password" "<your-smtp-password>"

# Hangfire dashboard credentials (protects the /jobs dashboard)
dotnet user-secrets set "HangfireSettings:Username" "admin"
dotnet user-secrets set "HangfireSettings:Password" "<dashboard-password>"
```

### 3. Create the database
Apply the EF Core migrations (run from the `SurveyBasket` solution folder):

```bash
dotnet ef database update --project SurveyBasket.Infrastructure --startup-project SurveyBasket.API
```

> Hangfire creates its own tables automatically in the `Hangfirt run.

### 4. Run
```bash
dotnet run --project SurveyBasket.API
```

Then browse to:
- **Swagger UI:** `https://localhost:<port>/swagger`
- **Hangfire dashboard:** `https://localhost:<port>/jobs`
- **Health checks:** `https://localhost:<port>/health`

### 5. Default admin account (seeded)
| Field | Value |
|---|---|
| Email | `admin@surveybasket.com` |
| Password | the seeded password (this project's default is typ it in production) |
| Role | `Admin` |

---

## 🔧 Configuration

Key sections in `appsettings.json` (secrets go in User Secrets — see above):

| Section | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | Main application data
| `ConnectionStrings:HangfireConnection` | Hangfire background-jobs database |
| `Jwt` | `Issuer`, `Audience`, `ExpiryMinutes` (+ `Key` from secrets) |
| `AllowedOrigins` | CORS whitelist (e.g. the Angular client at
| `MailSettings` | SMTP `Mail`, `DisplayName`, `Host`, `Port` (+ `Password` from secrets) |
| `HangfireSettings` | Dashboard basic-auth `Username` / `Password` |
| `Serilog` | Logging levels & sinks (console + rolling file) |

---

## 📡 API Endpoints

> Base URL: `https://localhost:<port>`. All protected endpointseader.

### 🔐 Auth — `/api/Auth`
| Method | Route | Description |
|---|---|---|
| POST | `/login` | Authenticate & receive JWT + refresh token
| POST | `/register` | Register a new account |
| POST | `/confirm-email` | Confirm email address |
| POST | `/resend-confirmation-email` | Resend the confirmation email |
| POST | `/forget-password` | Request a password reset |
| POST | `/reset-password` | Reset the password |
| POST | `/refresh` | Exchange a refresh token for a new JWT |
| PUT  | `/revoke-refresh-token` | Revoke a refresh token |

### 👤 Account — `/me` *(authenticated)*
| Method | Route | Description |
|---|---|---|
| GET | `/me/profile` | Get current user's profile |
| PUT | `/me/profile` | Update profile |
| POST | `/me/change-password` | Change password |

### 🗳 Polls — `/api/v{version}/Polls` *(v1 & v2)*
| Method | Route | Permission |
|---|---|---|
| GET | `/GetList` | `GetPolls` |
| GET | `/GetCurrentList` | `User` role (v1 & v2) |
| GET | `/GetById` | `GetPolls` |
| POST | `/Create` | `AddPolls` |
| PUT | `/Update` | `UpdatePolls` |
| DELETE | `/Delete` | `DeletePolls` |
| PUT | `/{id}/TogglePublishStatus` | `UpdatePolls` |

### ❓ Questions — `/api/Question`
| Method | Route | Permission |
|---|---|---|
| POST | `/` | `AddQuestions` |
| GET | `/` | `GetQuestions` |
| GET | `/GetByPollId` | `GetQuestions` |
| PUT | `/ToggleStatus` | `UpdateQuestions` |
| PUT | `/Update` | `UpdateQuestions` |

### ✅ Vote — `/api/Vote` *(User role)*
| Method | Route | Description |
|---|---|---|
| GET | `/GetListAvaibale/{pollId}` | Get available questions/answers to vote on |
| POST | `/{pollId}` | Submit a vote |

### 📊 Result — `/api/Result` *(`Results` permission)*
| Method | Route | Description |
|---|---|---|
| GET | `/{pollId}` | Poll results summary |
| GET | `/{pollId}/votes-per-day` | Votes grouped per day |
| GET | `/{pollId}/votes-per-question` | Votes grouped per question |

### 👥 Users — `/api/Users` *(admin permissions)*
| Method | Route | Permission |
|---|---|---|
| GET | `/` | `GetUsers` |
| GET | `/GetById/{id}` | `GetUsers` |
| POST | `/` | `AddUsers` |
| PUT | `/{id}` | `UpdateUsers` |
| PUT | `/{id}/toggle-status` | `UpdateUsers` |
| PUT | `/{id}/unlock` | `UpdateUsers` |

### 🎭 Roles — `/api/Roles` *(admin permissions)*
| Method | Route | Permission |
|---|---|---|
| GET | `/GetAll` | `GetRoles` |
| GET | `/GetDetailById/{id}` | `GetRoles` |
| POST | `/CreateRole` | `AddRoles` |
| PUT | `/UpdateRole/{id}` | `UpdateRoles` |
| PUT | `/ToggleRole/{id}` | `UpdateRoles` |

---

## 🔑 Authentication & Authorization

- **JWT Bearer** tokens signed with a symmetric key (`Jwt:Key`), validating issuer, audience, lifetime & signing key.
- **Refresh tokens** allow silent re-authentication and can be revoked.
- **Identity rules:** minimum password length 8, unique email required, email confirmation required before sign-in.
- **Permission-based authorization:** a custom `IAuthorizationPolicyProvider` + `PermissionAuthorizationHandler` enforce fine-grained permissions (e.g. `AddPol
ls`, `GetUsers`) via the `[HasPermission(...)]` attribute, in addition to the `Admin` / `User` roles.

To call protected endpoints in Swagger, click **Authorize** and paste your token.

---

## 🧩 Cross-Cutting Concerns

| Concern | Implementation |
|---|---|
| **Consistent responses** | All endpoints return an `ApiResponata`, `Messages`) |
| **Validation** | FluentValidation + automatic model validation (SharpGrip) |
| **Rate limiting** | Per-IP & per-user fixed windows (2 requests / 10s) + a concurrency limiter; returns `429 Too Many Requests` |
| **Background jobs** | Hangfire server + dashboard at `/jobs`; daily recurring "new polls" notification |
| **Health checks** | `/health` reports database, Hangfire & mail-provider status |
| **Logging** | Serilog → console and daily rolling JSON files under `Logs/` |
| **Caching** | `ICacheService` over a distributed memory cache |
| **Error handling** | Global exception handler returning RFC 7807 ProblemDetails |
| **Soft delete & auditing** | `BaseEntity` tracks who/when for create, update & delete |

---

## 🧪 Testing

Unit tests live in **`SurveyBasket.Application.Tests`** and use:

- **[xUnit](https://xunit.net/)** — test framework
- **[Moq](https://github.com/devlooped/moq)** — mocking dependencies (`IUnitOfWork`, repositories, …)
- **[FluentAssertions 7](https://fluentassertions.com/)** — expressive assertions *(pinned to the free 7.x line)*

Run all tests from the solution folder:

```bash
dotnet test
```

Run a single test class:

```bash
dotnet test --filter "FullyQualifiedName~PollRequestValidatorTests"
```

**Conventions followed:** Arrange–Act–Assert, `Method_ShouldResult_WhenCondition` naming, one behavior per test, and mocking interfaces so tests stay fast and
isolated (no database).

---

## 👤 Author

**Shenoda Adel**

> Built as a hands-on **.NET backend** project demonstrating Clean Architecture, secure authentication, and production-grade cross-cutting concerns.

---

⭐ If you find this project helpful, please consider giving it a star!
