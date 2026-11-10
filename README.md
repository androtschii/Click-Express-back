# Click Express — Backend

REST API for **Click Express**, a web platform for a trucking and heavy-freight dispatch service that covers all 48 contiguous US states. Customers browse available loads, place and track orders, leave reviews and apply for driver jobs; administrators manage the catalogue, fleet, users and content.

Frontend: [Click-Express-front](https://github.com/androtschii/Click-Express-front) · Live demo: [my-app-alpha-eight-45.vercel.app](https://my-app-alpha-eight-45.vercel.app)

## Features

- **Authentication** — JWT access tokens with refresh tokens, BCrypt password hashing, password reset by e-mail, user and admin roles
- **Load catalogue** — products (freight loads), saved loads / favourites and a shopping cart
- **Orders** — checkout, order history and **real-time status updates** over SignalR (`/hubs/orders`)
- **Fleet and drivers** — vehicles, driver sign-up with document upload, job applications
- **Content** — news, customer reviews, contact leads, notifications
- **Admin** — dashboard with statistics, user management and an audit log of administrative actions
- **Operations** — health checks (`/health/live`, `/health/ready`), global error-handling middleware, Swagger UI

## Architecture

The solution follows a layered architecture:

| Project | Responsibility |
|---|---|
| `ClickExpress.Domain` | Entities and enums |
| `ClickExpress.DataAccess` | EF Core `DbContext`, migrations, repositories |
| `ClickExpress.BusinessLogic` | Services, DTOs, AutoMapper profiles, business rules |
| `ClickExpress.Api` | Controllers, SignalR hub, filters, middleware, FluentValidation validators |

## Tech stack

C# · .NET 8 · ASP.NET Core Web API · Entity Framework Core · SQL Server · SignalR · JWT · BCrypt · AutoMapper · FluentValidation · Swagger · xUnit · Moq

## Getting started

Requirements: .NET 8 SDK and SQL Server (LocalDB or Express).

```bash
cd Back-End/Back-End
dotnet restore
dotnet ef database update --project ClickExpress.DataAccess --startup-project ClickExpress.Api
dotnet run --project ClickExpress.Api
```

The connection string is `ConnectionStrings:DefaultConnection` in `ClickExpress.Api/appsettings.json`. Secrets (JWT key, SMTP password) should be set with `dotnet user-secrets` or environment variables, not committed.

API documentation is available at `/swagger` when the API runs in Development.

## Tests

```bash
cd Back-End/Back-End
dotnet test
```
