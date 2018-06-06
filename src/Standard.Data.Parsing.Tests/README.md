Lightweight monadic parsing
===========================
Use the `Standard.Data.Parsing` package for lightweight text parsing. You can use this library as a more powerful alternative to regular expressions, while enjoying 
the ease of LINQ styled coding.


Getting started
---------------
Let's build a simple parser:

```C#
// parseOfA is a parser that can handle any number of capital 'A's in a row
Parser<IEnumerable<char>> parserOfA = Parse.Char('A').AtLeastOnce();
IEnumerable<char> result = parserOfA.Parse("AAAbbb");
Assert.Equal("AAA", string.Concat(result));

try
{
	IEnumerable<char> result = parserOfA.Parse("ccAAAbbb");
}
catch (Exception ex)
{
	Assert.Equal("Failed to parse input: Unexpected token: c. Expectations: expected A; Remainer: Line 1, Column 1; Last consumed: ", ex.Message);
}
```

Our `parserOfA` will return an `IEnumerable<char>` if it can successfully parse the given input. In the first case, it returns "AAA".

In the second case, it has encountered a character that it cannot handle ('c'). Therefore, an exception is produced.

Okay. Now to parse something useful, we need to combine multiple simple parsers into a bigger one. The best way to do that is to use LINQ query comprehensions:

```C#
Parser<string> identifierParser =
    from leading in Parse.WhiteSpace.Many()
    from first in Parse.Letter.Once()
    from rest in Parse.LetterOrDigit.Many()
    from trailing in Parse.WhiteSpace.Many()
    select new string(first.Concat(rest).ToArray());

string id = identifierParser.Parse(" abc123  ");
Assert.Equal("abc123", id);
```



Dev Notes
---------
rebased from commit #a286cb3 from sprache/Sprache
