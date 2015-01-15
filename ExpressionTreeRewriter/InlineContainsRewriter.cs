using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DigitallyCreated.ExpressionTreeRewriter
{
    /// <summary>
    /// The <see cref="InlineContainsRewriter"/> rewrites a <see cref="RewriterMarkers.InlineContains{T}"/>
    /// method call into a boolean OR binary expression using the contents of the collection as it
    /// was at rewrite-time.
    /// </summary>
    public class InlineContainsRewriter : IExpressionRewriter
    {
        /// <inheritdoc />
        public Expression RewriteMethodCall(MethodCallExpression methodCallExpression)
        {
            var collectionExpr = methodCallExpression.Arguments[0];
            var itemExpr = methodCallExpression.Arguments[1];
            var collectionContainsType = methodCallExpression.Method.GetParameters()[1].ParameterType;

            var values = GetCollectionValues(collectionExpr, collectionContainsType);
            return BuildBinaryExpression(values, itemExpr, collectionContainsType);
        }

        private static IEnumerable<object> GetCollectionValues(Expression collectionExpr, Type collectionContainsType)
        {
            var expr = collectionExpr;

            if (collectionContainsType.IsValueType)
            {
                var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(typeof(object));
                expr = Expression.Call(castMethod, expr);
            }

            return Expression.Lambda<Func<IEnumerable<object>>>(expr).Compile()();
        }

        private static Expression BuildBinaryExpression(IEnumerable<object> values, Expression itemExpr, Type collectionContainsType)
        {
            Expression binaryExpressionTree = null;

            foreach (var item in values)
            {
                var constant = Expression.Constant(item, collectionContainsType);
                var comparison = Expression.Equal(itemExpr, constant);

                binaryExpressionTree = binaryExpressionTree == null 
                    ? comparison 
                    : Expression.OrElse(binaryExpressionTree, comparison);
            }

            return binaryExpressionTree ?? Expression.Constant(false);
        }
    }
}