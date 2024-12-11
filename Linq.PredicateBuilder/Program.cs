﻿using System.Linq.Expressions;
using Linq.PredicateBuilder;

// TODO(1): Expected expression
{
    Expression<Func<WeatherForecast, bool>> expression = x => x.Name == "B" && (x.Value > 3 || !x.Deleted);
    WeatherForecast.Log("TODO(1): Expected expression", expression);
}

// TODO(2): Build expression thủ công
// And thực hiện cả (left,right) - AndAlso thực hiện (right) khi (left)=true
// Or thực hiện cả (left,right) - OrElse thực hiện (right) khi (left)=false
// -> Perfomance
{
    var parameter = Expression.Parameter(typeof(WeatherForecast), "x");

    var nameMember = Expression.PropertyOrField(parameter, "Name");
    var nameConstant = Expression.Constant("B");
    var nameExpression = Expression.Equal(nameMember, nameConstant);

    var valueMember = Expression.Property(parameter, "Value");
    var valueConstant = Expression.Constant(3);
    var valueExpression = Expression.GreaterThan(valueMember, valueConstant);

    var deletedMember = Expression.Property(parameter, "Deleted");
    var deletedExpression = Expression.Not(deletedMember);

    var valueOrDeletedExpression = Expression.OrElse(valueExpression, deletedExpression);
    var finalExpression = Expression.AndAlso(nameExpression, valueOrDeletedExpression);
    var expression = Expression.Lambda<Func<WeatherForecast, bool>>(finalExpression, parameter);
    WeatherForecast.Log("TODO(2): Build expression thủ công", expression);
}


// TODO(3): Build expression bằng composing expressions
// Các expression khác nhau thì có paramter name khác nhau
// -> Invoke chuẩn hóa parameter
// -> Sử dụng InvocationExpression
// -> Problem: LINQ không support InvocationExpression
{
    Expression<Func<WeatherForecast, bool>> nameIsB = x => x.Name == "B";
    Expression<Func<WeatherForecast, bool>> valueIs3 = x1 => x1.Value > 3;
    Expression<Func<WeatherForecast, bool>> isNotDeleted = x2 => !x2.Deleted;

    var parameter = nameIsB.Parameters[0];

    var expression = Expression.Lambda<Func<WeatherForecast, bool>>(
        Expression.AndAlso(
            nameIsB.Body,
            Expression.OrElse(
                Expression.Invoke(valueIs3, parameter),
                Expression.Invoke(isNotDeleted, parameter)
            )
        ),
        parameter
    );
    WeatherForecast.Log("TODO(3): Build expression bằng composing expressions", expression);
}

// TODO(4): Build expression bằng ExpressionPredicateBuilder
{
    Expression<Func<WeatherForecast, bool>> expression = x => true;
    expression = expression.And(x1 => x1.Name == "B");
    expression = expression.And(x2 => x2.Value > 3 || !x2.Deleted);
    WeatherForecast.Log("TODO(4): Build expression bằng ExpressionPredicateBuilder", expression);
    var func = expression.Compile();
    Console.WriteLine("Func: {0}", func);
}