# 🏗️ Odex.AspNetCore.Clarc.Infrastructure

[![NuGet Version](https://img.shields.io/nuget/v/Odex.AspNetCore.Clarc.Infrastructure)](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Infrastructure)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Odex.AspNetCore.Clarc.Infrastructure)](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Infrastructure)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Infrastructure layer implementation for DDD-based ASP.NET Core applications**  
*Provides data access abstractions, query builders, and infrastructure-specific exceptions.*

---

## 📦 Overview

`Odex.AspNetCore.Clarc.Infrastructure` is the companion infrastructure layer to `Odex.AspNetCore.Clarc.Domain`. It
provides concrete implementations and utilities for:

- **Query Building** – Fluent, extensible query builders with pagination support.
- **Infrastructure Exceptions** – Typed exceptions for configuration, database, external services, serialization, and
  more.
- **Data Access Abstractions** – Base classes for building dynamic LINQ queries with includes, filters, and sorting.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- An ASP.NET Core project (Web API, Minimal API, or MVC)
- [Odex.AspNetCore.Clarc.Domain](https://www.nuget.org/packages/Odex.AspNetCore.Clarc.Domain) package (optional but recommended)

### Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package Odex.AspNetCore.Clarc.Infrastructure
```

Or use the Package Manager Console:

```bash
Install-Package Odex.AspNetCore.Clarc.Infrastructure
```

---

## ✨ Features

| Feature                           | Description                                                                                      |
|-----------------------------------|--------------------------------------------------------------------------------------------------|
| 🔍 **ClarcQueryBuilder**          | Base query builder with modification pipeline (`ModifyQuery`), include tracking, and sort flags. |
| 📄 **ClarcPaginatedQueryBuilder** | Extends query builder with automatic `Skip`/`Take` pagination using `PagedRequest`.              |
| ⚠️ **Infrastructure Exceptions**  | Typed exceptions for common infrastructure failures (DB, config, external services, etc.).       |
| 🏷️ **State Tracking**            | Built-in flags (`IsFiltered`, `HasIncludes`, `HasSorts`) to know what was applied to the query.  |

---

## 🏗️ Core Components

### 1. Query Builders

**ClarcQueryBuilder<TEntity, TIncludes>**

Base abstract class for building dynamic LINQ queries:

```csharp

namespace Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

public abstract class ClarcQueryBuilder<TEntity, TIncludes>(IQueryable<TEntity> query)
    where TEntity : class
{
    private IQueryable<TEntity> _query = query;
    protected bool IsFiltered;
    protected bool HasIncludes;
    protected bool HasSorts;

    protected void MarkAsFiltered() => IsFiltered = true;
    protected void MarkAsHasIncludes() => HasIncludes = true;
    protected void MarkAsHasSorts() => HasSorts = true;

    protected IQueryable<TEntity> ModifyQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> expression)
    {
        _query = expression(_query);
        return _query;
    }

    protected virtual void ApplyIncludes(IReadOnlyList<TIncludes>? includes)
    {
        MarkAsHasIncludes();
    }

    protected virtual void ApplySorts()
    {
        MarkAsHasSorts();
    }

    public virtual IQueryable<TEntity> Build() => _query;
}
```

**ClarcPaginatedQueryBuilder<TEntity, TIncludes>**

Extends the base builder with automatic pagination:

```csharp
using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;

namespace Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

public abstract class ClarcPaginatedQueryBuilder<TEntity, TIncludes>(IQueryable<TEntity> query, PagedRequest request)
    : ClarcQueryBuilder<TEntity, TIncludes>(query)
    where TEntity : class
{
    protected virtual void ApplyPagination()
    {
        if (request.PageSize > 0) ModifyQuery(q => q.Skip(request.SkipCount).Take(request.PageSize));
    }
}
```

### 2. Infrastructure Exceptions

All exceptions inherit from `InfrastructureException` and include an `ExceptionType` enum for programmatic handling.

| Exception                     | Use Case                                       |
|-------------------------------|------------------------------------------------|
| `ConfigurationException`      | Missing/invalid configuration values.          |
| `DatabaseConnectionException` | Failed connection to database.                 |
| `ExternalServiceException`    | HTTP call failures to external APIs.           |
| `GeneratorException`          | ID generation or token generation failures.    |
| `RepositoryException`         | Generic repository operation failures.         |
| `SerializationException`      | JSON/XML serialization/deserialization errors. |
| `TransactionException`        | Transaction rollback or commit failures.       |

**Exception Types Enum:**

```csharp
namespace Odex.AspNetCore.Clarc.Infrastructure.Constants;

public enum ExceptionType
{
    Unknown,
    Configuration,
    DbConnection,
    ExternalServiceFailed,
    Generator,
    Repository,
    Serialization,
    Transaction
}
```

---

## 🚀 Usage Examples

### Creating a Custom Query Builder

```csharp
public class UserQueryBuilder : ClarcQueryBuilder<User, UserInclude>
{
    public UserQueryBuilder(IQueryable<User> query) : base(query) { }
    
    public UserQueryBuilder WithName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            ModifyQuery(q => q.Where(u => u.Name.Contains(name)));
            MarkAsFiltered();
        }
        return this;
    }
    
    public UserQueryBuilder WithEmail(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            ModifyQuery(q => q.Where(u => u.Email == email));
            MarkAsFiltered();
        }
        return this;
    }
    
    protected override void ApplyIncludes(IReadOnlyList<UserInclude>? includes)
    {
        if (includes == null) return;
        
        foreach (var include in includes)
        {
            ModifyQuery(q => include switch
            {
                UserInclude.Orders => q.Include(u => u.Orders),
                UserInclude.Profile => q.Include(u => u.Profile),
                _ => q
            });
        }
        
        base.ApplyIncludes(includes);
    }
}

// Usage
var query = new UserQueryBuilder(_context.Users)
    .WithName("John")
    .WithEmail("john@example.com")
    .Build();
```

### Using Paginated Query Builder

```csharp
public class PaginatedProductQueryBuilder : ClarcPaginatedQueryBuilder<Product, ProductInclude>
{
    public PaginatedProductQueryBuilder(IQueryable<Product> query, PagedRequest request) 
        : base(query, request) { }
    
    public PaginatedProductQueryBuilder WithCategory(int categoryId)
    {
        if (categoryId > 0)
        {
            ModifyQuery(q => q.Where(p => p.CategoryId == categoryId));
            MarkAsFiltered();
        }
        return this;
    }
    
    public PaginatedProductQueryBuilder WithPriceRange(decimal min, decimal max)
    {
        ModifyQuery(q => q.Where(p => p.Price >= min && p.Price <= max));
        MarkAsFiltered();
        return this;
    }
    
    public override IQueryable<Product> Build()
    {
        ApplyIncludes(null);
        ApplySorts();
        ApplyPagination();
        return base.Build();
    }
}

// Usage (with automatic pagination)
var request = new PagedRequest { Page = 1, PageSize = 20 };
var query = new PaginatedProductQueryBuilder(_context.Products, request)
    .WithCategory(5)
    .WithPriceRange(10, 100)
    .Build();
    
var products = await query.ToListAsync();
```

### Handling Infrastructure Exceptions

```csharp
public class ProductRepository
{
    public async Task<Product> GetByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new RepositoryException(nameof(Product), "GetById");
            
            return product;
        }
        catch (SqlException ex)
        {
            throw new DatabaseConnectionException("ProductDb", ex.Message);
        }
    }
    
    public async Task<string> ExportToJsonAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(products);
            return json;
        }
        catch (JsonException ex)
        {
            throw new SerializationException("Product export", ex);
        }
    }
    
    public async Task CallExternalApiAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.example.com/products");
            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalServiceException("ProductApi", (int)response.StatusCode, response.ReasonPhrase ?? "Unknown error");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException("ProductApi", 0, ex.Message);
        }
    }
}

// Exception handling in service layer
try
{
    await _productRepository.CallExternalApiAsync();
}
catch (ExternalServiceException ex)
{
    _logger.LogError(ex, "External service failed");
    return Result.Failure($"Service error: {ex.Message}");
}
catch (DatabaseConnectionException ex)
{
    _logger.LogCritical(ex, "Database unavailable");
    return Result.Failure("System temporarily unavailable");
}
catch (InfrastructureException ex)
{
    _logger.LogWarning(ex, "Infrastructure error of type {Type}", ex.Type);
    return Result.Failure("Operation failed");
}
```

---

## 📂 Namespace Map

| Namespace                                                 | Purpose                                           |
|-----------------------------------------------------------|---------------------------------------------------|
| `Odex.AspNetCore.Clarc.Infrastructure.Constants`          | `ExceptionType` enum                              |
| `Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders` | `ClarcQueryBuilder`, `ClarcPaginatedQueryBuilder` |
| `Odex.AspNetCore.Clarc.Infrastructure.Exceptions`         | All infrastructure-specific exceptions            |

---

## 🔗 Related Packages

- **Odex.AspNetCore.Clarc.Domain** – Domain layer with aggregates, events, specifications, and policies.

---

## 🤝 Contributing

Contributions are welcome! Please follow the standard GitHub flow:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License** – see the [LICENSE](LICENSE) file for details.

---

*Built with ❤️ for clean DDD infrastructure on ASP.NET Core*
