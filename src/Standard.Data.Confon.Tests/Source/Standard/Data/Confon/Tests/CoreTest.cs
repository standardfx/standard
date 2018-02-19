using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard.Data.Confon;

namespace Standard.Data.Confon.Tests
{
    public class ConfonTests
    {

/*
#todo
        [Fact]
        public void CanSubstituteQuotedKeysWithPeriodsInside()
        {
            var confon = @"
a {
   'dot.key': {
      frog = green
   }
}
b = ${a.'dot.key'}
";
            var config = ConfonFactory.ParseString(confon);

            var config2 = config.GetContext("b.'dot.key'");
            var enumerable2 = config2.AsEnumerable();
            Assert.Equal("frog",
                enumerable2.Select(kvp => kvp.Key).First());
        }        
*/

        // undefined behavior in spec
        [Fact]
        public void CanUnwrapSub() 
        {
            var confon = @"
a {
   b {
     c = 1
     d = true
   }
}";
            var config = ConfonFactory.ParseString(confon).Root.GetObject().Unwrapped;
            var a = config["a"] as IDictionary<string, object>;
            var b = a["b"] as IDictionary<string, object>;
            Assert.Equal(1, (b["c"] as ConfonValue).GetInt32());
            Assert.True((b["d"] as ConfonValue).GetBoolean());
        }

        //undefined behavior in spec
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedObject() 
        {
            var confon = " root { string : \"hello\" ";
            Assert.Throws<ConfonParserException>(() => 
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedNestedObject() 
        {
            var confon = " root { bar { string : \"hello\" } ";
            Assert.Throws<ConfonParserException>(() =>
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedString() 
        {
            var confon = " string : \"hello";
            Assert.Throws<ConfonParserException>(() => 
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedStringInObject()
        {
            var confon = " root { string : \"hello }";
            Assert.Throws<ConfonParserException>(() => 
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedArray() 
        {
            var confon = " array : [1,2,3";
            Assert.Throws<ConfonParserException>(() => 
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void ThrowsParserExceptionOnUnterminatedArrayInObject() 
        {
            var confon = " root { array : [1,2,3 }";
            Assert.Throws<ConfonParserException>(() => 
                ConfonFactory.ParseString(confon));
        }

        //undefined behavior in spec        
        [Fact]
        public void GettingStringFromArrayReturnsNull()
        {
            var confon = " array : [1,2,3]";
            Assert.Null(ConfonFactory.ParseString(confon).GetString("array"));
        }


        //TODO: not sure if this is the expected behavior but it is what we have established
        //undefined behavior in spec        
        [Fact]
        public void GettingArrayFromLiteralsReturnsNull() 
        {
            var confon = " literal : a b c";
            var res = ConfonFactory.ParseString(confon).GetStringList("literal");

            Assert.Empty(res);
        }

        //Added tests to conform to spec
        [Fact]
        public void CanUsePathsAsKeys_3_14()
        {
            var confon1 = @"3.14 : 42";
            var confon2 = @"3 { 14 : 42}";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("3.14"),
                ConfonFactory.ParseString(confon2).GetString("3.14"));
        }

        [Fact]
        public void CanUsePathsAsKeys_3()
        {
            var confon1 = @"3 : 42";
            var confon2 = @"""3"" : 42";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("3"),
                ConfonFactory.ParseString(confon2).GetString("3"));
        }

        [Fact]
        public void CanUsePathsAsKeys_true()
        {
            var confon1 = @"true : 42";
            var confon2 = @"""true"" : 42";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("true"),
                ConfonFactory.ParseString(confon2).GetString("true"));
        }

        [Fact]
        public void CanUsePathsAsKeys_FooBar()
        {
            var confon1 = @"foo.bar : 42";
            var confon2 = @"foo { bar : 42 }";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("foo.bar"),
                ConfonFactory.ParseString(confon2).GetString("foo.bar"));
        }

        [Fact]
        public void CanUsePathsAsKeys_FooBarBaz()
        {
            var confon1 = @"foo.bar.baz : 42";
            var confon2 = @"foo { bar { baz : 42 } }";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("foo.bar.baz"),
                ConfonFactory.ParseString(confon2).GetString("foo.bar.baz"));
        }

        [Fact]
        public void CanUsePathsAsKeys_AX_AY()
        {
            var confon1 = @"a.x : 42, a.y : 43";
            var confon2 = @"a { x : 42, y : 43 }";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("a.x"),
                ConfonFactory.ParseString(confon2).GetString("a.x"));
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("a.y"),
                ConfonFactory.ParseString(confon2).GetString("a.y"));
        }

        [Fact]
        public void CanUsePathsAsKeys_A_B_C()
        {
            var confon1 = @"a b c : 42";
            var confon2 = @"""a b c"" : 42";
            Assert.Equal(
                ConfonFactory.ParseString(confon1).GetString("a b c"),
                ConfonFactory.ParseString(confon2).GetString("a b c"));
        }


        [Fact]
        public void CanConcatenateSubstitutedUnquotedString()
        {
            var confon = @"a {
  name = Roger
  c = Hello my name is ${a.name}
}";
            Assert.Equal(
                "Hello my name is Roger", 
                ConfonFactory.ParseString(confon).GetString("a.c"));
        }

        [Fact]
        public void CanConcatenateSubstitutedArray()
        {
            var confon = @"a {
  b = [1,2,3]
  c = ${a.b} [4,5,6]
}";
            Assert.True(new[] {1, 2, 3, 4, 5, 6}.SequenceEqual(ConfonFactory.ParseString(confon).GetInt32List("a.c")));
        }

        [Fact]
        public void CanParseSubConfig()
        {
            var confon = @"
a {
   b {
     c = 1
     d = true
   }
}";
            var config = ConfonFactory.ParseString(confon);
            var subConfig = config.GetContext("a");
            Assert.Equal(1, subConfig.GetInt32("b.c"));
            Assert.True(subConfig.GetBoolean("b.d"));
        }


        [Fact]
        public void CanParseConfon()
        {
            var confon = @"
root {
  int = 1
  quoted-string = ""foo""
  unquoted-string = bar
  concat-string = foo bar
  object {
    hasContent = true
  }
  array = [1,2,3,4]
  array-concat = [[1,2] [3,4]]
  array-single-element = [1 2 3 4]
  array-newline-element = [
    1
    2
    3
    4
  ]
  null = null
  double = 1.23
  bool = true
}
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal("1", config.GetString("root.int"));
            Assert.Equal("1.23", config.GetString("root.double"));
            Assert.True(config.GetBoolean("root.bool"));
            Assert.True(config.GetBoolean("root.object.hasContent"));
            Assert.Null(config.GetString("root.null"));
            Assert.Equal("foo", config.GetString("root.quoted-string"));
            Assert.Equal("bar", config.GetString("root.unquoted-string"));
            Assert.Equal("foo bar", config.GetString("root.concat-string"));
            Assert.True(
                new[] {1, 2, 3, 4}.SequenceEqual(ConfonFactory.ParseString(confon).GetInt32List("root.array")));
            Assert.True(
                new[] {1, 2, 3, 4}.SequenceEqual(
                    ConfonFactory.ParseString(confon).GetInt32List("root.array-newline-element")));
            Assert.True(
                new[] {"1 2 3 4"}.SequenceEqual(
                    ConfonFactory.ParseString(confon).GetStringList("root.array-single-element")));
        }

        [Fact]
        public void CanParseJson()
        {
            var confon = @"
""root"" : {
  ""int"" : 1,
  ""string"" : ""foo"",
  ""object"" : {
        ""hasContent"" : true
    },
  ""array"" : [1,2,3],
  ""null"" : null,
  ""double"" : 1.23,
  ""bool"" : true
}
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal("1", config.GetString("root.int"));
            Assert.Equal("1.23", config.GetString("root.double"));
            Assert.True(config.GetBoolean("root.bool"));
            Assert.True(config.GetBoolean("root.object.hasContent"));
            Assert.Null(config.GetString("root.null"));
            Assert.Equal("foo", config.GetString("root.string"));
            Assert.True(new[] {1, 2, 3}.SequenceEqual(ConfonFactory.ParseString(confon).GetInt32List("root.array")));
        }

        [Fact]
        public void CanMergeObject()
        {
            var confon = @"
a.b.c = {
        x = 1
        y = 2
    }
a.b.c = {
        z = 3
    }
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal("1", config.GetString("a.b.c.x"));
            Assert.Equal("2", config.GetString("a.b.c.y"));
            Assert.Equal("3", config.GetString("a.b.c.z"));
        }

        [Fact]
        public void CanOverrideObject()
        {
            var confon = @"
a.b = 1
a = null
a.c = 3
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Null(config.GetString("a.b"));
            Assert.Equal("3", config.GetString("a.c"));
        }

        [Fact]
        public void CanParseObject()
        {
            var confon = @"
a {
  b = 1
}
";
            Assert.Equal("1", ConfonFactory.ParseString(confon).GetString("a.b"));
        }

        [Fact]
        public void CanTrimValue()
        {
            var confon = "a= \t \t 1 \t \t,";
            Assert.Equal("1", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanTrimConcatenatedValue()
        {
            var confon = "a= \t \t 1 2 3 \t \t,";
            Assert.Equal("1 2 3", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanConsumeCommaAfterValue()
        {
            var confon = "a=1,";
            Assert.Equal("1", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignIpAddressToField()
        {
            var confon = @"a=127.0.0.1";
            Assert.Equal("127.0.0.1", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignConcatenatedValueToField()
        {
            var confon = @"a=1 2 3";
            Assert.Equal("1 2 3", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignValueToQuotedField()
        {
            var confon = @"""a""=1";
            Assert.Equal(1L, ConfonFactory.ParseString(confon).GetInt64("a"));
        }

        [Fact]
        public void CanAssignValueToPathExpression()
        {
            var confon = @"a.b.c=1";
            Assert.Equal(1L, ConfonFactory.ParseString(confon).GetInt64("a.b.c"));
        }

        [Fact]
        public void CanAssignValuesToPathExpressions()
        {
            var confon = @"
a.b.c=1
a.b.d=2
a.b.e.f=3
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal(1L, config.GetInt64("a.b.c"));
            Assert.Equal(2L, config.GetInt64("a.b.d"));
            Assert.Equal(3L, config.GetInt64("a.b.e.f"));
        }

        [Fact]
        public void CanAssignLongToField()
        {
            var confon = @"a=1";
            Assert.Equal(1L, ConfonFactory.ParseString(confon).GetInt64("a"));
        }

        [Fact]
        public void CanAssignArrayToField()
        {
            var confon = @"a=
[
    1
    2
    3
]";
            Assert.True(new[] {1, 2, 3}.SequenceEqual(ConfonFactory.ParseString(confon).GetInt32List("a")));

            //confon = @"a= [ 1, 2, 3 ]";
            //Assert.True(new[] { 1, 2, 3 }.SequenceEqual(ConfonFactory.ParseString(confon).GetIntList("a")));
        }

        [Fact]
        public void CanConcatenateArray()
        {
            var confon = @"a=[1,2] [3,4]";
            Assert.True(new[] {1, 2, 3, 4}.SequenceEqual(ConfonFactory.ParseString(confon).GetInt32List("a")));
        }

        [Fact]
        public void CanAssignSubstitutionToField()
        {
            var confon = @"a{
    b = 1
    c = ${a.b}
    d = ${a.c}23
}";
            Assert.Equal(1, ConfonFactory.ParseString(confon).GetInt32("a.c"));
            Assert.Equal(123, ConfonFactory.ParseString(confon).GetInt32("a.d"));
        }

        [Fact]
        public void CanAssignDoubleToField()
        {
            var confon = @"a=1.1";
            Assert.Equal(1.1, ConfonFactory.ParseString(confon).GetDouble("a"));
        }

        [Fact]
        public void CanAssignNullToField()
        {
            var confon = @"a=null";
            Assert.Null(ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignBooleanToField()
        {
            var confon = @"a=true";
            Assert.True(ConfonFactory.ParseString(confon).GetBoolean("a"));
            confon = @"a=false";
            Assert.False(ConfonFactory.ParseString(confon).GetBoolean("a"));

            confon = @"a=on";
            Assert.True(ConfonFactory.ParseString(confon).GetBoolean("a"));
            confon = @"a=off";
            Assert.False(ConfonFactory.ParseString(confon).GetBoolean("a"));
        }

        [Fact]
        public void CanAssignQuotedStringToField()
        {
            var confon = @"a=""hello""";
            Assert.Equal("hello", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignTrippleQuotedStringToField()
        {
            var confon = @"a=""""""hello""""""";
            Assert.Equal("hello", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignUnQuotedStringToField()
        {
            var confon = @"a=hello";
            Assert.Equal("hello", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanAssignTripleQuotedStringToField()
        {
            var confon = @"a=""""""hello""""""";
            Assert.Equal("hello", ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanUseFallback()
        {
            var confon1 = @"
foo {
   bar {
      a=123
   }
}";
            var confon2 = @"
foo {
   bar {
      a=1
      b=2
      c=3
   }
}";

            var config1 = ConfonFactory.ParseString(confon1);
            var config2 = ConfonFactory.ParseString(confon2);

            var config = config1.WithFallback(config2);

            Assert.Equal(123, config.GetInt32("foo.bar.a"));
            Assert.Equal(2, config.GetInt32("foo.bar.b"));
            Assert.Equal(3, config.GetInt32("foo.bar.c"));
        }

        [Fact]
        public void CanUseFallbackInSubConfig()
        {
            var confon1 = @"
foo {
   bar {
      a=123
   }
}";
            var confon2 = @"
foo {
   bar {
      a=1
      b=2
      c=3
   }
}";

            var config1 = ConfonFactory.ParseString(confon1);
            var config2 = ConfonFactory.ParseString(confon2);

            var config = config1.WithFallback(config2).GetContext("foo.bar");

            Assert.Equal(123, config.GetInt32("a"));
            Assert.Equal(2, config.GetInt32("b"));
            Assert.Equal(3, config.GetInt32("c"));
        }

        [Fact]
        public void CanUseMultiLevelFallback()
        {
            var confon1 = @"
foo {
   bar {
      a=123
   }
}";
            var confon2 = @"
foo {
   bar {
      a=1
      b=2
      c=3
   }
}";
            var confon3 = @"
foo {
   bar {
      a=99
      zork=555
   }
}";
            var confon4 = @"
foo {
   bar {
      borkbork=-1
   }
}";

            var config1 = ConfonFactory.ParseString(confon1);
            var config2 = ConfonFactory.ParseString(confon2);
            var config3 = ConfonFactory.ParseString(confon3);
            var config4 = ConfonFactory.ParseString(confon4);

            var config = config1.WithFallback(config2.WithFallback(config3.WithFallback(config4)));

            Assert.Equal(123, config.GetInt32("foo.bar.a"));
            Assert.Equal(2, config.GetInt32("foo.bar.b"));
            Assert.Equal(3, config.GetInt32("foo.bar.c"));
            Assert.Equal(555, config.GetInt32("foo.bar.zork"));
            Assert.Equal(-1, config.GetInt32("foo.bar.borkbork"));
        }

        [Fact]
        public void CanUseFluentMultiLevelFallback()
        {
            var confon1 = @"
foo {
   bar {
      a=123
   }
}";
            var confon2 = @"
foo {
   bar {
      a=1
      b=2
      c=3
   }
}";
            var confon3 = @"
foo {
   bar {
      a=99
      zork=555
   }
}";
            var confon4 = @"
foo {
   bar {
      borkbork=-1
   }
}";

            var config1 = ConfonFactory.ParseString(confon1);
            var config2 = ConfonFactory.ParseString(confon2);
            var config3 = ConfonFactory.ParseString(confon3);
            var config4 = ConfonFactory.ParseString(confon4);

            var config = config1.WithFallback(config2).WithFallback(config3).WithFallback(config4);

            Assert.Equal(123, config.GetInt32("foo.bar.a"));
            Assert.Equal(2, config.GetInt32("foo.bar.b"));
            Assert.Equal(3, config.GetInt32("foo.bar.c"));
            Assert.Equal(555, config.GetInt32("foo.bar.zork"));
            Assert.Equal(-1, config.GetInt32("foo.bar.borkbork"));
        }

        [Fact]
        public void CanParseQuotedKeys()
        {
            var confon = @"
a {
   ""some quoted, key"": 123
}
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal(123, config.GetInt32("a.some quoted, key"));
        }

        [Fact]
        public void CanParseQuotedKeysWithPeriodsInside()
        {
            var confon = @"
a {
   ""some quoted key. with periods."": 123
}
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal(123, config.GetInt32(@"a.""some quoted key. with periods."""));
        }

        [Fact]
        public void CanEnumerateQuotedKeys()
        {
            var confon = @"
a {
   ""some quoted, key"": 123
}
";
            var config = ConfonFactory.ParseString(confon);
            var config2 = config.GetContext("a");
            var enumerable = config2.AsEnumerable();

            Assert.Equal("some quoted, key",
                enumerable.Select(kvp => kvp.Key).First());
        }

        [Fact]
        public void CanEnumerateQuotedKeysWithPeriodsInside()
        {
            var confon = @"
a {
   ""some quoted key. with periods."": 123
}
";
            var config = ConfonFactory.ParseString(confon);
            var config2 = config.GetContext("a");
            var enumerable = config2.AsEnumerable();

            Assert.Equal("some quoted key. with periods.",
                enumerable.Select(kvp => kvp.Key).First());
        }

        [Fact]
        public void CanEnumerateQuotedKeysOfObjectWithPeriodsInside()
        {
            var confon = @"
a {
   ""some.quoted.key"": {
      foo = bar
   }
   'single.quoted.key': {
      frog = green
   }
}
";
            var config = ConfonFactory.ParseString(confon);

            var config2 = config.GetContext("a.'some.quoted.key'");
            var enumerable2 = config2.AsEnumerable();
            Assert.Equal("foo",
                enumerable2.Select(kvp => kvp.Key).First());

            var config3 = config.GetContext("a.\"single.quoted.key\"");
            var enumerable3 = config3.AsEnumerable();
            Assert.Equal("frog",
                enumerable3.Select(kvp => kvp.Key).First());
        }        

        [Fact]
        public void CanTurnObjectToEnumerable()
        {
            var confon = @"
foo.bar {
    child {
      hair = ""copper brown""
      eye-color = ""electric blue""
      gender = ""it is a girl!""
    }

    child-obj2 {
      # Here comes the comments
      # blah blah blah...
      # more blah blah
      ""toe.can.bend"" = yeah
      ""very, very.naughty"" = dododo
    }

}";

            var config = ConfonFactory.ParseString(confon);

            var child1 = config.GetContext("foo.bar.child").AsEnumerable().ToList();
            var child2 = config.GetContext("foo.bar.child-obj2").AsEnumerable().ToList();

            Assert.Equal("copper brown",
                child1.Select(kvp => kvp.Value).First().GetString());

            Assert.Equal("very, very.naughty",
                child2.Select(kvp => kvp.Key).Last());
        }

        [Fact]
        public void CanOverwriteValue()
        {
            var confon = @"
test {
  value  = 123
}
test.value = 456
";
            var config = ConfonFactory.ParseString(confon);
            Assert.Equal(456, config.GetInt32("test.value"));
        }

        [Fact]
        public void CanCSubstituteObject()
        {
            var confon = @"a {
  b {
      foo = hello
      bar = 123
  }
  c {
     d = xyz
     e = ${a.b}
  }  
}";
            var ace = ConfonFactory.ParseString(confon).GetContext("a.c.e");
            Assert.Equal("hello", ace.GetString("foo"));
            Assert.Equal(123, ace.GetInt32("bar"));
        }

        [Fact]
        public void CanAssignNullStringToField()
        {
            var confon = @"a=null";
            Assert.Null(ConfonFactory.ParseString(confon).GetString("a"));
        }

        // [Ignore("we currently do not make any destinction between quoted and unquoted strings once parsed")]
        [Fact]
        public void CanAssignQuotedNullStringToField()
        {
            var confon = @"a=""null""";
            Assert.Null(ConfonFactory.ParseString(confon).GetString("a"));
        }

        [Fact]
        public void CanParseInclude()
        {
            var confon = @"a {
  b { 
       include ""foo""
  }";
            var inclConfon = @"
x = 123
y = hello
";
            Func<string, ConfonRoot> include = s => ConfonParser.Parse(inclConfon, null);
            var config = ConfonFactory.ParseString(confon, include);

            Assert.Equal(123, config.GetInt32("a.b.x"));
            Assert.Equal("hello", config.GetString("a.b.y"));
        }

        [Fact]
        public void CanResolveSubstitutesInInclude()
        {
            var confon = @"a {
  b { 
       include ""foo""
  }";
            var inclConfon = @"
x = 123
y = ${x}
";
            Func<string, ConfonRoot> include = s => ConfonParser.Parse(inclConfon, null);
            var config = ConfonFactory.ParseString(confon, include);

            Assert.Equal(123, config.GetInt32("a.b.x"));
            Assert.Equal(123, config.GetInt32("a.b.y"));
        }

        [Fact]
        public void CanResolveSubstitutesInNestedIncludes()
        {
            var confon = @"a.b.c {
  d { 
       include ""foo""
  }";
            var inclConfon = @"
f = 123
e {
      include ""foo""
}
";

            var inclConfon2 = @"
x = 123
y = ${x}
";

            Func<string, ConfonRoot> include2 = s => ConfonParser.Parse(inclConfon2, null);
            Func<string, ConfonRoot> include = s => ConfonParser.Parse(inclConfon, include2);
            var config = ConfonFactory.ParseString(confon, include);

            Assert.Equal(123, config.GetInt32("a.b.c.d.e.x"));
            Assert.Equal(123, config.GetInt32("a.b.c.d.e.y"));
        }
    }
}

