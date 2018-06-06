using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringNewLineTest
    {
        [Fact]
        public void TurnsCrToCrLf() 
        {
            string actual = "Line\ris broken.".NormalizeNewLine();
            string expected = string.Format("Line{0}is broken.", Environment.NewLine);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TurnsLfToCrLf() 
        {
            string actual = "Line\nis broken.".NormalizeNewLine();
            string expected = string.Format("Line{0}is broken.", Environment.NewLine);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TurnsIntermixedCrAndLfToCrLf() 
        {
            string actual = "Line\nis\rbroken.".NormalizeNewLine();
            string expected = string.Format("Line{0}is{0}broken.", Environment.NewLine);

            Assert.Equal(expected, actual);
        }
    }
}

