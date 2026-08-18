# EventTicketingApp

A full-stack event ticketing platform (browse events, book tickets, receive QR-coded e-tickets) built with a **.NET Core Web API** backend and a **React + TypeScript** frontend, containerized with Docker.

## Architecture Overview

```
┌─────────────────────┐        REST API (JWT)        ┌──────────────────────┐
│  React + TypeScript  │ ───────────────────────────▶ │   .NET Core Web API   │
│  (Vite, port 5173)   │ ◀─────────────────────────── │   (Docker, port 8080) │
└─────────────────────┘                               └───────────┬──────────┘
                                                                    │
                                                                    ▼
                                                        ┌──────────────────────┐
                                                        │   PostgreSQL (Docker) │
                                                        └──────────────────────┘
```

The backend and frontend are **separate, independently deployable projects** in the same solution/repo — not a bundled template. This keeps the API reusable (a future mobile app could call the same endpoints) and lets each side use its own tooling (`dotnet` for the API, `npm`/Vite for the client).

## Repository Structure

```
EventTicketingApp/                    ← solution root
├── EventTicketingApp/                ← .NET Core Web API project
│   ├── Controllers/
│   │   ├── AuthController.cs         ← register/login, issues JWT
│   │   ├── EventsController.cs       ← browse events
│   │   ├── OrdersController.cs       ← create/view orders (protected)
│   │   └── TicketsController.cs      ← QR code + check-in
│   ├── Models/                       ← EF Core entities (Event, Venue, TicketType, Order, OrderItem, Ticket, ApplicationUser)
│   ├── DTOs/                         ← API request/response contracts (never expose raw entities)
│   ├── Services/
│   │   ├── Interfaces/               ← IEventService, IOrderService, ITokenService
│   │   ├── EventService.cs
│   │   ├── OrderService.cs
│   │   └── TokenService.cs           ← JWT generation
│   ├── Data/
│   │   ├── ApplicationDbContext.cs   ← EF Core DbContext, relationship config
│   │   └── Migrations/               ← auto-generated, don't hand-edit
│   ├── Helpers/
│   │   └── QrCodeGenerator.cs        ← generates ticket QR codes (QRCoder)
│   ├── Dockerfile
│   └── Program.cs
│
├── EventTicketingApp.Client/         ← React + TypeScript (Vite) project
│   └── ticketing-client/
│       └── src/
│           ├── pages/                ← EventList, EventDetail, etc.
│           ├── services/             ← api.ts (fetch wrapper with JWT auth header)
│           ├── context/              ← AuthContext (login/register/logout)
│           └── types/                ← TypeScript interfaces mirroring backend DTOs
│
├── docker-compose.yml                ← orchestrates API + Postgres containers
├── .env                              ← secrets (JWT key, DB password) — gitignored
├── .gitignore
└── EventTicketingApp.sln
```

## Backend — .NET Core Web API

**Layered design:**
```
Controllers (HTTP endpoints)
      ↓
Services (business logic, interfaces for testability)
      ↓
DbContext / EF Core (PostgreSQL via Npgsql)
```

- **Database:** PostgreSQL, accessed via EF Core with the `Npgsql.EntityFrameworkCore.PostgreSQL` provider
- **Auth:** ASP.NET Identity for user management + JWT Bearer tokens for stateless API authentication (no server-side session — required since the frontend is a separate origin)
- **Concurrency safety:** `TicketType` uses an optimistic concurrency token (`Version`, Postgres-compatible — not SQL Server's `[Timestamp]`) to prevent overselling tickets when multiple users book simultaneously
- **DTOs, not raw entities:** every API response is explicitly projected into a DTO, so EF Core navigation properties and internal fields never leak to the client
- **CORS:** configured to allow the Vite dev server origin (`http://localhost:5173`) to call the API

## Frontend — React + TypeScript

- **Vite** as the build tool/dev server (not Create React App)
- **TypeScript interfaces** mirror the backend DTOs field-for-field, keeping the API contract explicit on both ends
- **JWT stored client-side** (localStorage) and attached as an `Authorization: Bearer` header on authenticated requests
- **React Router** for page navigation (event list → event detail/booking → order confirmation)

## Containerization

- The **API** runs in its own Docker container (built from its `Dockerfile`)
- **PostgreSQL** runs in a separate container
- **Docker Compose** orchestrates both together on a shared network — containers reach each other by service name (e.g. `Host=db`, not `localhost`), avoiding the "which localhost" confusion that comes from running multiple standalone containers
- The **React app** currently runs locally via `npm run dev` during development (fastest feedback loop); it can be added to the Compose stack later once the app is closer to deployment

## Secrets Management

Sensitive values (JWT signing key, database password) are **never committed to source control**:

| Environment | Where secrets live |
|---|---|
| Local `dotnet run` (no Docker) | `dotnet user-secrets` |
| Docker Compose | `.env` file at the solution root, referenced via `${VARIABLE}` in `docker-compose.yml` |
| Production | Platform secret manager (Azure Key Vault, AWS Secrets Manager, etc.) |

`.env` is listed in `.gitignore` to prevent accidental commits.

## Core Domain Flow

1. **Organizer** creates an `Event`, with one or more `TicketType`s (e.g. General, VIP) each with a price and available quantity
2. **Customer** browses published events (`GET /api/events`), selects ticket quantities, and submits an order (`POST /api/orders`, requires auth)
3. The order service validates ticket availability inside a **database transaction**, decrements `QuantitySold`, and generates one `Ticket` (with a unique `Guid` code) per unit purchased
4. Each `Ticket`'s code is rendered as a **QR code** (via the `QrCodeGenerator` helper) for entry validation
5. At the venue, a scan endpoint (`POST /api/tickets/checkin/{ticketCode}`) marks the ticket as checked in — restricted to Organizer/Admin roles

## Getting Started

```bash
# Clone and enter the repo
git clone <repo-url>
cd EventTicketingApp

# Set up secrets
dotnet user-secrets init --project EventTicketingApp
dotnet user-secrets set "Jwt:Key" "<generated-32+-char-secret>" --project EventTicketingApp
# ...or create a .env file for the Docker Compose path

# Run backend + database via Docker
docker compose up --build

# Run frontend
cd EventTicketingApp.Client/ticketing-client
npm install
npm run dev
```

## Roadmap / Not Yet Implemented

- Organizer/Admin controllers (create/edit events and ticket types)
- Payment gateway integration (Stripe) — orders currently go straight to `Pending` without a payment step
- Ticket-hold/reservation timeout during checkout
- Input validation attributes on DTOs
- Global exception-handling middleware
- Seed data for local development
