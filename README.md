# Recam

Recam is a platform designed to streamline collaboration between real estate agents and photography companies. The system allows photography companies to create property listing cases, upload images and videos, and assign them to agents for review. It helps automate media sharing and centralizes communication for property marketing.

- Backend: ASP.NET Core 9 Web API (`Remp.Solution`)
- Frontend: React 19 + TypeScript + Vite (`frontend`)
- Data: SQL Server (business data) + MongoDB (logging)
- Storage: Azure Blob Storage (media files)

## Tech Stack

- Backend: ASP.NET Core 9, C#, Entity Framework Core, ASP.NET Core Identity, JWT, FluentValidation, AutoMapper, Serilog, Swagger
- Frontend: React 19, TypeScript, Vite, React Router, Axios, Tailwind CSS, Zod
- Databases: SQL Server, MongoDB
- Cloud: Azure App Service, Azure Blob Storage, Azure SQL, Azure Cosmos DB, Azure Application Insights
- Testing: xUnit, Moq, FluentAssertions
- CI/CD Pipeline: GitHub Actions

## Main Features

- JWT authentication and role-based authorization (`PhotographyCompany`, `Agent`)
- Property listing case lifecycle management (create/update/delete)
- Agent management (create/search/assign/remove)
- Media upload/download and final media selection
- Email sending support
- Swagger API documentation in development environment

## Repository Structure

```text
Recam/
  frontend/                      # React + Vite app
  Remp.Solution/
    Remp.API/                    # Presentation layer. ASP.NET Core API entry point
    Remp.Service/                # Service Layer. Business logic
    Remp.Repository/             # Repository layer. Interact with database, retrive, create, update and deltete data in database.
    Remp.DataAccess/             # EF Core + MongoDB context + migrations
    Remp.Models/                 # Entitiy models
    Remp.Common/                 # Shared helper functions, exceptions define, unify responses Structure
    Remp.UnitTests/              # xUnit test project
```

## Prerequisites

- .NET SDK 9.0+
- Node.js 20+ and npm
- SQL Server (local or cloud)
- MongoDB (local or cloud)
- Azure Blob Storage account/container

## Environment Configuration

Backend reads settings from:

- `Remp.Solution/Remp.API/appsettings.json`
- `Remp.Solution/Remp.API/appsettings.Development.json`
- environment variables / user secrets (recommended for secrets)

Frontend expects a `.env` file in `frontend/` with:

```bash
VITE_BACKEND_URL=http://localhost:5153/api
VITE_GOOGLEMAPS_API_KEY=your_google_maps_api_key
```

## Backend Setup and Run

From repo root:

```bash
cd Remp.Solution
dotnet restore
dotnet build
```

Apply database migrations:

```bash
dotnet ef database update --project Remp.DataAccess --startup-project Remp.API
```

Run API:

```bash
dotnet run --project Remp.API
```

Default local URLs (from launch settings):

- `http://localhost:5153`

Swagger UI in development:

- `http://localhost:5153/swagger`

## Frontend Setup and Run

From repo root:

```bash
cd frontend
npm install
npm run dev
```

Default Vite URL:

- `http://localhost:5173`

## Running Tests

Backend unit tests:

```bash
cd Remp.Solution
dotnet test
```

Frontend lint:

```bash
cd frontend
npm run lint
```

## API Overview

Core controllers:

- `AuthController` (`/api/auth`): login, register
- `ListingCaseController` (`/api/listings`): listing CRUD, status updates
- `MediaController` (`/api/media`): media upload, delete and download
- `UserController` (`/api/user`, `/api/users/me`): agent and user operations

Use Swagger for full request/response schema and auth testing.

## Development Notes

- CORS allows `http://localhost:5173` by default.
- API validates request models with FluentValidation.
- Role seed logic runs on application startup.
