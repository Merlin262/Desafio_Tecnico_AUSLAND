# ProductsAPI

Full-stack product management application with user authentication, built with .NET 10 and Angular 20.

## Tech Stack

**Backend**
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + PostgreSQL 16
- Wolverine (CQRS / message bus)
- JWT Bearer authentication
- .NET Aspire (local orchestration)
- Swagger / OpenAPI

**Frontend**
- Angular 20 (standalone components, Signals)
- TypeScript 5.9 / RxJS 7.8
- Reactive Forms + Route Guards

---

## Architecture

```
ProductsAPI/
├── ProductsAPI.API/             # Web API entry point (controllers, middleware)
├── ProductsAPI.Application/     # Business logic (CQRS handlers, DTOs, auth)
├── ProductsAPI.Domain/          # Entities (Product, User), shared models
├── ProductsAPI.Infrastructure/  # EF Core context, repositories, migrations
├── ProductsAPI.ServiceDefaults/ # OpenTelemetry + service configuration
├── ProductsAPI.AppHost/         # Aspire orchestration
└── products-app/                # Angular frontend
```

The backend follows **Clean Architecture** with CQRS via Wolverine. The frontend uses a **feature-based structure** with standalone Angular components.

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running (required by Aspire to spin up PostgreSQL)
- .NET 10 SDK
- Node.js (LTS)

```bash
# Install frontend dependencies before the first run
cd products-app
npm install
```

### Option 1 — .NET Aspire (recommended)

> **Before running**, create `ProductsAPI.AppHost/appsettings.json`:
>
> The `DefaultConnection` value can be copied directly from the Aspire Dashboard after the first startup (go to the **postgres** resource → **Connection strings**).
>
> ```json
> {
>   "ConnectionStrings": {
>     "DefaultConnection": ""
>   },
>   "Jwt": {
>     "Key": "<at-least-32-character-secret>",
>     "Issuer": "ProductsAPI",
>     "Audience": "ProductsApp",
>     "ExpiresInMinutes": 120
>   },
>   "Logging": {
>     "LogLevel": {
>       "Default": "Information",
>       "Microsoft.AspNetCore": "Warning"
>     }
>   },
>   "AllowedHosts": "*",
>   "Cors": {
>     "AllowedOrigins": [
>       "http://localhost:4200"
>     ]
>   }
> }
> ```

```bash
cd ProductsAPI.AppHost
dotnet run
```

Aspire launches the Aspire Dashboard, PostgreSQL, the API, and the Angular dev server with automatic port allocation and health checks. Migrations run automatically on API startup.

---

### Option 2 — Manual

**Backend**

> **Before running**, create `ProductsAPI/appsettings.json`:

```bash
cd ProductsAPI
dotnet restore
dotnet run
```

**Frontend**

```bash
cd products-app
npm install
npm start
```

---

## API Endpoints

### Authentication

| Method | Endpoint             | Description       |
|--------|----------------------|-------------------|
| POST   | /api/auth/register   | Create account    |
| POST   | /api/auth/login      | Authenticate      |

Both endpoints return `{ token, username }` on success.

**Default in-memory user** (available without database registration):

| Field    | Value    |
|----------|----------|
| username | `admin`  |
| password | `123456` |

Example login request:
```json
POST /api/auth/login
{
  "username": "admin",
  "password": "123456"
}
```

### Products

All product endpoints require `Authorization: Bearer <token>`.

| Method | Endpoint              | Description              |
|--------|-----------------------|--------------------------|
| GET    | /api/products         | List products            |
| GET    | /api/products/{id}    | Get product by ID        |
| POST   | /api/products         | Create product           |
| PUT    | /api/products/{id}    | Update product           |
| DELETE | /api/products/{id}    | Delete product           |

**Pagination query params**: `pageNumber` (default: 1), `pageSize` (default: 10)

**GET /api/products response:**
```json
{
  "items": [...],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10
}
```

Full API documentation available at `http://localhost:5021/swagger` when running locally.

---

## Database

PostgreSQL 16 with EF Core migrations.

| Table    | Key Columns                                                |
|----------|------------------------------------------------------------|
| Products | id, name, description, price, stock, is_deleted, created_at |
| Users    | id, username, password_hash, created_at                   |

- Products use **soft delete** — deleted records are filtered automatically via a global EF query filter.
- Passwords are hashed with SHA256 + random 16-byte salt (`salt.hash` Base64 format).

---

## Unit Tests

The `ProductAPI.UnitTests` project covers handlers, and repositories using **xUnit**, **Moq**, and **EF Core InMemory**.

### Run

```bash
dotnet test ProductAPI.UnitTests/ProductAPI.UnitTests.csproj
```

### Coverage

| Layer | Classes tested | Strategy |
|---|---|---|
| **Handlers** | `CreateProduct`, `UpdateProduct`, `DeleteProduct`, `GetAllProducts`, `GetProductById`, `Login`, `Register` | Moq (`IProductRepository`, `IUserRepository`, `JwtTokenService`) |
| **Repositories** | `ProductRepository`, `UserRepository` | EF Core InMemory provider — real `AppDbContext` |

---

## Features

- User registration and login with JWT
- Product CRUD with pagination
- Soft delete with automatic query filtering
- Angular route guards (authenticated routes)
- JWT interceptor (auto-attaches token, handles 401)
- Form validation (required fields, password confirmation, strength rules)
- Health checks (`/health`, `/alive`)
- OpenTelemetry observability
