# Odex.AspNetCore.Clarc.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/Odex.AspNetCore.Clarc.Infrastructure)](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Infrastructure)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Odex.AspNetCore.Clarc.Infrastructure)](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Infrastructure)
[![CI](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/actions/workflows/ci.yml/badge.svg)](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/actions/workflows/ci.yml)
[![Release](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/actions/workflows/release.yml/badge.svg)](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/actions/workflows/release.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Reusable **infrastructure-layer** helpers for **.NET 9** that align with **[Odex.AspNetCore.Clarc.Domain](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Domain)**: LINQ query builders, **`PagedRequest` / `PagedResponse<T>`** materialization, **`Specification<T>`** on **`IQueryable`**, a default **`ITransactionContext`**, and typed **infrastructure** exceptions. The **public API is documented** with XML comments. This assembly targets **`net9.0`** and does **not** reference a specific ORM—you plug in **`Count`**, **`ToList`**, or async equivalents from your stack.

---

## Table of contents

- [About](#about)
- [Installation](#installation)
- [Requirements](#requirements)
- [Clean architecture role](#clean-architecture-role)
- [Capabilities](#capabilities)
- [API documentation](#api-documentation)
- [Quick examples](#quick-examples)
- [Namespaces](#namespaces)
- [Exception types (Domain vs Infrastructure)](#exception-types-domain-vs-infrastructure)
- [CI/CD](#cicd)
- [Contributing](#contributing)
- [Security](#security)
- [License](#license)
- [Links](#links)

---

## About

**Odex.AspNetCore.Clarc.Infrastructure** is part of the **CLARC** (clean-architecture style) family. It complements the **Domain** package with **general-purpose** building blocks you typically implement once per solution: composable **`IQueryable`** pipelines, pagination aligned with **`PagedRequest`**, **`PagedResponse<T>`** projection, specification-based filters, and a cooperative **`ITransactionContext`** for unit-of-work code.

The **`AspNetCore`** segment in the package ID is **historical**; the library does not require ASP.NET Core at compile time.

---

## Installation

```bash
dotnet add package Odex.AspNetCore.Clarc.Infrastructure
```

Pin a version for reproducible builds:

```bash
dotnet add package Odex.AspNetCore.Clarc.Infrastructure --version 0.2.0
```

---

## Requirements

| Item | Version |
|------|---------|
| **Target framework** | `net9.0` |
| **Companion domain package** | **Odex.AspNetCore.Clarc.Domain** `0.2.x` (referenced transitively when you add this package) |
| **.NET SDK** | 9.x for development and CI |

Runtime dependencies beyond **.NET** and **Domain**: none. The package build uses **Microsoft.SourceLink.GitHub** (private asset) for debuggable symbols on GitHub.

---

## Clean architecture role

| Layer | Responsibility | CLARC package |
|-------|------------------|---------------|
| **Domain** | Entities, value objects, specifications, repository **interfaces**, domain exceptions | **Odex.AspNetCore.Clarc.Domain** |
| **Application** | Use cases, orchestration | *Odex.AspNetCore.Clarc.Application* (separate) |
| **Infrastructure** | Persistence adapters, technical exceptions, query composition | **This package** (plus your ORM-specific code in the host solution) |

This library stays **below** application use-case code: it provides **helpers**, not business rules.

---

## Capabilities

| Area | Summary |
|------|---------|
| **Query builders** | **`BaseQueryBuilder<TEntity,TIncludes>`**, **`PagedQueryBuilder<,>`** — fluent **`ModifyQuery`**, flags (`IsFiltered`, `HasIncludes`, `HasSorts`), **`ApplyPagination`** from **`PagedRequest`**. |
| **Paged responses** | **`QueryablePagedResponseExtensions`** — **`ToPagedResponse`** (sync LINQ); **`ToPagedResponseAsync`** with injected count/page delegates for async ORMs. |
| **Specifications** | **`QueryableSpecificationExtensions.Where(query, specification)`** — uses **`Specification<T>.ToExpression()`**. |
| **Transactions** | **`TransactionContext`** — implements **`ITransactionContext`**; call **`NotifyCommitted`** / **`NotifyRolledBack`** from your **`IBaseRepository`** implementation. |
| **Exceptions** | **`InfrastructureException`** and concrete types (`ConfigurationException`, `RepositoryException`, …) with **`Infrastructure.Constants.ExceptionType`**. |

---

## API documentation

Public types and members use **`///` XML documentation**. **`GenerateDocumentationFile`** is enabled; the NuGet package ships **`Odex.AspNetCore.Clarc.Infrastructure.xml`** next to the assembly for IntelliSense and [NuGet.org](https://www.nuget.org/) tooltips.

---

## Quick examples

**Paged response (sync, in-memory or deferred provider):**

```csharp
using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;
using Odex.AspNetCore.Clarc.Infrastructure.Data.Pagination;

public sealed record MyListRequest : PagedRequest;

var page = myQuery.ToPagedResponse(new MyListRequest { Page = 2, PageSize = 10 });
```

**Paged response (async — host supplies delegates; example uses EF Core extension methods after referencing that package):**

```csharp
var page = await myQuery.ToPagedResponseAsync(
    request,
    (q, ct) => q.CountAsync(ct),   // e.g. Microsoft.EntityFrameworkCore
    (q, ct) => q.ToListAsync(ct),
    cancellationToken);
```

**Specification on `IQueryable`:**

```csharp
using Odex.AspNetCore.Clarc.Infrastructure.Data.Specifications;

var filtered = myQuery.Where(myDomainSpecification);
```

**Transaction context (in your unit of work):**

```csharp
using Odex.AspNetCore.Clarc.Infrastructure.Contexts;

var ctx = new TransactionContext();
// Pass ctx into ExecuteInTransactionAsync; after store work:
ctx.NotifyCommitted(); // or ctx.NotifyRolledBack();
```

---

## Namespaces

| Namespace | Contents |
|-----------|----------|
| `Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders` | `BaseQueryBuilder`, `PagedQueryBuilder` |
| `Odex.AspNetCore.Clarc.Infrastructure.Data.Pagination` | `QueryablePagedResponseExtensions` |
| `Odex.AspNetCore.Clarc.Infrastructure.Data.Specifications` | `QueryableSpecificationExtensions` |
| `Odex.AspNetCore.Clarc.Infrastructure.Contexts` | `TransactionContext` |
| `Odex.AspNetCore.Clarc.Infrastructure.Constants` | `ExceptionType` |
| `Odex.AspNetCore.Clarc.Infrastructure.Exceptions` | Infrastructure exception types |

---

## Exception types (Domain vs Infrastructure)

- **`Odex.AspNetCore.Clarc.Domain.Constants.ExceptionType`** — classifies **domain** failures (`DomainException`, policy violations, not found, concurrency, …).
- **`Odex.AspNetCore.Clarc.Infrastructure.Constants.ExceptionType`** — classifies **technical** failures (configuration, DB connectivity, external HTTP, serialization, …).

Use the **Domain** enum at your application boundary for business errors; use the **Infrastructure** enum for infrastructure exceptions shipped from this package.

---

## CI/CD

GitHub Actions run **restore → strict Release build → tests → pack** on every push and pull request to **`main`**, and a **Release** workflow publishes to **NuGet.org** from version tags or manual dispatch. See **[CONTRIBUTING.md](CONTRIBUTING.md#cicd-github-actions)** for **`NUGET_API_KEY`** setup and tagging conventions.

---

## Contributing

See **[CONTRIBUTING.md](CONTRIBUTING.md)** for local build commands, XML documentation rules, pull request expectations, and versioning.

---

## Security

Report vulnerabilities privately per **[SECURITY.md](SECURITY.md)**. Do not use public issues for undisclosed security problems.

---

## License

This project is released under the **[MIT License](LICENSE)**.

---

## Links

- **Domain package:** [Odex.AspNetCore.Clarc.Domain](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Domain) — [repository](https://github.com/o-shabi/odex-clarc-domain-aspnetcore), [changelog](https://github.com/o-shabi/odex-clarc-domain-aspnetcore/blob/main/CHANGELOG.md)
- **This repository:** [github.com/o-shabi/odex-clarc-infrastructure-aspnetcore](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore)
- **This changelog:** [CHANGELOG.md](CHANGELOG.md)
