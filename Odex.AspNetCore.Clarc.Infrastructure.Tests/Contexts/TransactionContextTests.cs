using Odex.AspNetCore.Clarc.Infrastructure.Contexts;

namespace Odex.AspNetCore.Clarc.Infrastructure.Tests.Contexts;

public class TransactionContextTests
{
    [Fact]
    public void MarkForRollback_sets_flags_and_reason()
    {
        var ctx = new TransactionContext();
        ctx.MarkForRollback("domain rule");
        Assert.True(ctx.ShouldRollback);
        Assert.Equal("domain rule", ctx.RollbackReason);
        Assert.False(ctx.IsCommitted);
        Assert.False(ctx.IsRolledBack);
    }

    [Fact]
    public void NotifyCommitted_sets_when_not_marked_for_rollback()
    {
        var ctx = new TransactionContext();
        ctx.NotifyCommitted();
        Assert.True(ctx.IsCommitted);
        Assert.False(ctx.IsRolledBack);
    }

    [Fact]
    public void NotifyCommitted_is_noop_after_MarkForRollback()
    {
        var ctx = new TransactionContext();
        ctx.MarkForRollback(null);
        ctx.NotifyCommitted();
        Assert.False(ctx.IsCommitted);
    }

    [Fact]
    public void NotifyRolledBack_sets_after_rollback_path()
    {
        var ctx = new TransactionContext();
        ctx.MarkForRollback("x");
        ctx.NotifyRolledBack();
        Assert.True(ctx.IsRolledBack);
    }

    [Fact]
    public void NotifyRolledBack_is_noop_after_committed()
    {
        var ctx = new TransactionContext();
        ctx.NotifyCommitted();
        ctx.NotifyRolledBack();
        Assert.False(ctx.IsRolledBack);
    }
}
