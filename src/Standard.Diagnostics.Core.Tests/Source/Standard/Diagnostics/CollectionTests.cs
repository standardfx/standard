using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class CollectionsTests
    {
        [Fact]
        public void EmptyArrayTest()
        {
			int[] a = new int[] { 1, 2, 3 };
			int[] b = new int[] { };

			SAssert.NotEmpty(a);
			XAssert.Throws<ArgumentException>(() => SAssert.NotEmpty(b));

			SAssert.Empty(b);
			XAssert.Throws<ArgumentException>(() => SAssert.Empty(a));
		}

		[Fact]
		public void EmptyDictionaryTest()
		{
			Dictionary<string, string> a = new Dictionary<string, string>();
			a.Add("foo", "bar");
			Dictionary<string, string> b = new Dictionary<string, string>();

			SAssert.NotEmpty(a);
			XAssert.Throws<ArgumentException>(() => SAssert.NotEmpty(b));

			SAssert.Empty(b);
			XAssert.Throws<ArgumentException>(() => SAssert.Empty(a));
		}

		[Fact]
		public void EmptyListTest()
		{
			List<string> a = new List<string>();
			a.Add("foo");
			List<string> b = new List<string>();

			SAssert.NotEmpty(a);
			XAssert.Throws<ArgumentException>(() => SAssert.NotEmpty(b));

			SAssert.Empty(b);
			XAssert.Throws<ArgumentException>(() => SAssert.Empty(a));
		}

		[Fact]
		public void AllPredicateTest()
		{
			int[] a = new int[] { 1, 2, 3 };
			SAssert.All(a, x => x < 5);
			XAssert.Throws<ArgumentException>(() => SAssert.All(a, x => x < 2));
		}

		[Fact]
		public void CountTest()
		{
			int[] a = new int[] { 1, 2, 3 };
			SAssert.Count(a, 2, x => x < 3);
			XAssert.Throws<ArgumentException>(() => SAssert.Count(a, 3, x => x < 3));
		}

		[Fact]
		public void CountMinTest()
		{
			int[] a = new int[] { 1, 2, 3, 5, 7 };
			SAssert.MinCount(a, 2, x => x > 4);
			XAssert.Throws<ArgumentException>(() => SAssert.MinCount(a, 3, x => x < 3));
		}

		[Fact]
		public void CountMaxTest()
		{
			int[] a = new int[] { 1, 2, 3, 5, 7 };
			SAssert.MaxCount(a, 3, x => x < 3);
			XAssert.Throws<ArgumentException>(() => SAssert.MaxCount(a, 2, x => x > 2));
		}

		[Fact]
		public void CountRangeTest()
		{
			int[] a = new int[] { 1, 2, 3, 5, 7, 8, 10 };
			SAssert.Count(a, 2, 4, x => x > 3);
			XAssert.Throws<ArgumentException>(() => SAssert.Count(a, 3, 5, x => x > 7));
			XAssert.Throws<ArgumentException>(() => SAssert.Count(a, 3, 5, x => x > 1));
		}

		[Fact]
		public void DictionaryTest()
		{
			Dictionary<string, string> a = new Dictionary<string, string>();
			a.Add("foo", "bar");
			SAssert.ContainsKey(a, "foo");
			XAssert.Throws<ArgumentException>(() => SAssert.ContainsKey(a, "loo"));
		}
	}
}