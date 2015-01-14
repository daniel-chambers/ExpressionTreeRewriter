using System.Collections.Generic;
using System.Linq;
using DigitallyCreated.ExpressionTreeRewriter;
using Xunit;
using Xunit.Should;

namespace ExpressionTreeRewriter.Tests
{
    public class InlineContainsFacts
    {
        private readonly KeyValuePair<string, string>[] _data =
        {
            new KeyValuePair<string, string>("Darth", "Vader"),
            new KeyValuePair<string, string>("Luke", "Skywalker"),
            new KeyValuePair<string, string>("Han", "Solo"),
        };
            
        [Fact]
        public void ArrayWithContentsIsUsedCorrectly()
        {
            var strs = new[] { "Leia", "Chewbacca", "Luke" };

            var results = _data.AsQueryable()
                .Where(d => strs.InlineContains(d.Key))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(1);
            results[0].ShouldBe(_data[1]);
        }

        [Fact]
        public void EmptyArrayResultsInNoMatches()
        {
            var strs = new string[0];

            var results = _data.AsQueryable()
                .Where(d => strs.InlineContains(d.Key))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(0);
        }
    }
}
