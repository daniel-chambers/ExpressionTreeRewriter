# Expression Tree Rewriter
This is small utility library that can be used to rewrite expression trees using methods as marker points for the rewriting process.

## NuGet
``Install-Package DigitallyCreated.ExpressionTreeRewriter``

## Example Usage
### InlineContains
Let's say you want to write a LINQ query that filters a list of people and returns only those whose first name is in a small set of names.

```c#
var names = new[] { "Leia", "Chewbacca", "Luke" };

var results = _data.AsQueryable()
    .Where(person => names.Contains(person.FirstName))
    .ToList();
```

However, you then discover your LINQ provider doesn't support ``.Contains``. Using the rewriter, you rewrite that ``.Contains`` call into a boolean expression by doing this:

```c#
var names = new[] { "Leia", "Chewbacca", "Luke" };

var results = _data.AsQueryable()
    .Where(person => names.InlineContains(person.FirstName))
    .Rewrite()
    .ToList();
```

The rewrite process effectively rewrites that ``.InlineContains`` method (known as a marker method) into the following boolean expression:

```c#
.Where(person => person.FirstName == "Leia" || person.FirstName == "Chewbacca" || person.FirstName == "Luke")
```

### Lambda Inlining
Sometimes you wish you could extract some relatively complex logic out into another method when writing against ``IQueryable``, but you can't because the Queryable provider wouldn't understand your method. Using the rewriter, you can extract that logic out into one place and then inline it into the expression tree at runtime. This keeps your code clean and your Queryable provider happy.

To do this, we first create an expression tree that represents our common logic and return it from a public static property on a class:

```c#
public class MyClass
{
  public static Expression<Func<Person, bool>> IsLukeExpr
  {
      get { return person => person.FirstName == "Luke" && person.LastName == "Skywalker"; }
  }
  
  ...
}
```

Then, we create a marker method that we can call in our query expressions. We annotate it with the ``RewriteUsingLambdaProperty`` attribute, telling the rewriter to replace all calls to this marker method with the lambda expression returned by the specified property (in this case ``MyClass.IsLukeExpr``).

```c#
[RewriteUsingLambdaProperty(typeof(MyClass), "IsLukeExpr")]
public static bool IsLuke(Person person)
{
    throw new NotImplementedException("Should not be executed. Should be rewritten out of the expression tree.");
}
```

We can then use this marker method in our queries:

```c#
var results = _data.AsQueryable()
  .Where(person => IsLuke(person))
  .Rewrite()
  .ToList();
```

This is effectively rewritten into:
```c#
.Where(person => person.FirstName == "Luke" && person.LastName == "Skywalker")
```

## More Information
This library is based on the concepts and classes discussed in [this blog post][1].

[1]: http://www.digitallycreated.net/Blog/66/sweeping-yucky-linq-queries-under-the-rug-with-expression-tree-rewriting
