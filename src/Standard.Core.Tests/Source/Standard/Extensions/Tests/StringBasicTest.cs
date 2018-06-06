using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringCasingTest
    {
        [Fact]
        public void ToCamelCase() 
        {
            Assert.Equal("fooFooFoo", "Foo foo Foo".ToCamelCase());
        }

        [Fact]
        public void ToPascalCase() 
        {
            Assert.Equal("FooFooFoo", "foo foo Foo".ToPascalCase());
        }

        [Fact]
        public void FirstCharToUpper() 
        {
            Assert.Equal("Foo", "foo".FirstToUpperInvariant());
        }

        [Fact]
        public void FirstCharToLower() 
        {
            Assert.Equal("fOO", "FOO".FirstToLowerInvariant());
        }

        [Fact]
        public void ToTitleCase() 
        {
            Assert.Equal("Foo Bar Tar", "foo bar _-_tar".ToTitleCase());
        }

        [Fact]
        public void ToSentenceCase() 
        {
            Assert.Equal("Foo bar tar", "foo bar _-_tar".ToSentenceCase());
        }
    }

    public class StringEncodingTest
    {
        [Fact]
        public void ToASCIITurnsUnicodeCharsToQuestionMark() 
        {
            Assert.Equal("foo???", "foo一二三".ToASCII());
        }
    }

    public class StringEqualsTest
    {
        [Fact]
        public void EqualOrdinalIgnoreCase() 
        {
            Assert.True("foo".EqualsIgnoreCase("FOo"));
        }
    }

    public class StringReplaceTest
    {
        [Fact]
        public void ReplaceSubstringWithCount() 
        {
            Assert.Equal("footarbar", "foobarbar".Replace("bar", "tar", 1));
        }

        [Fact]
        public void ReplaceSubstringIgnoreCase() 
        {
            Assert.Equal("footar", "fooBaR".ReplaceIgnoreCase("bar", "tar"));
        }

        [Fact]
        public void RemoveSubstring() 
        {
            Assert.Equal("foo", "foobartar".Remove(new string[] { "bar", "tar" }));
        }

        [Fact]
        public void RemoveSubstringIgnoreCase() 
        {
            Assert.Equal("foo", "fooBarTar".RemoveIgnoreCase(new string[] { "bar", "tar" }));
        }

        [Fact]
        public void RemoveSubChars()
        {
            Assert.Equal("foo", "f1o$oX".Remove(new char[] { '1', '$', 'X' }));
        }
    }

    public class StringRepeatTest
    {
        [Fact]
        public void RepeatString() 
        {
            Assert.Equal("foofoofoo", "foo".Repeat(2));
        }
    }

    public class StringReverseTest
    {
        [Fact]
        public void ReverseString() 
        {
            Assert.Equal("12345", "54321".Reverse());
        }

        [Fact]
        public void ReverseStringUnicodeAware() 
        {
            // http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
            // Les Mise`rables -(reverse as byte array)-> selbar`esiM seL
            // Les Mise`rables -(reverse as unicode   )-> selba`resiM seL

            Assert.Equal("selbare\u0301siM seL", "Les Mise\u0301rables".Reverse(true));
            Assert.NotEqual("selbare\u0301siM seL", "Les Mise\u0301rables".Reverse(false));           
        }
    }

    public class StringStartEndWithTest
    {
        [Fact]
        public void StartWithIgnoreCase() 
        {
            Assert.True("foo".StartsWithIgnoreCase("F"));
        }

        [Fact]
        public void EndWithIgnoreCase() 
        {
            Assert.True("foo".EndsWithIgnoreCase("O"));
        }

        [Fact]
        public void EnsureStartWith() 
        {
            Assert.Equal("foobar", "bar".EnsureStartsWith("foo"));
            Assert.Equal("foobar", "foobar".EnsureStartsWith("foo"));
            Assert.Equal("Foofoobar", "foobar".EnsureStartsWith("Foo"));
            Assert.Equal("foobar", "foobar".EnsureStartsWithIgnoreCase("Foo"));
        }

        [Fact]
        public void EnsureEndWith() 
        {
            Assert.Equal("foobar", "foo".EnsureEndsWith("bar"));
            Assert.Equal("foobar", "foobar".EnsureEndsWith("bar"));
            Assert.Equal("foobarBar", "foobar".EnsureEndsWith("Bar"));
            Assert.Equal("foobar", "foobar".EnsureEndsWithIgnoreCase("Bar"));
        }
    }
}
