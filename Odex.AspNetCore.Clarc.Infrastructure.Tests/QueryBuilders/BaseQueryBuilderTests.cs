using Odex.AspNetCore.Clarc.Infrastructure.Data.QueryBuilders;

namespace Odex.AspNetCore.Clarc.Infrastructure.Tests.QueryBuilders;

public sealed class TestEntity
{
    public int Value { get; init; }
}

public sealed class TestEntityQueryBuilder : BaseQueryBuilder<TestEntity, string>
{
    public TestEntityQueryBuilder(IQueryable<TestEntity> query)
        : base(query)
    {
    }

    public IQueryable<TestEntity> PublicModifyQuery(Func<IQueryable<TestEntity>, IQueryable<TestEntity>> fn) =>
        ModifyQuery(fn);

    public void PublicMarkFiltered() => MarkAsFiltered();
}

public class BaseQueryBuilderTests
{
    [Fact]
    public void Constructor_throws_when_query_null()
    {
        Assert.Throws<ArgumentNullException>(() => new TestEntityQueryBuilder(null!));
    }

    [Fact]
    public void ModifyQuery_throws_when_expression_null()
    {
        var builder = new TestEntityQueryBuilder(Array.Empty<TestEntity>().AsQueryable());
        Assert.Throws<ArgumentNullException>(() => builder.PublicModifyQuery(null!));
    }

    [Fact]
    public void ModifyQuery_throws_when_expression_returns_null()
    {
        var builder = new TestEntityQueryBuilder(Array.Empty<TestEntity>().AsQueryable());
        Assert.Throws<ArgumentNullException>(() => builder.PublicModifyQuery(_ => null!));
    }

    [Fact]
    public void ModifyQuery_composes_and_Build_returns_filtered_sequence()
    {
        var items = new[] { new TestEntity { Value = 1 }, new TestEntity { Value = 2 } }.AsQueryable();
        var builder = new TestEntityQueryBuilder(items);
        builder.PublicModifyQuery(q => q.Where(x => x.Value > 1));
        builder.PublicMarkFiltered();
        var list = builder.Build().ToList();
        Assert.Single(list);
        Assert.Equal(2, list[0].Value);
    }
}
