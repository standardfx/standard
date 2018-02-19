using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Standard.Data.StringMetrics;

namespace Standard.Data.StringMetrics.Tests
{
    public class UtilityClassFixture : IDisposable
    {
        public AffineGapRange1To0Multiplier1Over3 CostFunction1;
        public AffineGapRange5To0Multiplier1 CostFunction2;
        public SubCostRange0To1 CostFunction3;
        public SubCostRange1ToMinus2 CostFunction4;
        public SubCostRange5ToMinus3 CostFunction5;

        public UtilityClassFixture()
        {
            CostFunction1 = new AffineGapRange1To0Multiplier1Over3();
            CostFunction2 = new AffineGapRange5To0Multiplier1();
            CostFunction3 = new SubCostRange0To1();
            CostFunction4 = new SubCostRange1ToMinus2();
            CostFunction5 = new SubCostRange5ToMinus3();
        }

        public void Dispose()
        {
            // do nothing
        }
    }

    public class UtilityClassTests : IClassFixture<UtilityClassFixture>
    {
        UtilityClassFixture fixture;

        public UtilityClassTests(UtilityClassFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AffineGapRange1To0Multiplier1Over3PassTest()
        {
            double result = fixture.CostFunction1.GetCost("CHRIS", 1, 3);
            Assert.Equal("1.333", result.ToString("F3"));
        }

        [Fact]
        public void AffineGapRange1To0Multiplier1Over3FailTest()
        {
            double result = fixture.CostFunction1.GetCost("CHRIS", 4, 3);
            Assert.Equal("0.000", result.ToString("F3"));
        }

        [Fact]
        public void AffineGapRange5To0Multiplier1PassTest()
        {
            double result = fixture.CostFunction2.GetCost("CHRIS", 1, 3);
            Assert.Equal("6.000", result.ToString("F3"));
        }

        [Fact]
        public void AffineGapRange5To0Multiplier1FailTest()
        {
            double result = fixture.CostFunction2.GetCost("CHRIS", 4, 3);
            Assert.Equal("0.000", result.ToString("F3"));
        }

        [Fact]
        public void SubCostRange0To1PassTest()
        {
            double result = fixture.CostFunction3.GetCost("CHRIS", 1, "KRIS", 3);
            Assert.Equal("1.000", result.ToString("F3"));
        }

        [Fact]
        public void SubCostRange0To1FailTest()
        {
            double result = fixture.CostFunction3.GetCost("CHRIS", 4, "KRIS", 3);
            Assert.Equal("0.000", result.ToString("F3"));
        }

        [Fact]
        public void SubCostRange1ToMinus2PassTest()
        {
            double result = fixture.CostFunction4.GetCost("CHRIS", 1, "CHRIS", 1);
            Assert.Equal("1.000", result.ToString("F3"));
        }

        [Fact]
        public void SubCostRange1ToMinus2FailTest()
        {
             // fail due to first word index greater than word length
            Assert.Equal("-2.000", fixture.CostFunction4.GetCost("CHRIS", 6, "CHRIS", 3).ToString("F3"));

            // fail due to second word index greater than word length
            Assert.Equal("-2.000", fixture.CostFunction4.GetCost("CHRIS", 3, "CHRIS", 6).ToString("F3"));

            // fail to different chars
            Assert.Equal("-2.000", fixture.CostFunction4.GetCost("CHRIS", 1, "KRIS", 1).ToString("F3"));
        }

        [Fact]
        public void SubCostRange5ToMinus3PassTest()
        {
            double result = fixture.CostFunction5.GetCost("CHRIS", 1, "CHRIS", 1);
            Assert.Equal("5.000", result.ToString("F3"));
        }

        [Fact]
        public void SubCostRange5ToMinus3FailTest()
        {
             // fail due to first word index greater than word length
            Assert.Equal("-3.000", fixture.CostFunction5.GetCost("CHRIS", 6, "CHRIS", 3).ToString("F3"));

            // fail due to second word index greater than word length
            Assert.Equal("-3.000", fixture.CostFunction5.GetCost("CHRIS", 3, "CHRIS", 6).ToString("F3"));

            // fail to different chars
            Assert.Equal("-3.000", fixture.CostFunction5.GetCost("CHRIS", 1, "KRIS", 1).ToString("F3"));
        }

        [Fact]
        public void SubCostRange5ToMinus3ApproxTest()
        {
            double result = fixture.CostFunction5.GetCost("GILL", 0, "JILL", 0);
            Assert.Equal("3.000", result.ToString("F3"));
        }
    }    
}