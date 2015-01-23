using System;
using System.Linq;
using Xunit;
using Xunit.Should;

namespace DigitallyCreated.ExpressionTreeRewriter.Tests
{
    public class InlineMaybeRewriterFacts
    {
        [Fact]
        public void InlineMaybeWorksWithReferenceTypeReturningValueType()
        {
            var names = new[] { "Luke", null, "Obi-Wan" };

            var lengths = names.AsQueryable()
                .Select(name => name.InlineMaybe(n => n.Length))
                .Rewrite()
                .ToArray();

            lengths.Length.ShouldBe(3);
            lengths[0].ShouldBe(4);
            lengths[1].ShouldBe(0);
            lengths[2].ShouldBe(7);
        }

        [Fact]
        public void InlineMaybeWorksWithReferenceTypeReturningReferenceType()
        {
            var names = new[] { "Luke", null, "Anakin" };

            var lengths = names.AsQueryable()
                .Select(name => name.InlineMaybe(n => n + " Skywalker"))
                .Rewrite()
                .ToArray();

            lengths.Length.ShouldBe(3);
            lengths[0].ShouldBe("Luke Skywalker");
            lengths[1].ShouldBeNull();
            lengths[2].ShouldBe("Anakin Skywalker");
        }

        [Fact]
        public void InlineMaybeWorksWithValueTypeReturningValueType()
        {
            var nums = new int?[] { 1, null, 3 };

            var largerNums = nums.AsQueryable()
                .Select(num => num.InlineMaybe((int n) => n + 1))
                .Rewrite()
                .ToArray();

            largerNums.Length.ShouldBe(3);
            largerNums[0].ShouldBe(2);
            largerNums[1].ShouldBe(0);
            largerNums[2].ShouldBe(4);
        }

        [Fact]
        public void InlineMaybeWorksWithValueTypeReturningReferenceType()
        {
            var nums = new int?[] { 1, null, 3 };

            var largerNums = nums.AsQueryable()
                .Select(num => num.InlineMaybe(n => String.Join(" ", Enumerable.Repeat("a", n))))
                .Rewrite()
                .ToArray();

            largerNums.Length.ShouldBe(3);
            largerNums[0].ShouldBe("a");
            largerNums[1].ShouldBeNull();
            largerNums[2].ShouldBe("a a a");
        }
    }
}