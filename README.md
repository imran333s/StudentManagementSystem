# Enterprise Student Management System - ASP.NET Core Web API (.NET 8)

A production-quality, enterprise-grade ASP.NET Core Web API built with Clean Architecture principles, demonstrating robust Layered Architecture, Repository and Service Patterns, Entity Framework Core Code-First, JWT Authentication, Serilog Logging, Centralized Exception Handling, and comprehensive Swagger documentation.

---

## 🏗️ Complete Solution Architecture

The solution implements **Clean Architecture** with a strict dependency flow from outer infrastructure and presentation layers inward to the core domain entities.

```text
       ┌────────────────────────────────────────────────────────┐
       │                 StudentManagement.API                  │
       │   (Controllers, Program.cs, Middleware, Configuration) │
       └───────────┬────────────────────────────────┬───────────┘
                   │                                │
                   ▼                                ▼
┌────────────────────────────────────┐    ┌────────────────────────────────────┐
│   StudentManagement.Application    │    │  StudentManagement.Infrastructure  │
│  (Services, DTOs, Custom Errors)   │◄───┤  (EF Core DbContext, Repositories) │
└──────────────────┬─────────────────┘    └─────────────────┬──────────────────┘
                   │                                        │
                   ▼                                        ▼
       ┌────────────────────────────────────────────────────────┐
       │                 StudentManagement.Core                 │
       │     (Domain Entities, Repository Contract Interfaces)  │
       └────────────────────────────────────────────────────────┘
```

### Layer Responsibilities
1. **Core Layer (`StudentManagement.Core`)**: Contains enterprise domain entities (`Student`, `User`) and data access contract interfaces (`IStudentRepository`, `IUserRepository`). Highly decoupled with **zero external dependencies**.
2. **Application Layer (`StudentManagement.Application`)**: Contains business logic orchestration (`StudentService`), input/output contracts (`DTOs`), validation rules using DataAnnotations, standardized generic API responses (`ApiResponse<T>`), and custom domain exceptions. References only the Core layer.
3. **Infrastructure Layer (`StudentManagement.Infrastructure`)**: Handles external infrastructure access concerns including SQL Server database communication using **Entity Framework Core Code-First**, implementation of repository interfaces, and authentication token generating services (`AuthService`). References Core and Application layers.
4. **API Presentation Layer (`StudentManagement.API`)**: Exposes RESTful HTTP endpoints via thin controllers, sets up middleware pipelines (Serilog structured logging, Global Exceptions, JWT Authentication, Swagger with Bearer authorization tokens), and configures runtime Dependency Injection.

---

## 📁 Folder Structure

```text
StudentManagementSystem/
│
├── StudentManagement.Core/
│   ├── Entities/
│   │   ├── Student.cs
│   │   └── User.cs
│   └── Interfaces/
│       └── Repositories/
│           ├── IStudentRepository.cs
│           └── IUserRepository.cs
│
├── StudentManagement.Application/
│   ├── DTOs/
│   │   ├── ApiResponse.cs
│   │   ├── AuthResponseDto.cs
│   │   ├── CreateStudentDto.cs
│   │   ├── LoginDto.cs
│   │   ├── StudentDto.cs
│   │   └── UpdateStudentDto.cs
│   ├── Exceptions/
│   │   ├── BadRequestException.cs
│   │   └── NotFoundException.cs
│   └── Interfaces/
│       └── Services/
│           ├── IAuthService.cs
│           └── IStudentService.cs
│
├── StudentManagement.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   │   ├── StudentRepository.cs
│   │   └── UserRepository.cs
│   └── Services/
│       └── AuthService.cs
│
└── StudentManagement.API/
    ├── Controllers/
    │   ├── AuthController.cs
    │   └── StudentsController.cs
    ├── Middleware/
    │   └── GlobalExceptionMiddleware.cs
    ├── appsettings.json
    └── Program.cs
```

---

## 📦 NuGet Packages Used

| Project | NuGet Package | Version | Purpose |
| :--- | :--- | :--- | :--- |
| **API** | `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.0.0` | JWT validation middleware & authorization integration |
| **API** | `Microsoft.EntityFrameworkCore.Design` | `8.0.0` | EF Core design-time migration tool support |
| **API** | `Microsoft.EntityFrameworkCore.Tools` | `8.0.0` | Package Manager Console migration tooling |
| **API** | `Serilog.AspNetCore` | `8.0.0` | High-performance structured logging integration |
| **API** | `Serilog.Sinks.File` | `5.0.0` | File persistence sink for rolling application logs |
| **API** | `Swashbuckle.AspNetCore` | `6.5.0` | Automatic Swagger API specification generation |
| **Application** | `System.IdentityModel.Tokens.Jwt` | `8.0.0` | JWT token parsing and serialization utilities |
| **Application** | `Microsoft.Extensions.Configuration.Abstractions` | `8.0.0` | Access configuration parameters cleanly |
| **Infrastructure** | `Microsoft.EntityFrameworkCore.SqlServer` | `8.0.0` | SQL Server database provider for EF Core |
| **Infrastructure** | `Microsoft.EntityFrameworkCore` | `8.0.0` | Core Object-Relational Mapper framework |

---

## ⚙️ Configuration Details

### 1. Program.cs & Middleware Setup
- **Two-stage initialization** with Serilog Bootstrap Logger ensures startup errors are perfectly captured.
- **Dependency Injection**: Services and repositories are registered cleanly with `AddScoped` lifetimes.
- **Global Exception Middleware**: Intercepts unhandled HTTP requests, prevents generic server crashes, logs complete stacks internally to Serilog, and maps errors cleanly to standard HTTP status codes (`400 Bad Request`, `404 Not Found`, `500 Internal Server Error`) wrapped inside the standardized `ApiResponse<T>` envelope.

### 2. JWT Configuration
Configured inside `appsettings.json` with secure minimum cryptographic signature length (>32 bytes):
```json
"Jwt": {
  "Secret": "SuperSecretKeyForStudentManagementSystemWebAPI2026Secure!!!",
  "Issuer": "StudentManagementAPI",
  "Audience": "StudentManagementUsers"
}
```

### 3. Serilog Configuration
Logs are dynamically structured and routed to both the standard console output and persistent daily rolling log files under `logs/log-<date>.txt`.

---

## 💾 SQL Migration Steps

To set up the database using Entity Framework Core Code-First migrations, execute the following commands from the root directory containing the solution:

### Step 1: Install EF Core CLI Tooling (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### Step 2: Add Initial Migration
```bash
dotnet ef migrations add InitialCreate --project StudentManagement.Infrastructure --startup-project StudentManagement.API
```

### Step 3: Update the Database
```bash
dotnet ef database update --project StudentManagement.Infrastructure --startup-project StudentManagement.API
```
> [!NOTE]  
> The migration configuration automatically seeds a default administrator account (`admin` / `Admin@123`) to streamline local API evaluation.

---

## 🔐 Swagger Bearer JWT Authentication Guide

1. Launch the API application (`dotnet run --project StudentManagement.API`).
2. Navigate to the Swagger UI endpoint served at the main application URL.
3. Under the **AuthController**, trigger the `POST /api/auth/login` endpoint using the default administrative credentials:
   ```json
   {
     "username": "admin",
     "password": "Admin@123"
   }
   ```
4. Copy the raw `token` string value from the successful response block.
5. Click the green **Authorize** button located at the top-right of the Swagger UI dashboard.
6. Type `Bearer ` followed by a space and paste the copied token string. Click **Authorize**.
7. All protected endpoints within the **StudentsController** can now be invoked securely.

---

## 🚀 API Endpoint Inventory

All API responses are enveloped within a standardized payload structure:
```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { ... },
  "errors": null
}
```

### Students API (`/api/students`)
- `GET /api/students` — Retrieve all active student profiles.
- `GET /api/students/{id}` — Fetch specific profile by unique record integer ID.
- `POST /api/students` — Provision a new student entity. Returns `201 Created` along with location tracking headers.
- `PUT /api/students/{id}` — Mutate/update existing student parameters.
- `DELETE /api/students/{id}` — Decommission student record gracefully.

---

## 📤 GitHub Submission Steps

To submit this project cleanly to a GitHub repository for assignment review:

1. **Initialize Git Repository:**
   ```bash
   cd d:\Machine_Test\StudentManagementSystem
   git init
   ```

2. **Create a standard `.gitignore` file** (to avoid committing build output binaries):
   ```bash
   dotnet new gitignore
   ```

3. **Stage and Commit Code:**
   ```bash
   git add .
   git commit -m "feat: initial commit of enterprise Student Management System Web API"
   ```

4. **Link Remote Repository & Push:**
   ```bash
   git remote add origin <your-github-repository-url>
   git branch -M main
   git push -u origin main
   ```
