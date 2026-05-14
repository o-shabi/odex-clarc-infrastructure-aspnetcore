namespace Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

/// <summary>
/// Base class for a fluent <see cref="IQueryable{T}"/> pipeline (filters, includes, sorting) with flags that record what was applied.
/// </summary>
/// <typeparam name="TEntity">Entity type (reference type).</typeparam>
/// <typeparam name="TIncludes">Discriminator or marker type for include paths (for example an enum).</typeparam>
/// <param name="query">Starting query from your stack (any component that exposes <see cref="IQueryable{TEntity}"/>).</param>
public abstract class BaseQueryBuilder<TEntity, TIncludes>(IQueryable<TEntity> query)
    where TEntity : class
{
    private IQueryable<TEntity> _query = query ?? throw new ArgumentNullException(nameof(query));

    /// <summary>
    /// Gets a value indicating whether a filter (Where) was applied via <see cref="MarkAsFiltered"/>.
    /// </summary>
    protected bool IsFiltered { get; private set; }

    /// <summary>
    /// Gets a value indicating whether eager-loading was applied via <see cref="MarkAsHasIncludes"/>.
    /// </summary>
    protected bool HasIncludes { get; private set; }

    /// <summary>
    /// Gets a value indicating whether ordering was applied via <see cref="MarkAsHasSorts"/>.
    /// </summary>
    protected bool HasSorts { get; private set; }

    /// <summary>
    /// Records that a filter was applied to the query.
    /// </summary>
    protected void MarkAsFiltered() => IsFiltered = true;

    /// <summary>
    /// Records that includes were applied to the query.
    /// </summary>
    protected void MarkAsHasIncludes() => HasIncludes = true;

    /// <summary>
    /// Records that sorting was applied to the query.
    /// </summary>
    protected void MarkAsHasSorts() => HasSorts = true;

    /// <summary>
    /// Composes the current query with <paramref name="expression"/> (for example <c>Where</c> or <c>Include</c>).
    /// </summary>
    /// <param name="expression">Transformation applied to the current query.</param>
    /// <returns>The query after transformation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expression"/> is null, or it returned null.</exception>
    protected IQueryable<TEntity> ModifyQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var next = expression(_query);
        ArgumentNullException.ThrowIfNull(next);
        _query = next;
        return _query;
    }

    /// <summary>
    /// Override to apply eager-loading based on <paramref name="includes"/>.
    /// Call <see cref="MarkAsHasIncludes"/> when you change the query for includes (typically when <paramref name="includes"/> is non-empty).
    /// </summary>
    /// <param name="includes">Requested include paths, or <c>null</c> if none.</param>
    protected virtual void ApplyIncludes(IReadOnlyList<TIncludes>? includes)
    {
    }

    /// <summary>
    /// Override to apply ordering.
    /// Call <see cref="MarkAsHasSorts"/> when you add ordering to the query.
    /// </summary>
    protected virtual void ApplySorts()
    {
    }

    /// <summary>
    /// Returns the composed query. Override to orchestrate <see cref="ApplyIncludes"/>, <see cref="ApplySorts"/>, pagination, etc.
    /// </summary>
    /// <returns>The current <see cref="IQueryable{TEntity}"/>.</returns>
    public virtual IQueryable<TEntity> Build() => _query;
}
