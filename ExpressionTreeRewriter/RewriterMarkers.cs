using System.Collections.Generic;
using System.Linq;


namespace DigitallyCreated.ExpressionTreeRewriter
{
	/// <summary>
	/// Marker methods that will be rewritten out of the expression trees they are used in by the 
	/// <see cref="ExpressionTreeRewriter"/>
	/// </summary>
	public static class RewriterMarkers
	{
        /// <summary>
        /// Determines whether <paramref name="item"/> is contained in <paramref name="collection"/>.
        /// </summary>
        /// <remarks>
        /// When used in an expression tree, this call site will be transformed into a boolean OR
        /// binary expression, removing the use of the collection from the expression tree and
        /// embedding its values into the expression directly.
        /// </remarks>
        /// <typeparam name="T">The type contained in the collection</typeparam>
        /// <param name="collection">The collection of items</param>
        /// <param name="item">The item to check for in the collection</param>
        /// <returns>
        /// True if <paramref name="item"/> in contained in <paramref name="collection"/>, false otherwise.
        /// </returns>
        [RewriteUsingRewriterClass(typeof(InlineContainsRewriter))]
	    public static bool InlineContains<T>(this IEnumerable<T> collection, T item)
        {
            return collection.Contains(item);
        }
	}
}