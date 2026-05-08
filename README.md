# LedgerPro

A modern General Ledger system built with **.NET 10** that automates the transition from raw bank statements to structured accounting records.

## The Vision
Provide a clean, automated pipeline to ingest CSV bank statements from multiple sources (Bank accounts, Credit Cards) and map them to a standardized Chart of Accounts.


## Architecture
This project follows **Clean Architecture** principles to ensure the business logic (Accounting Rules) remains independent of infrastructure (File Parsing/Databases).

- **LedgerPro.Core**: Domain entities (Transactions, Accounts) and business interfaces.
- **LedgerPro.Infrastructure**: Data access (EF Core/SQLite) and Bank CSV parsing logic.
- **LedgerPro.Api**: RESTful endpoints for uploading statements and generating reports.


## Tech Stack & Tools
- **Framework:** .NET 10 (C#)
- **Database:** SQLite with Entity Framework Core
- **Testing:** xUnit & FluentAssertions
- **Automation:** GitHub Actions for CI/CD
- **Parsing:** CsvHelper for robust file ingestion

## Roadmap
- [x] Core Domain Modeling (Current)
- [ ] Bank Statement Parsing Service
- [x] SQLite Integration & Migrations
- [ ] Categorization Engine (Auto-mapping descriptions to accounts)
- [ ] Basic Financial Reporting API (Monthly Totals)

## Local Setup
1. Clone the repository.
2. Ensure you have the **.NET 10 SDK** installed.
3. Run `dotnet restore` to install dependencies.
4. Run `dotnet build` to verify the project structure.