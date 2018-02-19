using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Standard.Data.Parsing;

namespace Standard.Data.Parsing.Tests
{
    public class StarDateParser
    {
        private static readonly Parser<DateTime> StarTrek2009StarDate =
            from year in Parse.Digit.Many().Text()
            from delimiter in Parse.Char('.')
            from dayOfYear in Parse.Digit.Repeat(1, 3).Text().End()
            select new DateTime(int.Parse(year), 1, 1).AddDays(int.Parse(dayOfYear) - 1);

        public static DateTime Eval(string input)
        {
            return StarTrek2009StarDate.Parse(input);
        }
    }
}
