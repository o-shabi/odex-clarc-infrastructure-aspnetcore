using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when serialization or deserialization of a payload fails.
/// </summary>
/// <param name="name">Context label (type name, endpoint, or file name).</param>
/// <param name="innerException">Optional underlying exception.</param>
public class SerializationException(string name, Exception? innerException = null) : InfrastructureException(
    $"Failed to serialize or deserialize '{name}'",
    ExceptionType.Serialization,
    innerException);
