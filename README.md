# The Sunday League - Football League Management API

A comprehensive RESTful API for managing amateur football leagues, built with **ASP.NET Core 8** following **Clean Architecture** principles and secured with **JWT authentication**.

---

## About the Project

**The Sunday League (TSL)** is a backend system designed to manage complete football league operations including:

- **League & Season Management** – Create and manage multiple leagues with their respective seasons
- **Team Registration** – Register and track teams across different leagues and seasons
- **Match Scheduling** – Program fixtures with automatic standings calculation
- **Live Standings** – Real-time league tables with automatic updates after each match
- **Historical Data** – Track team performance across multiple seasons

The API supports **two user roles** with different access levels:
- 🟠 **Admin**: Manage leagues, seasons, teams, and matches
- 🟢 **User**: View public information and access premium features (historical stats)

---

## Authentication & Security

### JWT Token-Based Authentication

All administrative endpoints require authentication via **JWT Bearer tokens**.

**Public Access (No authentication required):**
- View leagues, seasons, teams, and matches
- View current standings and results
- User registration and login

**Authenticated Access (Login required):**
- View team historical statistics
- Change own password
- Logout (revoke tokens)

**Admin Access (Admin role required):**
- Create/Edit/Delete leagues, seasons, teams, and matches
- Register match results
- Inscribe teams to seasons
- Create other admin users

### Authentication Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/account/authenticate` | Login and obtain JWT token | Public |
| POST | `/api/v1/account/register` | Register new user account | Public |
| POST | `/api/v1/account/refresh-token` | Refresh expired token | Public |
| POST | `/api/v1/account/logout` | Revoke active tokens | Authenticated |
| POST | `/api/v1/account/change-password` | Change password | Authenticated |
| POST | `/api/v1/account/register-admin` | Create admin user | Admin only |

---

## API Endpoints Overview

### Leagues Management (`/api/v1/ligas`)

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/` | List all leagues | Public |
| GET | `/{id}` | Get league details | Public |
| POST | `/` | Create new league | Admin |
| PUT | `/{id}` | Update league | Admin |
| DELETE | `/{id}` | Delete league | Admin |
| PATCH | `/{id}/activar` | Activate league | Admin |
| PATCH | `/{id}/desactivar` | Deactivate league | Admin |

---

### Seasons Management (`/api/v1/temporadas`)

Each season includes:
- Name, start/end dates
- Associated league
- Active status
- Match count

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/` | List all seasons | Public |
| GET | `/{id}` | Get season details | Public |
| GET | `/liga/{ligaId}` | Get seasons by league | Public |
| GET | `/liga/{ligaId}/activa` | Get active season | Public |
| POST | `/` | Create new season | Admin |
| PUT | `/{id}` | Update season | Admin |
| DELETE | `/{id}` | Delete season | Admin |
| PATCH | `/{id}/finalizar` | Finalize season | Admin |

---

### Teams Management (`/api/v1/equipos`)

Each team includes:
- Name, city, badge URL
- Registration date
- Match count

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/` | List all teams | Public |
| GET | `/{id}` | Get team details | Public |
| POST | `/` | Create new team | Admin |
| PUT | `/{id}` | Update team | Admin |
| DELETE | `/{id}` | Delete team | Admin |

---

### Matches Management (`/api/v1/partidos`)

Each match includes:
- Home team, away team
- Match date and round number
- Score (goals home/away)
- Status: **Programado** (Scheduled) / **Jugado** (Played)

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/` | List all matches | Public |
| GET | `/{id}` | Get match details | Public |
| GET | `/jornada` | Get matches by round | Public |
| GET | `/proximos` | Get upcoming matches | Public |
| GET | `/resultados` | Get recent results | Public |
| POST | `/` | Schedule new match | Admin |
| PUT | `/{id}` | Update scheduled match | Admin |
| DELETE | `/{id}` | Delete match | Admin |
| PATCH | `/{id}/resultado` | Register match result | Admin |

**Automatic Features:**
- Standings update automatically when result is registered
- Match status changes from "Programado" to "Jugado"
- Points, goals, and stats calculated instantly

---

### Standings Management (`/api/v1/tablaposiciones`)

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/temporada/{temporadaId}` | Get standings table | Public |
| GET | `/existe/{temporadaId}` | Check if table exists | Public |
| POST | `/recalcular/{temporadaId}` | Recalculate table | Admin |

**Standings Criteria (in order):**
1. Points (3 for win, 1 for draw, 0 for loss)
2. Goal difference
3. Goals scored
4. Alphabetical order

---

### Team Positions (`/api/v1/posiciones`)

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/equipo/{equipoId}/tabla/{tablaId}` | Get team position | Public |
| GET | `/equipo/{equipoId}/temporada/{temporadaId}` | Get current position | Public |
| GET | `/equipo/{equipoId}/historial` | Get historical positions | **Authenticated**  |
| GET | `/existe` | Check position exists | Public |
| POST | `/` | Inscribe team to season | Admin |
| PATCH | `/{id}/reiniciar` | Reset team stats | Admin |
| DELETE | `/tabla/{tablaId}` | Remove all positions | Admin |


---

### Technologies Used

**Backend:**
- ASP.NET Core 8 (Web API)
- Entity Framework Core 8 (Code First)
- SQL Server
- ASP.NET Core Identity
- JWT Bearer Authentication

**Patterns & Practices:**
- Clean Architecture (Onion Architecture)
- Repository Pattern (Generic + Specific)
- Service Layer Pattern
- CQRS-inspired separation
- Dependency Injection
- AutoMapper for object mapping

**Security:**
- JWT token-based authentication
- Role-based authorization (`[Authorize(Roles = "Admin")]`)
- Refresh token support
- Password hashing (Identity default)
- Account lockout after failed attempts

**API Documentation:**
- Swagger/OpenAPI
- XML documentation comments
- Response type annotations

---

## Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **SQL Server** (2019 or later)
- **Visual Studio 2022** or **VS Code** or **Rider**

---

### Installation

1. **Clone the repository:**

```bash
   git clone https://github.com/yourusername/the-sunday-league.git
   cd the-sunday-league
```

2. **Configure database connection:**

   Edit `TSL.WebAPI/appsettings.json`:

```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=TSLDb;Integrated Security=true;TrustServerCertificate=true;",
       "IdentityConnection": "Server=.;Database=TSLIdentityDb;Integrated Security=true;TrustServerCertificate=true;"
     },
     "JWTSettings": {
       "Key": "YOUR-SECRET-KEY-AT-LEAST-32-CHARACTERS-LONG",
       "Issuer": "TSL.WebAPI",
       "Audience": "TSL.Client",
       "DurationInMinutes": 60
     }
   }
```

3. **Apply database migrations:**

```bash
   # Main database (Leagues, Teams, Matches)
   dotnet ef database update --project TSL.Infrastructure --startup-project TSL.WebAPI

   # Identity database (Users, Roles, Tokens)
   dotnet ef database update --project TSL.Infrastructure.Identity --startup-project TSL.WebAPI --context IdentityContext
```

4. **Run the application:**

```bash
   dotnet run --project TSL.WebAPI
```

---

## Frontend

This API is consumed by the TSL frontend, built with Angular 21 and Angular Material, providing a modern and minimalist interface for managing football leagues.

👉 [TSL Frontend](https://github.com/Jaqz23/TSL.WebApp)
