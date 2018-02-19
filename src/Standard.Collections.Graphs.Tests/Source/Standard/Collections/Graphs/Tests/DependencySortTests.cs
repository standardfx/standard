using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Standard.Collections.Graphs;

namespace Standard.Collections.Graphs.Tests
{
    public class DependencySortTests
    {
        [Fact]
        public void CanSortDependency()
        {
			var a = new DependencyItem<string>("A");
			var b = new DependencyItem<string>("B", "C", "E");
			var c = new DependencyItem<string>("C");
			var d = new DependencyItem<string>("D", "A");
			var e = new DependencyItem<string>("E", "D", "G");
			var f = new DependencyItem<string>("F");
			var g = new DependencyItem<string>("G", "F", "H");
			var h = new DependencyItem<string>("H");

			var unsorted = new[] { a, b, c, d, e, f, g, h };
			var expected = new[] { a, c, d, f, h, g, e, b };

			var actual = SortUtility.TopoSort(unsorted, x => x.Name, y => y.Dependencies).ToArray();
			var actual2 = SortUtility.TopoSort(unsorted).ToArray();

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.Equal(expected[i].Name, actual[i].Name);
				Assert.Equal(expected[i].Name, actual2[i].Name);
			}
		}

        [Fact]
        public void ThrowsOnCyclicDependency()
        {
			var unsorted = new List<List<string>>()
			{
				new List<string>() { "A" },
				new List<string>() { "B", "C", "E" },
				new List<string>() { "C" },
				new List<string>() { "D", "A" },
				new List<string>() { "E", "D", "G" },
				new List<string>() { "F", "E" },
				new List<string>() { "G", "F", "H" },
				new List<string>() { "H" },
			};

			Assert.Throws<ArgumentException>(() => SortUtility.TopoSort(unsorted).ToArray());
		}

		[Fact]
		public void ThrowsOnMissingDependency()
		{
			var unsorted = new List<List<string>>()
			{
				new List<string>() { "A" },
				new List<string>() { "B", "C", "E" },
				new List<string>() { "C" },
				new List<string>() { "D", "A" },
				new List<string>() { "E", "D", "G" },
				new List<string>() { "F", "Z" }, // let's miss Z
				new List<string>() { "G", "F", "H" },
				new List<string>() { "H" },
			};

			Assert.Throws<ArgumentException>(() => SortUtility.TopoSort(unsorted).ToArray());
		}
	}
}
