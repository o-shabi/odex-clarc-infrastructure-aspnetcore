using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when required configuration is missing or invalid.
/// </summary>
/// <param name="name">Configuration section or key name.</param>
/// <param name="reason">Why the configuration is considered invalid.</param>
public class ConfigurationException(string name, string reason = "No reason provided") : InfrastructureException(
    $"'{name}' configuration failed: {reason}",
    ExceptionType.Configuration);
