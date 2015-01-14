using System;
using System.Collections.Generic;
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
            var enumContainsType = methodCallExpression.Method.GetParameters()[1].ParameterType;

            var values = GetCollectionValues(collectionExpr);
            return BuildBinaryExpression(values, itemExpr, enumContainsType);
        }

        private static IEnumerable<object> GetCollectionValues(Expression collectionExpr)
        {
            return Expression.Lambda<Func<IEnumerable<object>>>(collectionExpr).Compile()();
        }

        private static Expression BuildBinaryExpression(IEnumerable<object> values, Expression itemExpr, Type enumContainsType)
        {
            Expression binaryExpressionTree = null;

            foreach (var item in values)
            {
                var constant = Expression.Constant(item, enumContainsType);
                var comparison = Expression.Equal(itemExpr, constant);

                binaryExpressionTree = binaryExpressionTree == null 
                    ? comparison 
                    : Expression.OrElse(binaryExpressionTree, comparison);
            }

            return binaryExpressionTree ?? Expression.Constant(false);
        }
    }
}