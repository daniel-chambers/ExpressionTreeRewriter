using System;
using System.Linq;
using System.Linq.Expressions;
using DigitallyCreated.ExpressionTreeRewriter;
using Xunit;
using Xunit.Should;

namespace ExpressionTreeRewriter.Tests
{
    public class RecursiveRewriteFacts
    {
        private readonly string[] _names = { "Anakin Skywalker", "Obi-Wan Kenobi", "Luke Skywalker" };

        [Fact]
        public void RecursivelyRewritesAllowingRewritesInRewrites()
        {
            var results = _names.AsQueryable()
                .Where(n => IsSkywalker(n))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(1);
            results[0].ShouldBe(_names[2]);
        }

        public static Expression<Func<string, bool>> IsSkywalkerExpr
        {
            get { return n => IsLuke(n.Split(' ')); }
        }

        [RewriteUsingLambdaProperty(typeof(RecursiveRewriteFacts), "IsSkywalkerExpr")]
        private static bool IsSkywalker(string name)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<string[], bool>> IsLukeExpr
        {
            get { return ns => ns.Length == 2 && ns[0] == "Luke" && ns[1] == "Skywalker"; }
        }

        [RewriteUsingLambdaProperty(typeof(RecursiveRewriteFacts), "IsLukeExpr")]
        private static bool IsLuke(string[] ns)
        {
            throw new NotImplementedException();
        }
    }
}