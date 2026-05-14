using Odex.AspNetCore.Clarc.Infrastructure.Constants;

namespace Odex.AspNetCore.Clarc.Infrastructure.Exceptions;

/// <summary>
/// Thrown when the application cannot reach the database (connection timeout, auth failure, etc.).
/// </summary>
/// <param name="database">Logical database or connection name.</param>
/// <param name="reason">Provider or wrapper error detail.</param>
public class DatabaseConnectionException(string database, string reason)
    : InfrastructureException($"Cannot connect to '{database}': {reason}", ExceptionType.DbConnection);
