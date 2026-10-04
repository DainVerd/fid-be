# OverView

This project implements BE part of the FID developer test task.
[The FE of the task could be find here](https://github.com/DainVerd/fid-fe)
The app loads document metadata from an XML source, validates and maps the records, stores them in SQLite using EF Core , and exposes the stored data through a REST API.

The API returns JSON and supports:

- Pagination
- Filtering
- Sorting
- Centralized error handling using ProblemDetails

List of content:

- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [XML Data Import](#xml-data-import)
- [Data Storage Choise](#data-storage-choise)
- [Error Handling](#error-handling)
- [How to launch project](#running-be)
- [Running Tests](#running-tests)
- [Decisions](#decisions)
  - [rest and json](#rest-json)
  - [Repository and Unit of Work](#repository-and-unit-of-work)
  - [server side pagination](#pagination)
  - [Limitations](#limitations)

## Technology Stack

- .NET/ASP.NET
- EF Core
- SQLite
- xUnit
- Moq
- AwesomeAssertions
- Swagger for endpoint testing

## Architecture

The solution is separated into the following projects by clean architecture :

- Domain
- Application
- Infrastructure
- WebApi
- Application.Tests
- Infrastructure.Tests

## XML Data Import

The app imports document metadata from an XML file during app's initialization.

If individial doc contains invalid data, that record is skipped and the remaining valid records are still imported.

## Data Storage Choise

SQLite was selected because it provides lightweight persistence without requiring external database infrastructure.

It integrates directly with EF Core and makes the test app easy to run locally.

For a larger production system with multiple concurrent users and larger datasets better to use PostgreSQL or SQL DB.

## Error Handling

The application uses a global exception handler. Errors are returnes using the standard ASP.NET Core ProblemDetails format. This keeps error responses consistent and avoids exception handling logic inside controllers.

## Running BE

### By command line

Restore dependencies

```sh
dotnet restore
```

Apply database migration

```sh
dotnet ef database update --project Infrastructure --startup-project WebApi
```

Run the API:

```sh
dotnet run --project WebApi
```

### By Visual studio

1) Select `WebAPI` project as startup project
2) press green arrow on visual studio navbar to launch project
3) Swagger can then be used to inspect and test the available API endpoints.

## Running Tests

Run all backend tests via command.

```sh
dotnet tests
```

The tests cover importa application behavior:

- documents mapping
- pagination
- empty results
- valid xml loading
- invalid XML records being skipped
- Invalid enum values
- Invalid reading time
- Database initialization
- Import being skipped when records already exist

## Decisions

### REST JSON

XML is used as the external import format because it is part of the homework requirements.

The FE interacts with the BE through a normal REST API using JSON.

### Repository and Unit of Work

A small repository and Unit of Work abstraction is used to separate apps logic from EF Core and to simplify unit testing.

### Pagination

Pagination, filtering and sorting are performed on the BE so the API remains suitable for larger datasets.

### Limitations

Given more time following could be added:

- external HTTP-base XML source instead of a lcoal simulated src
- more advanced validation
- server-side caching
- authentification and authorization
- docker support
- integration tests
