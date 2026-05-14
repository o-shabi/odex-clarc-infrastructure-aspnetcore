using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;
using Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

namespace Odex.AspNetCore.Clarc.Infrastructure.Tests.QueryBuilders;

public sealed record NumbersPagedRequest : PagedRequest;

public sealed class NumberPagedQueryBuilder : PagedQueryBuilder<TestEntity, int>
{
    public NumberPagedQueryBuilder(IQueryable<TestEntity> query, PagedRequest request)
        : base(query, request)
    {
    }

    public IQueryable<TestEntity> BuildPage()
    {
        ApplyPagination();
        return Build();
    }
}

public class PagedQueryBuilderTests
{
    [Fact]
    public void ApplyPagination_uses_skip_and_take_from_request()
    {
        var entities = Enumerable.Range(0, 25).Select(i => new TestEntity { Value = i }).AsQueryable();
        var request = new NumbersPagedRequest { Page = 2, PageSize = 10 };
        var page = new NumberPagedQueryBuilder(entities, request).BuildPage().ToList();
        Assert.Equal(10, page.Count);
        Assert.Equal(10, page[0].Value);
        Assert.Equal(19, page[^1].Value);
    }

    [Fact]
    public void Constructor_throws_when_request_null()
    {
        var entities = Array.Empty<TestEntity>().AsQueryable();
        Assert.Throws<ArgumentNullException>(() => new NumberPagedQueryBuilder(entities, null!));
    }
}
