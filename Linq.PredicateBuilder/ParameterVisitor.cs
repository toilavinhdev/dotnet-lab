using System.Linq.Expressions;

namespace Linq.PredicateBuilder;

public sealed class ParameterVisitor(Dictionary<ParameterExpression, ParameterExpression> map) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) => map.GetValueOrDefault(node, node);
}