using System.Collections.Generic;
using System.Linq.Expressions;


namespace DigitallyCreated.ExpressionTreeRewriter
{
	/// <summary>
	/// An <see cref="ExpressionVisitor"/> that recurses through an expression tree and replaces expressions
	/// with other expressions
	/// </summary>
	public class ExpressionReplacerVisitor : ExpressionVisitor
	{
		private readonly IDictionary<Expression, Expression> _replacementExpressions;


		/// <summary>
		/// Constructor, creates an <see cref="ExpressionReplacerVisitor"/>
		/// </summary>
		/// <param name="replacementExpressions">
		/// A dictionary where the key is the expression to replace and the value is the expression to replace with
		/// </param>
		public ExpressionReplacerVisitor(IDictionary<Expression, Expression> replacementExpressions)
		{
			_replacementExpressions = replacementExpressions;
		}


		/// <summary>
		/// Dispatches the expression to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>
		/// The modified expression, if it or any subexpression was modified; otherwise, returns the original
		/// expression.
		/// </returns>
		public override Expression Visit(Expression node)
		{
			if (_replacementExpressions.ContainsKey(node))
				return Visit(_replacementExpressions[node]);

			return base.Visit(node);
		}
	}
}