namespace Odex.AspNetCore.Clarc.Infrastructure.Constants;

/// <summary>
/// Infrastructure failure category carried by <see cref="Exceptions.InfrastructureException"/>.
/// Not related to domain-level exception classification.
/// </summary>
public enum ExceptionType
{
    /// <summary>Unspecified or unknown category.</summary>
    Unknown,

    /// <summary>Configuration missing or invalid.</summary>
    Configuration,

    /// <summary>Database connectivity failure.</summary>
    DbConnection,

    /// <summary>External HTTP or remote service failure.</summary>
    ExternalServiceFailed,

    /// <summary>Identifier or token generation failure.</summary>
    Generator,

    /// <summary>Persistence or repository operation failure.</summary>
    Repository,

    /// <summary>Serialization or deserialization failure.</summary>
    Serialization,

    /// <summary>Transaction commit or rollback failure.</summary>
    Transaction
}
