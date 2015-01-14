using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DigitallyCreated.ExpressionTreeRewriter;
using Xunit;
using Xunit.Should;

namespace ExpressionTreeRewriter.Tests
{
    public class RewriteUsingLambdaPropertyFacts
    {
        private readonly KeyValuePair<string, string>[] _data =
        {
            new KeyValuePair<string, string>("Darth", "Vader"),
            new KeyValuePair<string, string>("Luke", "Skywalker"),
            new KeyValuePair<string, string>("Han", "Solo"),
        };

        [Fact]
        public void CanBeUsedToInlineStaticData()
        {
            var results = _data.AsQueryable()
                .Where(d => GetStringArray().Contains(d.Key))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(1);
            results[0].ShouldBe(_data[1]);
        }

        [Fact]
        public void InlinesMethodsWithParameters()
        {
            var results = _data.AsQueryable()
                .Where(d => IsLuke(d))
                .Rewrite()
                .ToList();

            results.Count.ShouldBe(1);
            results[0].ShouldBe(_data[1]);
        }

        public static Expression<Func<string[]>> MyStringArray
        {
            get { return () => new[] { "Leia", "Chewbacca", "Luke" }; }
        }

        [RewriteUsingLambdaProperty(typeof(RewriteUsingLambdaPropertyFacts), "MyStringArray")]
        private static string[] GetStringArray()
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<KeyValuePair<string, string>, bool>> IsLukeExpr
        {
            get { return kvp => kvp.Key == "Luke" && kvp.Value == "Skywalker"; }
        }

        [RewriteUsingLambdaProperty(typeof(RewriteUsingLambdaPropertyFacts), "IsLukeExpr")]
        private static bool IsLuke(KeyValuePair<string, string> kvp)
        {
            throw new NotImplementedException();
        }
    }
}