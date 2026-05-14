# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **Tests:** `Odex.AspNetCore.Clarc.Infrastructure.Tests` (xUnit) covering query builders, pagination extensions, specification extensions, and `TransactionContext`.
- **Repository:** root `README.md`, `CONTRIBUTING.md`, `SECURITY.md`, `CODE_OF_CONDUCT.md`, GitHub issue templates, Dependabot, and CI/CD aligned with open-source conventions.

## [0.2.0] - 2026-05-14

### Added

- **`BaseQueryBuilder`** / **`PagedQueryBuilder`** — fluent `IQueryable` composition with Domain **`PagedRequest`** pagination.
- **`QueryablePagedResponseExtensions`** — **`PagedResponse<T>`** materialization (sync **`ToPagedResponse`**, async **`ToPagedResponseAsync`** with injectable delegates or known total).
- **`QueryableSpecificationExtensions`** — **`Where(IQueryable<T>, Specification<T>)`**.
- **`TransactionContext`** — default **`ITransactionContext`** with **`NotifyCommitted`** / **`NotifyRolledBack`**.
- **`InfrastructureException`** hierarchy with optional inner exceptions; **`SerializationException`** accepts an inner exception.

### Changed (breaking)

- Renamed **`ClarcQueryBuilder`** → **`BaseQueryBuilder`**; **`ClarcPaginatedQueryBuilder`** → **`PagedQueryBuilder`**.

### Fixed

- **`BaseQueryBuilder`:** `ApplyIncludes` / `ApplySorts` defaults no longer incorrectly set **`HasIncludes`** / **`HasSorts`**; callers mark flags when they apply changes.
- **Validation:** null checks for query, **`ModifyQuery`** delegate and result, and **`PagedRequest`** on **`PagedQueryBuilder`**.
- **Messages:** clearer **`ExternalServiceException`**, **`GeneratorException`**, and **`TransactionException`** text.

### Dependencies

- **`Odex.AspNetCore.Clarc.Domain`** **0.2.0**.

## [0.1.1] - 2026-05-14

### Changed

- Domain package reference **0.1.2 → 0.2.0**; XML documentation on public API; CI workflows and Dependabot scaffolding.

## [0.1.0] - Initial published baseline

- Query builders, infrastructure exceptions, and package metadata.

[Unreleased]: https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/compare/v0.1.0...v0.1.1
