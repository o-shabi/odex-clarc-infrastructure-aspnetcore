using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown for repository-level failures that are not better modeled as <see cref="DatabaseConnectionException"/>.
/// </summary>
/// <param name="name">Aggregate or repository name.</param>
/// <param name="operation">Operation that failed (for example <c>GetById</c>).</param>
public class RepositoryException(string name, string operation)
    : InfrastructureException($"Repository operation '{operation}' failed for {name}", ExceptionType.Repository);
