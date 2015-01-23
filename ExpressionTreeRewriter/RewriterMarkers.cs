using System;
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

        /// <summary>
        /// If <paramref name="obj"/> is not null, calls <paramref name="func"/> and returns
        /// its return value, otherwise returns the default value of <typeparamref name="TReturn"/>.
        /// </summary>
        /// <remarks>
        /// When used in an expression tree, this call site will be transformed into ternary
        /// expression that performs the Maybe logic inline in the expression tree.
        /// </remarks>
        /// <typeparam name="T">The nullable reference type</typeparam>
        /// <typeparam name="TReturn">The type being returned by the function</typeparam>
        /// <param name="obj">The reference being tested for null</param>
        /// <param name="func">
        /// A function that will be called and passed <paramref name="obj"/> if <paramref name="obj"/> 
        /// is not null
        /// </param>
        /// <returns>
        /// The result of running <paramref name="func"/>, or the default value of 
        /// <typeparamref name="TReturn"/>
        /// </returns>
        [RewriteUsingRewriterClass(typeof(InlineMaybeRewriter))]
        public static TReturn InlineMaybe<T, TReturn>(this T obj, Func<T, TReturn> func)
            where T : class
        {
            return obj != null
                ? func(obj)
                : default(TReturn);
        }

        /// <summary>
        /// If <paramref name="obj"/> is not null, calls <paramref name="func"/> and returns
        /// its return value, otherwise returns the default value of <typeparamref name="TReturn"/>.
        /// </summary>
        /// <remarks>
        /// When used in an expression tree, this call site will be transformed into ternary
        /// expression that performs the Maybe logic inline in the expression tree.
        /// </remarks>
        /// <typeparam name="T">The nullable value type</typeparam>
        /// <typeparam name="TReturn">The type being returned by the function</typeparam>
        /// <param name="obj">The nullable value being tested for null</param>
        /// <param name="func">
        /// A function that will be called and passed <paramref name="obj"/> if <paramref name="obj"/> 
        /// is not null
        /// </param>
        /// <returns>
        /// The result of running <paramref name="func"/>, or the default value of 
        /// <typeparamref name="TReturn"/>
        /// </returns>
        [RewriteUsingRewriterClass(typeof(InlineMaybeRewriter))]
        public static TReturn InlineMaybe<T, TReturn>(this T? obj, Func<T, TReturn> func)
            where T : struct
        {
            return obj.HasValue
                ? func(obj.Value)
                : default(TReturn);
        }
	}
}