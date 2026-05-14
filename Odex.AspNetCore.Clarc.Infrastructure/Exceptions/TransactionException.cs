using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when a unit-of-work or explicit transaction cannot complete (commit or rollback).
/// </summary>
/// <param name="reason">Failure detail.</param>
public class TransactionException(string reason) : InfrastructureException($"Transaction failed: {reason}",
    ExceptionType.Transaction);
