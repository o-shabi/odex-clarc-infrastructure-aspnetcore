using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when an ID, token, or similar generator fails.
/// </summary>
/// <param name="name">Generator or algorithm name.</param>
/// <param name="reason">Failure detail.</param>
public class GeneratorException(string name, string reason = "No reason provided")
    : InfrastructureException($"Generator '{name}' threw an error: {reason}", ExceptionType.Generator);
