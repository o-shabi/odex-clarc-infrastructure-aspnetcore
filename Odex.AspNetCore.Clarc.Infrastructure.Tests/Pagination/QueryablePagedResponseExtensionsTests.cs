using Odex.AspNetCore.Clarc.Domain.ValueObjects.Requests;
using Odex.AspNetCore.Clarc.Infrastructure.Data.Pagination;
using Odex.AspNetCore.Clarc.Infrastructure.Tests.QueryBuilders;

namespace Odex.AspNetCore.Clarc.Infrastructure.Tests.Pagination;

public sealed record CatListRequest : PagedRequest;

public class QueryablePagedResponseExtensionsTests
{
    [Fact]
    public void ToPagedResponse_returns_total_and_page()
    {
        var data = Enumerable.Range(1, 25).Select(i => new TestEntity { Value = i }).AsQueryable();
        var request = new CatListRequest { Page = 1, PageSize = 10 };
        var page = data.ToPagedResponse(request);
        Assert.Equal(25, page.Total);
        Assert.Equal(10, page.Data.Count);
        Assert.Equal(1, page.Data[0].Value);
        Assert.Equal(10, page.Data[^1].Value);
    }

    [Fact]
    public async Task ToPagedResponseAsync_with_delegates_materializes_page()
    {
        var data = Enumerable.Range(1, 15).Select(i => new TestEntity { Value = i }).AsQueryable();
        var request = new CatListRequest { Page = 2, PageSize = 5 };
        var page = await data.ToPagedResponseAsync(
            request,
            async (q, ct) =>
            {
                await Task.Yield();
                return q.Count();
            },
            async (q, ct) =>
            {
                await Task.Yield();
                return q.ToList();
            },
            CancellationToken.None);
        Assert.Equal(15, page.Total);
        Assert.Equal(5, page.Data.Count);
        Assert.Equal(6, page.Data[0].Value);
    }

    [Fact]
    public async Task ToPagedResponseAsync_with_known_total_skips_count_delegate()
    {
        var data = Enumerable.Range(1, 8).Select(i => new TestEntity { Value = i }).AsQueryable();
        var request = new CatListRequest { Page = 1, PageSize = 3 };
        var page = await data.ToPagedResponseAsync(
            request,
            total: 8,
            async (q, ct) =>
            {
                await Task.Yield();
                return q.ToList();
            },
            CancellationToken.None);
        Assert.Equal(8, page.Total);
        Assert.Equal(3, page.Data.Count);
    }
}
