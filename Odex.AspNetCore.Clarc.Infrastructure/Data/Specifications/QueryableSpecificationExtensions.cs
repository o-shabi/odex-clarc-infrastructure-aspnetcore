using Odex.AspNetCore.Clarc.Domain.Specifications;

namespace Odex.AspNetCore.Clarc.Infrastructure.Data.Specifications;

/// <summary>
/// Bridges Domain <see cref="Specification{T}"/> types to <see cref="IQueryable{T}"/> filters.
/// </summary>
public static class QueryableSpecificationExtensions
{
    /// <summary>
    /// Narrows <paramref name="query"/> using the predicate from <paramref name="specification"/>.
    /// </summary>
    /// <typeparam name="T">Entity or projected type.</typeparam>
    /// <param name="query">Queryable to filter.</param>
    /// <param name="specification">Domain specification providing an expression tree.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with <c>Where</c> composed from <see cref="Specification{T}.ToExpression"/>.</returns>
    public static IQueryable<T> Where<T>(this IQueryable<T> query, Specification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);
        return query.Where(specification.ToExpression());
    }
}
