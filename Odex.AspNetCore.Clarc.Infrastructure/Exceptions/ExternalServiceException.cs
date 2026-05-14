using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when an outbound call to an external dependency fails or returns an error status.
/// </summary>
/// <param name="serviceName">Logical name of the remote service.</param>
/// <param name="statusCode">HTTP status code if applicable; otherwise 0.</param>
/// <param name="message">Response reason or exception message.</param>
public class ExternalServiceException(string serviceName, int statusCode, string message) : InfrastructureException(
    $"External service '{serviceName}' failed ({statusCode}): {message}",
    ExceptionType.ExternalServiceFailed);
