# LedgerPro

A modern General Ledger system built with **.NET 10** that automates the transition from raw bank statements to structured accounting records.

## The Vision
Provide a clean, automated pipeline to ingest CSV bank statements from multiple sources (Bank accounts, Credit Cards) and map them to a standardized Chart of Accounts.

## Architecture
This project follows **Clean Architecture** principles to ensure the business logic (Accounting Rules) remains independent of infrastructure (File Parsing/Databases).

- **LedgerPro.Core**: Contains domain entities (Transactions, Accounts), custom business exceptions, and core repository interfaces.
- **LedgerPro.Application**: The orchestrator. Coordinates system use cases, executes business logic, manages DTO mapping, and handles transaction boundaries via the Unit of Work.
- **LedgerPro.Infrastructure**: The data and external service layer. Implements repository interfaces using EF Core/SQLite and contains the underlying bank CSV parsing logic.
- **LedgerPro.Api**: The entry point. Features lightweight Minimal API endpoints for uploading statements and generating reports, backed by centralized global exception handling middleware.
- **LedgerPro.Tests**: The automated test suite. Combines lightweight unit tests for business logic with robust WebApplicationFactory integration tests to ensure API contract and data integrity.

## Tech Stack & Tools
- **Framework:** .NET 10 (C#)
- **Database:** SQLite with Entity Framework Core
- **Testing:** xUnit & FluentAssertions
- **Automation:** GitHub Actions for CI/CD
- **Parsing:** CsvHelper for robust file ingestion

## Roadmap
- [x] Core Domain Modeling (Current)
- [x] Bank Statement Parsing Service
- [x] SQLite Integration & Migrations
- [x] Categorization Engine (Auto-mapping descriptions to accounts)
- [ ] Advanced Read-Model Aggregations (UI Bridge)
- [ ] Basic Financial Report API
- [ ] LedgerPro Web UI
- [ ] Account Register View
- [ ] Statement Upload Portal
- [ ] Reporting Dashboard

## Local Setup
1. Clone the repository.
2. Ensure you have the **.NET 10 SDK** installed.
3. Run `dotnet restore` to install dependencies.
4. Run `dotnet build` to verify the project structure.