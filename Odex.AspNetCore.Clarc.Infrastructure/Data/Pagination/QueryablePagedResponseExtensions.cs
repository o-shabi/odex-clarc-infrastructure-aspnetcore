using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;
using Odex.AspNetCore.Clarc.Domain.ValueObjects.Responses;

namespace Odex.AspNetCore.Clarc.Infrastructure.Data.Pagination;

/// <summary>
/// Materializes <see cref="PagedResponse{T}"/> from an <see cref="IQueryable{T}"/> and <see cref="PagedRequest"/> without taking a dependency on a specific ORM.
/// </summary>
public static class QueryablePagedResponseExtensions
{
    /// <summary>
    /// Executes a total count and a paged projection synchronously via <see cref="Queryable"/> operators.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="query">Filter before calling; this method does not add filters.</param>
    /// <param name="request">Paging window; uses <see cref="PagedRequest.SkipCount"/> and <see cref="PagedRequest.PageSize"/>.</param>
    /// <returns>A <see cref="PagedResponse{T}"/> with the current page and total row count.</returns>
    /// <remarks>Most LINQ providers issue separate commands for <c>Count()</c> and for the paged sequence. For async database I/O, use the <c>ToPagedResponseAsync</c> overloads on this class.</remarks>
    public static PagedResponse<T> ToPagedResponse<T>(this IQueryable<T> query, PagedRequest request)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);

        var total = query.Count();
        var data = query.Skip(request.SkipCount).Take(request.PageSize).ToList();
        return new PagedResponse<T>(data, total);
    }

    /// <summary>
    /// Materializes a page asynchronously using caller-supplied executors (for example EF Core <c>CountAsync</c> / <c>ToListAsync</c> from the host project).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="query">Base query (filters, includes, sorts) before paging.</param>
    /// <param name="request">Paging window.</param>
    /// <param name="countAsync">Returns total matches for <paramref name="query"/> (unpaged).</param>
    /// <param name="pageAsync">Returns the current page for an already-sliced <see cref="IQueryable{T}"/>.</param>
    /// <param name="cancellationToken">Token to cancel the operations.</param>
    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        PagedRequest request,
        Func<IQueryable<T>, CancellationToken, Task<int>> countAsync,
        Func<IQueryable<T>, CancellationToken, Task<List<T>>> pageAsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(countAsync);
        ArgumentNullException.ThrowIfNull(pageAsync);

        var total = await countAsync(query, cancellationToken).ConfigureAwait(false);
        var paged = query.Skip(request.SkipCount).Take(request.PageSize);
        var data = await pageAsync(paged, cancellationToken).ConfigureAwait(false);
        return new PagedResponse<T>(data, total);
    }

    /// <summary>
    /// Builds a <see cref="PagedResponse{T}"/> when the total is already known (avoids a count query).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="query">Base query; paging is applied inside this method.</param>
    /// <param name="request">Paging window.</param>
    /// <param name="total">Total number of matching rows (must be consistent with <paramref name="query"/>).</param>
    /// <param name="pageAsync">Materializes <paramref name="query"/> after <c>Skip</c>/<c>Take</c>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        PagedRequest request,
        int total,
        Func<IQueryable<T>, CancellationToken, Task<List<T>>> pageAsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(pageAsync);

        var paged = query.Skip(request.SkipCount).Take(request.PageSize);
        var data = await pageAsync(paged, cancellationToken).ConfigureAwait(false);
        return new PagedResponse<T>(data, total);
    }
}
