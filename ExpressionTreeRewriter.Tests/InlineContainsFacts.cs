using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Should;

namespace DigitallyCreated.ExpressionTreeRewriter.Tests
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
        public void ArrayWithReferenceTypeContentsIsUsedCorrectly()
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
        public void ArrayWithValueTypeContentsIsUsedCorrectly()
        {
            var strs = new[] { 1, 2, 3 };

            var data = new[] { 2, 4, 5 };
            var results = data.AsQueryable()
                .Where(d => strs.InlineContains(d))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(1);
            results[0].ShouldBe(data[0]);
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
