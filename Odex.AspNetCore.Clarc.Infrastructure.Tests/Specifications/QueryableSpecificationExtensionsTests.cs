using System.Linq.Expressions;
using Odex.AspNetCore.Clarc.Domain.Specifications;
using Odex.AspNetCore.Clarc.Infrastructure.Data.Specifications;
using Odex.AspNetCore.Clarc.Infrastructure.Tests.QueryBuilders;

namespace Odex.AspNetCore.Clarc.Infrastructure.Tests.Specifications;

public sealed class ValueAtLeastSpecification : Specification<TestEntity>
{
    private readonly int _min;

    public ValueAtLeastSpecification(int min) => _min = min;

    public override Expression<Func<TestEntity, bool>> ToExpression() => e => e.Value >= _min;
}

public class QueryableSpecificationExtensionsTests
{
    [Fact]
    public void Where_applies_specification_expression()
    {
        var data = new[] { new TestEntity { Value = 1 }, new TestEntity { Value = 5 }, new TestEntity { Value = 10 } }
            .AsQueryable();
        var filtered = data.Where(new ValueAtLeastSpecification(5)).ToList();
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, e => Assert.True(e.Value >= 5));
    }
}
