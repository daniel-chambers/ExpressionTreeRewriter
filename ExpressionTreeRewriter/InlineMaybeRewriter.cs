using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DigitallyCreated.ExpressionTreeRewriter
{
    /// <summary>
    /// The <see cref="InlineMaybeRewriter"/> rewrites a 
    /// <see cref="RewriterMarkers.InlineMaybe{T,TReturn}(T,System.Func{T,TReturn})"/> method call
    /// into a ternary expression that is the inlined form of the Maybe function.
    /// </summary>
    public class InlineMaybeRewriter : IExpressionRewriter
    {
        /// <inheritdoc />
        public Expression RewriteMethodCall(MethodCallExpression methodCallExpression)
        {
            var genericArgs = methodCallExpression.Method.GetGenericArguments();
            var tType = genericArgs[0];
            var returnType = genericArgs[1];
            var objParamType = methodCallExpression.Method.GetParameters()[0].ParameterType;

            var objExpr = methodCallExpression.Arguments[0];
            var funcExpr = (LambdaExpression)methodCallExpression.Arguments[1];

            var funcBody = InlineFunctionToExpression(tType, objExpr, objParamType, funcExpr);

            var defaultReturnTypeValue = returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
            
            return Expression.Condition(
                Expression.NotEqual(objExpr, Expression.Constant(null, objParamType)),
                funcBody,
                Expression.Constant(defaultReturnTypeValue, returnType));
        }

        private static Expression InlineFunctionToExpression(
            Type tType, Expression objExpr, Type objParamType, LambdaExpression funcExpr)
        {
            var funcArgExpr = tType.IsValueType
                ? Expression.MakeMemberAccess(objExpr, objParamType.GetProperty("Value"))
                : objExpr;

            var replacements = new Dictionary<Expression, Expression> { { funcExpr.Parameters[0], funcArgExpr } };
            return new ExpressionReplacerVisitor(replacements).Visit(funcExpr.Body);
        }
    }
}