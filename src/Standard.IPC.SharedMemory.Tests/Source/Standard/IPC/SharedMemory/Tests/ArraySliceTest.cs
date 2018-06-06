using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
    public class ArraySliceTest
    {
		[Fact]
		public void ArraySliceWorksLikeArray()
		{
			var a = new[] {1.0, 2.71828, 3.14, 4, 4.99999, 42, 1024};
            var slicea = new ArraySlice<double>(a);
            var sliceaSame = new ArraySlice<double>(a);

            var b = new[] { 1.0, 2, 3, 4, 5, 99, 1024 };
            var sliceb = new ArraySlice<double>(b);

            Assert.Equal(a, slicea.List);
            Assert.Equal(0, slicea.Offset);
            Assert.Equal(7, slicea.Count);
            Assert.True(slicea.Equals(sliceaSame));
            Assert.True(slicea.Equals((object)sliceaSame));
            Assert.Equal(sliceaSame.GetHashCode(), sliceaSame.GetHashCode());
            Assert.True(slicea == sliceaSame);
            Assert.True(slicea != sliceb);
            Assert.True(ApproximatelyEqual(4, slicea[3]));
            Assert.Equal(6, slicea.IndexOf(1024));
            Assert.Equal(-1, slicea.IndexOf(1025));
            Assert.Contains(1024, slicea);
            Assert.DoesNotContain(1025, slicea);
            Assert.True(ApproximatelyEqual(1081.85827, slicea.Sum()));

            IList<double> asList = slicea;
            Assert.True(ApproximatelyEqual(4, asList[3]));
            Assert.Equal(6, asList.IndexOf(1024));
            Assert.Equal(-1, asList.IndexOf(1025));
            Assert.Contains(1024, asList);
            Assert.DoesNotContain(1025, asList);
            Assert.True(ApproximatelyEqual(1081.85827, asList.Sum()));
		}

		[Fact]
		public void ArraySliceCanSlice()
		{
            var a = new[] { 1.0, 2.71828, 3.14, 4, 4.99999, 42, 1024 };
            var slicea = new ArraySlice<double>(a, 2, 3);
            var sliceaSame = new ArraySlice<double>(a, 2, 3);

            var b = new[] { 1.0, 2, 3, 4, 5, 99, 1024 };
            var sliceb = new ArraySlice<double>(b, 2, 3);

            Assert.Equal(a, slicea.List);
            Assert.Equal(2, slicea.Offset);
            Assert.Equal(3, slicea.Count);
            Assert.True(slicea.Equals(sliceaSame));
            Assert.True(slicea.Equals((object)sliceaSame));
            Assert.Equal(sliceaSame.GetHashCode(), sliceaSame.GetHashCode());
            Assert.True(slicea == sliceaSame);
            Assert.True(slicea != sliceb);

            Assert.True(ApproximatelyEqual(4.99999, slicea[2]));
            Assert.Equal(1, slicea.IndexOf(4));
            Assert.Equal(-1, slicea.IndexOf(1025));
            Assert.Contains(4, slicea);
            Assert.DoesNotContain(1025, slicea);
            Assert.True(ApproximatelyEqual(12.13999, slicea.Sum()));

            IList<double> asList = slicea;

            Assert.True(ApproximatelyEqual(4.99999, asList[2]));
            Assert.Equal(1, asList.IndexOf(4));
            Assert.Equal(-1, asList.IndexOf(1025));
            Assert.True(asList.Contains(4));
            Assert.False(asList.Contains(1025));
            Assert.True(ApproximatelyEqual(12.13999, asList.Sum()));			
		}

        internal static bool ApproximatelyEqual(double x, double y)
        {
            var epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }
	}
}
