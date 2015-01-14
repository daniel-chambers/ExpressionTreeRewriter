using System;
using System.Linq.Expressions;
using System.Reflection;


namespace DigitallyCreated.ExpressionTreeRewriter
{
	/// <summary>
	/// Marks a method as a method that should be replaced by the <see cref="ExpressionTreeRewriter"/> by
	/// inlining the lambda expression returned by the public static property specified.
	/// </summary>
	/// <remarks>
	/// The marker method and the lambda expression returned by the property must take and return exactly the
	/// same parameters.
	/// </remarks>
	public class RewriteUsingLambdaPropertyAttribute : RewriterMarkerMethodAttribute
	{
		private readonly PropertyInfo _propertyInfo;


		/// <summary>
		/// The public static property that returns the lambda
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get { return _propertyInfo; }
		}


		/// <summary>
		/// Constructor, creates a <see cref="RewriteUsingLambdaPropertyAttribute"/>
		/// </summary>
		/// <param name="type">The type with the static property</param>
		/// <param name="staticPropertyName">The name of the public static property</param>
		public RewriteUsingLambdaPropertyAttribute(Type type, string staticPropertyName)
		{
			_propertyInfo = type.GetProperty(staticPropertyName, BindingFlags.Public | BindingFlags.Static);
			if (PropertyInfo == null)
				throw new ArgumentException(String.Format("No public static property called {0} on {1} was found.", staticPropertyName, type));
			if (typeof(LambdaExpression).IsAssignableFrom(PropertyInfo.PropertyType) == false)
				throw new ArgumentException(String.Format("{0}.{1} must return a LambdaExpression", type, staticPropertyName));
		}


		/// <summary>
		/// Creates an <see cref="IExpressionRewriter"/> that can rewrite out the marker method
		/// this attribute is applied to.
		/// </summary>
		/// <returns>The <see cref="IExpressionRewriter"/></returns>
		public override IExpressionRewriter CreateRewriter()
		{
			return new LambdaInlinerRewriter((LambdaExpression)PropertyInfo.GetValue(null, null));
		}
	}
}