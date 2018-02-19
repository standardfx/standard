using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using TRS = Standard.Data.Parsing.RS;

namespace Standard.Data.Parsing.Tests
{
    public class ResultTests
    {
        private readonly ITestOutputHelper output;

        public ResultTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void FailureContainingBracketFormattedSuccessfully()
        {
            var p = Parse.String("xy").Text().XMany().End();
            var r = (Result<IEnumerable<string>>)p.TryParse("x{");
            Assert.Contains(string.Format(TRS.UnexpectedToken, "{"), r.Message);
        }

        [Fact]
        public void FailureShowsNearbyParseResults()
        {
            var p = 
                from a in Parse.Char('x')
                from b in Parse.Char('y')
                select string.Format("{0},{1}", a, b);

            var r = (Result<string>)p.TryParse("x{");

            string expectedMessage = string.Format(TRS.ParseFailureInfo, 
                string.Format(TRS.UnexpectedToken, "{"),
                TRS.Expected + " " + "y",
                "Line 1, Column 2",
                "x");
            //output.WriteLine(r.ToString());
            Assert.Equal(expectedMessage, r.ToString());
        }
    }
}
