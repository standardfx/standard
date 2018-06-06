using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringPatternTest
    {
        [Fact]
        public void RegexTest() 
        {
            string ipregex = "^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$";
            Assert.True("127.0.0.6".IsMatch(ipregex));
            Assert.False("1427.0.0.b".IsMatch(ipregex));
        }

        [Fact]
        public void WildcardTest() 
        {
            Assert.False("foo12r.txt".IsLike("Foo*.txt"));
            Assert.True("foo12r.txt".IsLike("foo*.txt"));
            Assert.False("foo12r.txt".IsLike("Foo*.txt", false));

            Assert.True("foo12r.txt".IsLike("foo##[rx].txt"));
            Assert.True("foo12x.txt".IsLike("foo##[rx].txt"));
            Assert.False("foo12z.txt".IsLike("foo##[rx].txt"));
        }
    }
}
