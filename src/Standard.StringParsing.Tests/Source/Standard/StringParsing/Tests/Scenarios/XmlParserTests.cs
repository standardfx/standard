using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XmlParsing;

namespace Standard.StringParsing.Tests
{
    /*
    
    // doesn't work yet...#todo

    public class XmlParserTests
    {
        [Fact]
        public void CanParseBasicXml()
        {
            string input = @"
<body>
  <p>
    hello,<br/> <!--
      This is a comment
    --><i>world!</i>
  </p>
</body>
";

            string expected = @"<body><p>hello,<br/><i>world!</i></p></body>";

            var parsed = XmlParser.Eval(input);
            Assert.Equal(expected, parsed.ToString());
        }
    }
    */
}
