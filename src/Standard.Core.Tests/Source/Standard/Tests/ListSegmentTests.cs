using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Standard;

namespace Standard.Tests
{
    public class ListSegmentTests
    {
        [Fact]
        public void ListSegmentWorksLikeArray()
        {
            var a = new[] {1.0, 2.71828, 3.14, 4, 4.99999, 42, 1024};
            var slicea = new ListSegment<double>(a);
            var sliceaSame = new ListSegment<double>(a);

            var b = new[] { 1.0, 2, 3, 4, 5, 99, 1024 };
            var sliceb = new ListSegment<double>(b);

            Assert.Equal(a, slicea.List);
            Assert.Equal(0, slicea.Offset);
            Assert.Equal(7, slicea.Count);
            Assert.True(slicea.Equals(sliceaSame));
            Assert.True(slicea.Equals((object)sliceaSame));
            Assert.Equal(sliceaSame.GetHashCode(), sliceaSame.GetHashCode());
            Assert.True(slicea == sliceaSame);
            Assert.True(slicea != sliceb);

            Assert.True(slicea[3].NearEquals(4));
            Assert.Equal(6, slicea.IndexOf(1024));
            Assert.Equal(-1, slicea.IndexOf(1025));
            Assert.Contains(1024, slicea);
            Assert.DoesNotContain(1025, slicea);
            Assert.True(slicea.Sum().NearEquals(1081.85827));

            IList<double> asList = slicea;

            Assert.True(asList[3].NearEquals(4));
            Assert.Equal(6, asList.IndexOf(1024));
            Assert.Equal(-1, asList.IndexOf(1025));
            Assert.Contains(1024, asList);
            Assert.DoesNotContain(1025, asList);
            Assert.True(asList.Sum().NearEquals(1081.85827));
        }

        [Fact]
        public void TestSlice()
        {
            var a = new[] { 1.0, 2.71828, 3.14, 4, 4.99999, 42, 1024 };
            var slicea = new ListSegment<double>(a, 2, 3);
            var sliceaSame = new ListSegment<double>(a, 2, 3);

            var b = new[] { 1.0, 2, 3, 4, 5, 99, 1024 };
            var sliceb = new ListSegment<double>(b, 2, 3);

            Assert.Equal(a, slicea.List);
            Assert.Equal(2, slicea.Offset);
            Assert.Equal(3, slicea.Count);
            Assert.True(slicea.Equals(sliceaSame));
            Assert.True(slicea.Equals((object)sliceaSame));
            Assert.Equal(sliceaSame.GetHashCode(), sliceaSame.GetHashCode());
            Assert.True(slicea == sliceaSame);
            Assert.True(slicea != sliceb);

            Assert.True(slicea[2].NearEquals(4.99999));
            Assert.Equal(1, slicea.IndexOf(4));
            Assert.Equal(-1, slicea.IndexOf(1025));
            Assert.Contains(4, slicea);
            Assert.DoesNotContain(1025, slicea);
            Assert.True(slicea.Sum().NearEquals(12.13999));

            IList<double> asList = slicea;

            Assert.True(asList[2].NearEquals(4.99999));
            Assert.Equal(1, asList.IndexOf(4));
            Assert.Equal(-1, asList.IndexOf(1025));
            Assert.Contains(4, asList);
            Assert.DoesNotContain(1025, asList);
            Assert.True(asList.Sum().NearEquals(12.13999));
        }
    }    
}