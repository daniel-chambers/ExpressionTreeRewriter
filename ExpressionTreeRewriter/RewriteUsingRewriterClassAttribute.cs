using System;


namespace DigitallyCreated.ExpressionTreeRewriter
{
	/// <summary>
	/// Marks a method as a method that should be replaced by the <see cref="ExpressionTreeRewriter"/> using an
	/// instance of the class specified by <see cref="RewriterClass"/>.
	/// </summary>
	public class RewriteUsingRewriterClassAttribute : RewriterMarkerMethodAttribute
	{
		private readonly Type _rewriterClass;


		/// <summary>
		/// The type of the class that can rewrite the method call into another expression.
		/// This type will implement <see cref="IExpressionRewriter"/>
		/// </summary>
		public Type RewriterClass
		{
			get { return _rewriterClass; }
		}


		/// <summary>
		/// Constructor, creates a <see cref="RewriteUsingRewriterClassAttribute"/>
		/// </summary>
		/// <param name="rewriterClass">
		/// The type of the class that can rewrite the method call into another expression.
		/// This type must implement <see cref="IExpressionRewriter"/>
		/// </param>
		public RewriteUsingRewriterClassAttribute(Type rewriterClass)
		{
			if (typeof(IExpressionRewriter).IsAssignableFrom(rewriterClass) == false)
				throw new ArgumentException("rewriterClass must implement IExpressionRewriter", "rewriterClass");

			_rewriterClass = rewriterClass;
		}


		/// <summary>
		/// Creates an <see cref="IExpressionRewriter"/> that can rewrite out the marker method
		/// this attribute is applied to.
		/// </summary>
		/// <returns>The <see cref="IExpressionRewriter"/></returns>
		public override IExpressionRewriter CreateRewriter()
		{
			return (IExpressionRewriter)Activator.CreateInstance(_rewriterClass);
		}
	}
}