using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace DigitallyCreated.ExpressionTreeRewriter
{
	/// <summary>
	/// The <see cref="LambdaInlinerRewriter"/> inlines the specified lambda expression into the expression tree
	/// it is rewriting at the marker method location.
	/// </summary>
	/// <remarks>
	/// The marker method and the lambda expression must take and return exactly the same parameters.
	/// </remarks>
	public class LambdaInlinerRewriter : IExpressionRewriter
	{
		private readonly LambdaExpression _treeToInline;


		/// <summary>
		/// Constructor, creates a <see cref="LambdaInlinerRewriter"/>
		/// </summary>
		/// <param name="treeToInline">The lambda expression to inline</param>
		public LambdaInlinerRewriter(LambdaExpression treeToInline)
		{
			_treeToInline = treeToInline;
		}


		/// <inheritdoc />
		public Expression RewriteMethodCall(MethodCallExpression methodCallExpression)
		{
			if (methodCallExpression.Type != _treeToInline.ReturnType)
				throw new InvalidOperationException(String.Format("Cannot inline lambda; the lambda's return type ({0}) is not the marker method return type ({1})", _treeToInline.Type, methodCallExpression.Type));

			if (methodCallExpression.Arguments.Count != _treeToInline.Parameters.Count)
				throw new InvalidOperationException(String.Format("Cannot inline lambda; marker method parameter count ({0}) is different to lambda parameter count ({1})", methodCallExpression.Arguments.Count, _treeToInline.Parameters.Count));

			var replacements = new Dictionary<Expression, Expression>();
			for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
			{
				if (_treeToInline.Parameters[i].Type != methodCallExpression.Arguments[i].Type)
					throw new InvalidOperationException(String.Format("Cannot inline lambda; marker method parameter (#{0}) type ({1}) is not the lambda parameter type ({2})", i, methodCallExpression.Arguments[i].Type, _treeToInline.Parameters[i].Type));

				replacements.Add(_treeToInline.Parameters[i], methodCallExpression.Arguments[i]);
			}

			return new ExpressionReplacerVisitor(replacements).Visit(_treeToInline.Body);
		}
	}
}