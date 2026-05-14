using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Base type for infrastructure-layer failures (configuration, IO, external services, persistence helpers).
/// </summary>
/// <param name="message">Human-readable error message.</param>
/// <param name="type">Programmatic category for logging or mapping.</param>
/// <param name="innerException">Optional exception that caused this failure; passed to <see cref="Exception.Exception(string, Exception?)"/>.</param>
public abstract class InfrastructureException(string message, ExceptionType type, Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>
    /// Gets the infrastructure exception category.
    /// </summary>
    public ExceptionType Type { get; } = type;
}
