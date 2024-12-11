using System.Linq.Expressions;

namespace Linq.PredicateBuilder;

public class WeatherForecast(string name, int value, bool deleted)
{
    public string Name { get; set; } = name;

    public int Value { get; set; } = value;

    public bool Deleted { get; set; } = deleted;

    public static void Log(string title, Expression<Func<WeatherForecast, bool>> expression)
    {
        Console.WriteLine(title);
        Console.WriteLine("Expression: {0}", expression);
        Console.WriteLine("-->Body: {0}", expression.Body);
        Console.WriteLine("-->Parameters: {0}", string.Join(", ", expression.Parameters));
        Console.WriteLine();
    }
}