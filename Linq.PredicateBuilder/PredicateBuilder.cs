using System.Linq.Expressions;

namespace Linq.PredicateBuilder;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() => x => true;

    public static Expression<Func<T, bool>> False<T>() => x => false;

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => MergeExpression(Expression.AndAlso, left, right);

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => MergeExpression(Expression.OrElse, left, right);

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        => Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);

    private static Expression<Func<T, bool>> MergeExpression<T>(
        Func<Expression, Expression, Expression> operation,
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right
    )
    {
        var map = CreateMap(left, right);
        var visitor = new ParameterVisitor(map);
        var body = operation(left.Body, visitor.Visit(right.Body));
        return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
    }

    /// <summary>
    /// Tạo dictionary có key là parameter cần thay thế, value là parameter mới
    /// </summary>
    private static Dictionary<ParameterExpression, ParameterExpression> CreateMap<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right
    )
    {
        return left.Parameters
            .Select((parameter, idx) => new
            {
                RightParameter = right.Parameters[idx],
                LeftParameter = parameter
            })
            .ToDictionary(
                x => x.RightParameter,
                x => x.LeftParameter
            );
    }
}