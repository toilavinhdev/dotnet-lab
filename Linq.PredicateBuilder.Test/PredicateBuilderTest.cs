namespace Linq.PredicateBuilder.Test;

public class PredicateBuilderTest
{
    private readonly List<WeatherForecast> _data =
    [
        new("C", 3, false), new("A", 1, true), new("B", 2, false),
        new("D", 8, false), new("E", 20, true), new("F", 11, false),
    ];

    [Fact]
    public void Test()
    {
        var expected = _data
            .Where(x => !x.Deleted && (x.Value > 3 || x.Value < 8))
            .OrderBy(x => x.Name)
            .ToList();

        var predicate = PredicateBuilder.True<WeatherForecast>();
        predicate = predicate.And(x => !x.Deleted);
        predicate = predicate.And(x => x.Value > 3 || x.Value < 8);
        var actual = _data
            .Where(predicate.Compile())
            .OrderBy(x => x.Name)
            .ToList();

        Assert.Equal(expected, actual);
    }
}