using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Standard.StringParsing.Tests
{
    public class StartDateParserTests
    {
        [Fact]
        public void ItIsPossibleToParseAStarDate()
        {
            Assert.Equal(new DateTime(2259, 2, 24), StarDateParser.Eval("2259.55"));
        }

        [Fact]
        public void InvalidStarDatesAreNotParsed()
        {
            Assert.Throws<ParseException>(() => { 
                var date = StarDateParser.Eval("2259.4000"); 
            });
        }
    }
}
