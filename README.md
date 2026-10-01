# AuraFit

AuraFit is a full-stack fitness tracking and planning application built with **ASP.NET Core Web API** and **Angular**.

It provides user authentication, fitness profile management, workout logging, workout planning, fitness calculations, user goals, and an AI chat feature.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Authentication](#authentication)
- [Database](#database)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Testing](#testing)
- [Current Status](#current-status)
- [Author](#author)

---

## Features

- User registration and login with JWT authentication
- User profile management
- BMI, BMR, and TDEE calculations
- Workout logging and tracking
- Exercise catalog
- Workout calorie estimation using exercise/MET data
- Weekly workout plans
- Daily workout management
- User goals and goal suggestions
- Weight and activity tracking
- AI chat integration
- RESTful Web API
- Unit tests for backend components

---

## Tech Stack

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / SQL Server LocalDB
- JWT Authentication
- Dependency Injection
- Serilog
- HttpClient

### Frontend

- Angular 19
- TypeScript
- Angular Material
- Angular CDK

### Testing

- xUnit
- MSTest
- Moq

---

## Architecture

AuraFit follows a layered application architecture.

```text
Angular Frontend
       |
       | HTTP
       v
ASP.NET Core Web API
       |
       +------------------+
       |                  |
   Controllers         Services
                          |
                     Repositories
                          |
                  Entity Framework Core
                          |
                     SQL Server
```

---

## Project Structure

```text
AuraFit
│
├── AuraFitWebService
│   ├── Controllers
│   ├── Services
│   └── Utilities
│
├── AuraFitDataAccessLayer
│   ├── Models
│   └── Repositories
│
├── AuraFitAPITestin
│
├── AuraFitDataAccessLayerTests
│
└── aurafit-frontend
```

---

## Authentication

AuraFit uses JWT-based authentication. The flow includes:

1. User registration
2. Password hashing
3. User login
4. JWT generation
5. JWT validation
6. Claims-based user identification
7. Authorization using `[Authorize]` on protected endpoints

---

## Database

AuraFit uses Microsoft SQL Server through Entity Framework Core. The default development configuration uses **SQL Server LocalDB**.

The data model includes entities for:

- Users
- User Profiles
- User Goals
- Weight Logs
- Activity Logs
- Exercises
- Workout Logs
- Workout Log Exercises
- Workout Templates
- Weekly Workout Plans
- Daily Workouts
- Meals
- Food Items

---

## API Endpoints

The backend exposes REST API endpoints for the application's major features.

| Method | Endpoint                        | Description                  |
| ------ | ------------------------------- | ---------------------------- |
| POST   | `/api/auth/register`            | Register a new user          |
| POST   | `/api/auth/login`               | Log in and receive a JWT     |
| GET    | `/api/userprofile/me`           | Get the current user profile |
| POST   | `/api/userprofile`              | Create a user profile        |
| GET    | `/api/exercisecatalog`          | Get the exercise catalog     |
| GET    | `/api/workoutlog`               | Get workout logs             |
| POST   | `/api/workoutlog`               | Create a workout log         |
| PUT    | `/api/workoutlog/{id}`          | Update a workout log         |
| DELETE | `/api/workoutlog/{id}`          | Delete a workout log         |
| POST   | `/api/workoutplan/create`       | Create a workout plan        |
| GET    | `/api/workoutplan/daily-plan`   | Get the daily workout plan   |
| GET    | `/api/usergoals/me`             | Get the current user's goals |
| POST   | `/api/usergoals`                | Create a user goal           |
| POST   | `/api/aurachat/ask`             | Ask the AI chat a question   |

---

## Getting Started

### Prerequisites

Make sure the following are installed:

- .NET SDK
- Node.js and npm
- Angular CLI
- SQL Server LocalDB or SQL Server
- Visual Studio or another suitable IDE

### Backend

1. Clone the repository.
2. Open the solution:

```text
   AuraFit.sln
```

3. Configure the database connection and required application settings.
4. Restore .NET dependencies:

```bash
   dotnet restore
```

5. Build the solution:

```bash
   dotnet build
```

6. Run the ASP.NET Core Web API project.

### Frontend

Navigate to the frontend directory:

```bash
cd aurafit-frontend
```

Install dependencies:

```bash
npm install
```

Run the Angular application:

```bash
ng serve
```

The frontend can then communicate with the running ASP.NET Core API.

---

## Testing

The repository contains backend test projects using xUnit, MSTest, and Moq.

Run the test suite with:

```bash
dotnet test
```

---

## Current Status

AuraFit is an actively developed full-stack fitness application. The repository contains the backend API, data access layer, Angular frontend, and automated tests.

Further development will focus on improving the application's architecture, security, documentation, and production readiness.

---

## Author

**Saurabh Nath Jha**
GitHub: [@saurabhnathjha](https://github.com/saurabhnathjha)