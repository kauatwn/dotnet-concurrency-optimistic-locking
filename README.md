# TicketFlow

A high-performance Ticket Reservation System built with **C# 14** and **.NET 10**. This project serves as an engineering sandbox to explore **Concurrency Control**, **Data Integrity**, and **Robust Persistence** patterns, evolving beyond simple CRUD operations to handle race conditions in a distributed environment.

## Table of Contents

- [Prerequisites](#prerequisites)
- [How to Run](#how-to-run)
- [Project Structure](#project-structure)
- [Architecture & Design Principles](#architecture--design-principles)

## Prerequisites

Ensure you have the following installed to run this project efficiently:

- **[.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)** (or later)
- **[Docker Desktop](https://www.docker.com/)** (Required to orchestrate the PostgreSQL database)
- **IDE:** [Visual Studio](https://visualstudio.microsoft.com), [Visual Studio Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/).

## How to Run

### 1. Clone the Repository

```bash
git clone https://github.com/kauatwn/dotnet-concurrency-optimistic-locking.git
```

### 2. Enter the Directory

```bash
cd dotnet-concurrency-optimistic-locking
```

### 3. Run with Docker Compose

This command builds the API, starts the PostgreSQL database, and **automatically applies migrations** on startup.

```bash
docker compose up -d
```

_The API documentation will be accessible at `https://localhost:8081/swagger`._

### 4. Execute Tests

To validate concurrency handling, domain logic, and time-dependent rules:

```bash
dotnet test
```

## Project Structure

The solution follows the **Clean Architecture** principles to ensure separation of concerns and testability, with a dedicated split between Unit and Integration testing.

```plaintext
dotnet-concurrency-optimistic-locking/
├── src/
│   ├── TicketFlow.API/               # Entry point, Controllers, Global Exception Handling
│   ├── TicketFlow.Application/       # Use Cases, DTOs
│   ├── TicketFlow.Domain/            # Aggregate Roots, Value Objects, Pure Logic
│   └── TicketFlow.Infrastructure/    # EF Core (PostgreSQL), Concurrency Handling, TimeProvider
└── tests/
    ├── TicketFlow.UnitTests/         # Domain Logic & Time Travel Tests
    └── TicketFlow.IntegrationTests/  # Race Condition Simulations & DB Tests
```

## Architecture & Design Principles

This repository prioritizes **software engineering quality** and **maintainability**, following strict development guidelines.

### 1. Domain-Driven Design (DDD)

The core logic resides entirely within the `Domain` layer.

- **Aggregate Roots:** `Ticket` acts as a transactional boundary. Operations like `Reserve()` enforce invariants immediately.
- **Method Injection:** Entities do not depend on `DateTime.UtcNow`. Instead, they receive the `currentDate` as a parameter, making them pure and testable.
- **Value Objects:** Concepts like `Seat` (Sector/Row/Number) are immutable and structural.

### 2. Design Patterns

The project utilizes established patterns to ensure modularity and scalability.

|          Pattern          | Usage Scenario                               | Implementation                            |
| :-----------------------: | :------------------------------------------- | :---------------------------------------- |
|       **Use Cases**       | Encapsulating specific application workflows | `IReserveTicketUseCase`                   |
|  **Optimistic Locking**   | Handling concurrent writes                   | `xmin` (PostgreSQL hidden transaction ID) |
| **Global Error Handling** | Standardizing API errors (409/400)           | `IExceptionHandler`                       |

### 3. Concurrency & Locking Strategy

To prevent "double booking" without killing performance, we made specific engineering decisions.

> [!IMPORTANT]
> **Architectural Decision: Optimistic Locking**
> To handle high-concurrency scenarios (e.g., thousands of users trying to buy the same seat), we implemented **Optimistic Locking**:
>
> - **Why not Pessimistic Locking?** Keeping database rows locked (`SELECT FOR UPDATE`) while the user "thinks" or pays would degrade performance and throughput significantly.
> - **How it works:** If two users read the same ticket version, the first one to write wins. The second one tries to update a stale record, which triggers a `DbUpdateConcurrencyException`. We catch this exception in the Persistence layer and translate it to a **409 Conflict** response.

![Sequence Diagram illustrating Optimistic Locking in action](./docs/optimistic-locking-sequence.png)
_Figure 1: Sequence diagram demonstrating the race condition handling and the 409 Conflict response._

### 4. Comprehensive Testing Strategy

The project adopts a strategy focused on **Time** and **Parallelism**.

- **Unit Tests (Time Travel):** We use time injection through method parameters to simulate shows in the past or future without dirty hacks like `Thread.Sleep`.
- **Integration Tests:** We spawn separate Service Scopes to simulate concurrent users (`Task.WhenAll`) hitting the real database via **Testcontainers** (PostgreSQL) to prove the locking mechanism works.

> [!NOTE]
> **Testing Isolation Strategy**
> Unlike standard CRUD tests, our integration tests **intentionally** share resources (the same Ticket ID) to provoke Race Conditions and validate that the system rejects the second attempt.

### 5. Known Limitations & Pragmatic Trade-offs

This project is an engineering sandbox focused on demonstrating Concurrency Controls (Optimistic Locking). While the architecture successfully prevents "double booking" for individual seats, it introduces specific pragmatic trade-offs favoring performance over strict global locking:

- **Global Rule Bypass (Check-Then-Act):** To maximize throughput, the transaction boundary (Aggregate Root) is restricted to the individual `Ticket`. The `MaxTicketsPerUser` rule is verified without a global lock. If a malicious user fires parallel requests for _different_ tickets simultaneously, all threads might read the same initial state (e.g., "0 tickets bought"), allowing the user to bypass the limit. _Production Mitigation:_ Implement eventual consistency checks (a Background Worker that cancels excess orders) or enforce a pessimistic lock on a dedicated user-quota table.
- **Exception Overhead under Extreme Contention:** Optimistic locking relies on throwing and catching `DbUpdateConcurrencyException` when collisions occur. If thousands of bots attempt to buy the _exact same_ ticket at the exact same millisecond, the API will generate thousands of exceptions. Catching exceptions is a CPU-intensive operation in .NET and could spike server load. _Production Mitigation:_ For hyper-scale scenarios (e.g., major concerts), this architecture would typically be fronted by a Virtual Waiting Room or an asynchronous queue (like AWS SQS) to absorb the traffic spike and serialize processing.

### 6. CI/CD & Quality

The project includes a **GitHub Actions** workflow that ensures quality on every push:

- **Parallel Testing:** Ensures the system handles load correctly.
- **Static Analysis:** Integrates with **SonarCloud** for code quality gates, ensuring strict compiler warnings and type safety.
- **Docker Build Validation:** Verifies that the container image builds successfully via Docker Compose.
