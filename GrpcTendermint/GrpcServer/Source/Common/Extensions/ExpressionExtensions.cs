using System;
using System.Linq.Expressions;

namespace GrpcServer.Source.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> ConvertToWhereClause<T>(this Expression<Func<T, object>> exp, T o) where T : class, new()
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            var memberExp = (MemberExpression)exp.Body;
            var objPropExp = Expression.PropertyOrField(Expression.Constant(o), memberExp.Member.Name);
            var equalExp = Expression.Equal(exp.Body, objPropExp);
            var exp2 = Expression.Lambda<Func<T, bool>>(equalExp, exp.Parameters);
            return exp2;
        }
    }
}
