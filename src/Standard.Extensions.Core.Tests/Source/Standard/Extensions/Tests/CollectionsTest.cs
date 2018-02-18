using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class CollectionsTest
    {
		private List<int> a = new List<int>() { 1, 2, 3, 4 };
		private List<int> b = new List<int>() { 2, 1, 3 };
		private List<int> c = new List<int>() { 4, 2, 1, 3 };
		private List<int> d = new List<int>() { 4, 2, 5, 1, 3 };
		private List<int> e = new List<int>() { 4, 6, 5 };

		[Fact]
        public void Overlap() 
        {
			Assert.True(a.Overlaps(b));
			Assert.True(c.Overlaps(a));
			Assert.True(a.Overlaps(e));
			Assert.False(b.Overlaps(e));
        }

        [Fact]
        public void Subset() 
        {
			Assert.True(b.IsSubsetOf(a));
			Assert.True(c.IsSubsetOf(a));
			Assert.False(d.IsSubsetOf(a));
		}

		[Fact]
        public void ProperSubset() 
        {
			Assert.True(b.IsProperSubsetOf(a));
			Assert.False(c.IsProperSubsetOf(a));
			Assert.False(d.IsProperSubsetOf(a));
		}

		[Fact]
		public void Superset()
		{
			Assert.True(c.IsSupersetOf(a));
			Assert.True(c.IsSupersetOf(b));
			Assert.False(c.IsSupersetOf(d));
		}

		[Fact]
		public void ProperSuperset()
		{
			Assert.False(c.IsProperSupersetOf(a));
			Assert.True(c.IsProperSupersetOf(b));
			Assert.False(c.IsProperSupersetOf(d));
		}
	}
}

