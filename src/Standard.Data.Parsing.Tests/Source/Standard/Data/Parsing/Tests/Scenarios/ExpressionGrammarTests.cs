using System;
using System.Linq.Expressions;
using Xunit;
using Standard.Data.Parsing;

namespace Standard.Data.Parsing.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void DroppedClosingParenthesisProducesMeaningfulError()
        {
            const string input = "1 + (2 * 3";
            var x = Assert.Throws<ParseException>(() => ExpressionParser.Eval(input));
            Assert.Contains("expected )", x.Message);
        }

        [Fact]
        public void MissingOperandProducesMeaningfulError()
        {
            const string input = "1 + * 3";
            var x = Assert.Throws<ParseException>(() => ExpressionParser.Eval(input));
            Assert.DoesNotContain("expected end of input", x.Message);
        }    
    }
}
