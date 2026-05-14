using Odex.AspNetCore.Clarc.Domain.Contexts;
using Odex.AspNetCore.Clarc.Domain.Repositories;

namespace Odex.AspNetCore.Clarc.Infrastructure.Contexts;

/// <summary>
/// Default infrastructure implementation of <see cref="ITransactionContext"/> for unit-of-work code that runs
/// <c>ExecuteInTransactionAsync</c> delegates on <see cref="IBaseRepository"/> which receive an <see cref="ITransactionContext"/>.
/// Domain / application code calls <see cref="MarkForRollback"/>; your persistence adapter calls <see cref="NotifyCommitted"/> or <see cref="NotifyRolledBack"/> after the store transaction completes.
/// </summary>
public sealed class TransactionContext : ITransactionContext
{
    private string? _rollbackReason;

    /// <inheritdoc />
    public void MarkForRollback(string? reason)
    {
        ShouldRollback = true;
        _rollbackReason = reason;
    }

    /// <inheritdoc />
    public bool ShouldRollback { get; private set; }

    /// <inheritdoc />
    public bool IsCommitted { get; private set; }

    /// <inheritdoc />
    public bool IsRolledBack { get; private set; }

    /// <summary>
    /// Gets the last reason passed to <see cref="MarkForRollback"/>, if any.
    /// </summary>
    public string? RollbackReason => _rollbackReason;

    /// <summary>
    /// Marks the ambient work as committed. No-op if <see cref="ShouldRollback"/> is true or a terminal state was already set.
    /// </summary>
    public void NotifyCommitted()
    {
        if (ShouldRollback || IsCommitted || IsRolledBack)
            return;

        IsCommitted = true;
    }

    /// <summary>
    /// Marks the ambient work as rolled back. No-op if already committed.
    /// </summary>
    public void NotifyRolledBack()
    {
        if (IsCommitted)
            return;

        IsRolledBack = true;
    }
}
