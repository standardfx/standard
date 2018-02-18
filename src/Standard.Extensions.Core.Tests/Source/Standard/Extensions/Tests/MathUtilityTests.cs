using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Standard;

namespace Standard.Data.StringMetrics.Tests
{
    public class MathUtilTests
    {
        [Fact]
        public void ConvertBaseXToDecimal()
        {
            Assert.Equal(296, MathUtility.BaseToDecimal("128", 16));
        }

        [Fact]
        public void ConvertDecimalToBaseX()
        {
            Assert.Equal("3F", MathUtility.DecimalToBase(63, 16));
        }

        [Fact]
        public void MathFuncsMinOf3Integer()
        {
            Assert.Equal(1, MathUtility.Min(1, 2, 3));
        }

        [Fact]
        public void MathFuncsMinOf3Double()
        {
            Assert.Equal(1.0, MathUtility.Min(1.0, 2.0, 3.0));
        }

        [Fact]
        public void MathFuncsMaxOf3Integer()
        {
            Assert.Equal(3, MathUtility.Max(1, 2, 3));
        }

        [Fact]
        public void MathFuncsMaxOf3Double()
        {
            Assert.Equal(3.0, MathUtility.Max(1.0, 2.0, 3.0));
        }

        [Fact]
        public void MathFuncsMaxOf4Integer()
        {
            Assert.Equal(4, MathUtility.Max(1, 2, 3, 4));
        }

        [Fact]
        public void MathFuncsMaxOf4Double()
        {
            Assert.Equal(4.0, MathUtility.Max(1.0, 2.0, 3.0, 4.0));
        }
    }    
}