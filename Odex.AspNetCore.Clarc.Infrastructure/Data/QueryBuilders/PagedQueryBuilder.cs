using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;

namespace Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

/// <summary>
/// Extends <see cref="BaseQueryBuilder{TEntity,TIncludes}"/> with <see cref="PagedRequest"/>-based <c>Skip</c>/<c>Take</c> using <see cref="PagedRequest.SkipCount"/> and <see cref="PagedRequest.PageSize"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type (reference type).</typeparam>
/// <typeparam name="TIncludes">Include path discriminator type.</typeparam>
public abstract class PagedQueryBuilder<TEntity, TIncludes> : BaseQueryBuilder<TEntity, TIncludes>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedQueryBuilder{TEntity,TIncludes}"/> class.
    /// </summary>
    /// <param name="query">Starting query.</param>
    /// <param name="request">Pagination request; <see cref="PagedRequest.Page"/> and <see cref="PagedRequest.PageSize"/> are normalized by the domain type (page ≥ 1, page size clamped 1–512).</param>
    protected PagedQueryBuilder(IQueryable<TEntity> query, PagedRequest request)
        : base(query) =>
        Request = request ?? throw new ArgumentNullException(nameof(request));

    /// <summary>
    /// Gets the pagination and sort inputs for this builder.
    /// </summary>
    protected PagedRequest Request { get; }

    /// <summary>
    /// Applies <c>Skip(<see cref="PagedRequest.SkipCount"/>).Take(<see cref="PagedRequest.PageSize"/>)</c> to the query.
    /// </summary>
    protected virtual void ApplyPagination()
    {
        ModifyQuery(q => q.Skip(Request.SkipCount).Take(Request.PageSize));
    }
}
