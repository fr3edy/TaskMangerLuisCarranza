# 🚀 Task Management System - Enterprise Architecture Demo

## 📖 Project Overview
This repository contains a full-stack Task Management application built to demonstrate enterprise-level software engineering practices. The solution strictly adheres to **Clean Architecture** principles, separating business logic from infrastructure and presentation concerns. It utilizes Command Query Responsibility Segregation (CQRS) via MediatR to handle data operations efficiently and securely.

The application allows users to register, securely log in, and manage their personal tasks. Data isolation is enforced at the API level, ensuring users can only interact with their own records.

**Developed by:** Alfredo Carranza

---

## 🛠️ Tech Stack

### Backend
* **Framework:** .NET 8 (ASP.NET Core Minimal APIs)
* **Architecture:** Clean Architecture & CQRS (MediatR)
* **ORM:** Entity Framework Core
* **Database:** SQLite (Chosen for portability and zero-config deployment)
* **Security:** JWT Authentication, BCrypt Password Hashing
* **Testing:** xUnit, Moq, FluentAssertions (TDD Approach)

### Frontend
* **Framework:** Angular (Standalone Components)
* **Styling:** Tailwind CSS & Angular Material
* **State & Forms:** RxJS, Reactive Forms

---

## 🚀 Getting Started

### Prerequisites
* .NET 8.0 SDK
* Node.js (v18 or higher) & npm
* Angular CLI (`npm install -g @angular/cli`)

### 1. Backend Setup
The database is configured to automatically apply migrations and seed initial data upon the first execution. Open a terminal and navigate to the API project directory:

```bash
cd TaskManager.Api
dotnet run
The API will start. A SQLite database file (TaskManager.db) will be automatically generated in the root directory.

2. Frontend Setup
Open a new terminal window and navigate to the Angular project directory:

Bash
cd task-manager-ui
npm install
ng serve
Open your browser and navigate to http://localhost:4200.

🔐 Demo Credentials
Upon the initial startup, the database is seeded with a default administrator account. You can access the system using the following credentials:

Email: demo@test.com

Password: Admin123!


🧪 Running Unit Tests
The backend logic is covered by a comprehensive suite of unit tests focusing on isolated business rules and commands. To run the tests, execute the following command from the root of the solution:

Bash
dotnet test

## 🏗️ Architecture Highlights

* **Domain-Driven Design (DDD):** Entities encapsulate their own validation and state (e.g., private setters).
* **Data Isolation:** User IDs are extracted securely from JWT claims at the Minimal API endpoint layer and passed down to the application layer to prevent cross-tenant data access.
* **Security First:** Passwords are never stored in plain text. The application uses BCrypt for robust cryptographic hashing and verification.

---

## 📝 The Informal User Story (Project Motivation)

**Title:** Secure Task Management for Multi-Project Workflows

> *"As a lead developer establishing a new software company and actively managing multiple initial projects, I need a secure, centralized web application to manage my daily tasks. I want to be able to create tasks with strict due dates, update their statuses as my work progresses, and delete them when they are no longer relevant, so that I can maintain absolute control over my workflow and ensure no project deadlines fall through the cracks. Furthermore, I need to know that my workspace is completely isolated and secure, so that my task data cannot be accessed by unauthorized users."*

### Acceptance Criteria (What success looks like):

1. **Security First:** I can register and log in securely. Without logging in, I can't see anything.
2. **Full Control (CRUD):** I can add new tasks, read my current list, edit the details if scope changes, and remove tasks I entered by mistake.
3. **Data Privacy:** When I log in, I only see my tasks. I should never see tasks created by other users in the system.
4. **Validation:** The system should prevent me from accidentally setting a task's due date in the past.

---

## 🤖 GenAI Tools & Critical Thinking Report

**Document Purpose:** This section outlines my methodology for integrating Generative AI into the software development lifecycle. It demonstrates how I use AI as an advanced pair-programmer and boilerplate generator, while maintaining absolute control over architectural decisions, debugging, and code quality.

### 1. AI Integration Philosophy
As a lead developer, my approach to GenAI is *"Trust, but Verify through Architecture."*

* **Boilerplate & Syntax:** I use AI to rapidly generate repetitive structures (e.g., CQRS MediatR handlers, xUnit mock setups, Angular reactive forms) to save time.
* **Architectural Boundaries:** I never rely on AI to design the system's core architecture. Clean Architecture, Domain-Driven Design (DDD), and Data Isolation rules are strictly defined by me before any prompts are written.
* **Debugging Context:** When using AI for debugging, I provide highly specific environmental context rather than generic error codes, treating the AI as a sounding board to validate my hypotheses.

### 2. AI Prompt Engineering & Validation

**The Task:** Generate the backend scaffold and core implementation for a Task Management System API using C#.

**The Validation Process:**
* **Dependency Rule Check:** I verified that the AI did not violate the Dependency Inversion Principle. I ensured the Domain project had no references to Entity Framework Core or external libraries, keeping the domain pure.
* **Correcting Anemic Models:** The AI initially generated an anemic database model (only public getters/setters). I corrected this by implementing private setters and encapsulation methods to protect the `TaskItem` business rules.
* **Handling Security Edge Cases:** The AI suggested a basic CRUD endpoint. I modified it to inject the `ClaimsPrincipal`, ensuring users can only read and delete tasks where the `UserId` matches the JWT token, preventing privilege escalation.

### 3. Case Study 1: The Database Lifecycle & Cryptography Anomaly
* **The Scenario:** During the implementation of the `LoginCommandHandler`, the application threw an `Invalid salt version` exception from the BCrypt library during user authentication.
* **The Debugging Process:** I provided the AI with the exact error stack trace. The AI correctly identified that BCrypt panics when reading non-hashed strings and suggested deleting the `.db` file.
* **Architectural Intervention:** Deleting the `.db` file alone did not work. Using my knowledge of SQLite, I deduced the issue involved Write-Ahead Logging (`-wal` and `-shm` temporary files) and Visual Studio build artifacts.
* **The Solution:** I halted the application, cleaned the solution build, deleted all three SQLite files, and implemented a safety check in the `DataSeeder` (`context.Users.RemoveRange()`) to guarantee a pristine database state.

### 4. Case Study 2: Angular Standalone Components & Silent Failures
* **The Scenario:** I implemented a custom `routerLink` button for user creation and integrated Angular Material icons, but the UI failed silently.
* **The Fix:** While a junior developer might assume the HTML syntax was wrong, I used the AI to confirm my hypothesis regarding Angular's new Standalone Components paradigm. I directed the architecture to explicitly import `RouterLink` and `MatIconModule` directly into the component's `@Component(imports: [...])` array.

### 5. Case Study 3: TDD & Business Logic Validation
* **The Scenario:** Setting up the Unit Tests for the `CreateTaskCommandHandler`.
* **Validation & Correction:** The AI initially generated a test command passing a string into a field that required a `DateTime`. I manually intercepted the AI's output, correcting the domain model constraints and aligning the Mock setups with the exact signatures of my `ITaskRepository`. I also ensured we strictly tested the "Sad Path"—verifying that the system throws an `ArgumentException` if a task is created with a date in the past.




## 📦 Technical Decisions & Library Breakdown

### 1. Presentation Layer (API) - `TaskManager.Api`
| Library / Package | Technical Justification |
| :--- | :--- |
| **`Microsoft.AspNetCore.Authentication.JwtBearer`** | Natively validates incoming JWT tokens to secure private endpoints. |
| **`Serilog.AspNetCore`** | Replaces the default .NET logger to provide structured logging and file persistence. |
| **`Scalar.AspNetCore`** | Provides a modern, interactive visual interface for exploring API documentation. |
| **`Microsoft.AspNetCore.OpenApi`** | Automatically generates the OpenAPI specification from the Minimal APIs. |

### 2. Business Logic Layer - `TaskManager.Application`
| Library / Package | Technical Justification |
| :--- | :--- |
| **`MediatR`** | Implements CQRS and Mediator patterns. Decouples the API layer from business logic. |
| **`BCrypt.Net-Next`** | Implements one-way cryptographic hashing algorithm for passwords. |

### 3. Data Access Layer - `TaskManager.Infrastructure`
| Library / Package | Technical Justification |
| :--- | :--- |
| **`Microsoft.EntityFrameworkCore.Sqlite`** | The ORM provider translating C# LINQ queries into native SQLite SQL commands. |
| **`Microsoft.EntityFrameworkCore.Tools`** | Enables EF Core CLI commands to manage database schema evolution. |
| **`System.IdentityModel.Tokens.Jwt`** | Provides classes to construct, cryptographically sign, and issue authentication tokens. |

### 4. Core Domain Layer - `TaskManager.Domain`
| Library / Package | Technical Justification |
| :--- | :--- |
| **None** | **By Design.** In Clean Architecture, the Domain must have zero external dependencies. |

### 5. Testing Layer - `TaskManager.Tests`
| Library / Package | Technical Justification |
| :--- | :--- |
| **`xunit`** | The standard testing framework in .NET. |
| **`Moq`** | Allows the creation of mock implementations for interfaces to isolate business logic. |
| **`FluentAssertions`** | Transforms classic assertions into a fluid, human-readable language. |
| **`Microsoft.AspNetCore.Mvc.Testing`** | Bootstraps the entire API in memory for HTTP Integration Testing. |
| **`Microsoft.Data.Sqlite`** | Enables opening SQLite connections using `DataSource=:memory:` for RAM-speed tests. |

### 6. Frontend - `task-manager-ui`
| Library / Package | Technical Justification |
| :--- | :--- |
| **`@angular/core` & `@angular/router`** | Core engine utilized with Standalone Components architecture for SPA routing. |
| **`@angular/material`** | Official UI component library used for standardized icons and UI elements. |
| **`tailwindcss`** | Utility-first CSS framework for rapid, responsive UI layout. |