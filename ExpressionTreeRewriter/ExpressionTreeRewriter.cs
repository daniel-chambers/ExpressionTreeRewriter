using System;
using System.Linq;
using System.Linq.Expressions;

namespace DigitallyCreated.ExpressionTreeRewriter
{
    /// <summary>
    /// The <see cref="ExpressionTreeRewriter"/> allows you to write expression trees that include  marker
    /// methods that will later be rewritten out and replaced with some other expression.
    /// </summary>
    /// <remarks>
    /// Marker methods are ones that are attributed with the <see cref="RewriterMarkerMethodAttribute"/>; they
    /// should throw an <see cref="InvalidOperationException"/> as they do not perform any action other than act
    /// as a placeholder in the expression tree. The rewriter then uses an instance of the 
    /// <see cref="IExpressionRewriter"/> class specified by the <see cref="RewriterMarkerMethodAttribute"/> to
    /// replace the method call with another expression.
    /// </remarks>
    public static class ExpressionTreeRewriter
    {
        /// <summary>
        /// Rewrites an <see cref="IQueryable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="queryable">The queryable</param>
        /// <returns>The rewritten queryable</returns>
        public static IQueryable<T> Rewrite<T>(this IQueryable<T> queryable)
        {
            Expression newExpression = Rewrite(queryable.Expression);
            return queryable.Provider.CreateQuery<T>(newExpression);
        }


        /// <summary>
        /// Rewrites an arbitrary expression tree
        /// </summary>
        /// <param name="expression">The expression tree to rewrite</param>
        /// <returns>The rewritten expression tree</returns>
        public static Expression Rewrite(Expression expression)
        {
            return new TreeVisitor().Visit(expression);
        }


        /// <summary>
        /// An <see cref="ExpressionVisitor"/> that rewrites all marker method calls with their replacement
        /// expression.
        /// </summary>
        private class TreeVisitor : ExpressionVisitor
        {
            /// <summary>
            /// Visits the children of the <see cref="MethodCallExpression"/>.
            /// </summary>
            /// <param name="methodCallExpr">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original
            /// expression.
            /// </returns>
            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpr)
            {
                RewriterMarkerMethodAttribute markerAttribute = methodCallExpr.Method
                    .GetCustomAttributes(typeof(RewriterMarkerMethodAttribute), false)
                    .Cast<RewriterMarkerMethodAttribute>()
                    .FirstOrDefault();

                if (markerAttribute == null)
                    return base.VisitMethodCall(methodCallExpr);

                IExpressionRewriter rewriter = markerAttribute.CreateRewriter();
                return Visit(rewriter.RewriteMethodCall(methodCallExpr));
            }
        }
    }
}
