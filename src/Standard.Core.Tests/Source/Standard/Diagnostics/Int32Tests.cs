using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class Int32Tests
    {
        [Fact]
        public void EqualityTest()
        {
			int actual = 32;
			int compared = 32;
			SAssert.Equals(actual, compared);

			int compared2 = 23;
			XAssert.Throws<ArgumentException>(() => SAssert.Equals(actual, compared2));
        }

		[Fact]
		public void NonEqualityTest()
		{
			int actual = 32;
			int compared = 23;
			SAssert.NotEquals(actual, compared);
			
			int compared2 = 32;
			XAssert.Throws<ArgumentException>(() => SAssert.NotEquals(actual, compared2));
		}

		[Fact]
		public void GreaterThanTest()
		{
			int actual = 32;
			int compared = 23;
			SAssert.GreaterThan(actual, compared);

			int compared2 = 64;
			XAssert.Throws<ArgumentException>(() => SAssert.GreaterThan(actual, compared2));
		}

		[Fact]
		public void GreaterThanOrEqualsTest()
		{
			int actual = 32;
			int compared = 23;
			int compared2 = 32;
			SAssert.GreaterThanOrEqualsTo(actual, compared);
			SAssert.GreaterThanOrEqualsTo(actual, compared2);

			int compared3 = 64;
			XAssert.Throws<ArgumentException>(() => SAssert.GreaterThan(actual, compared3));
		}

		[Fact]
		public void LessThanTest()
		{
			int actual = 23;
			int compared = 32;
			SAssert.LessThan(actual, compared);

			int compared2 = 12;
			XAssert.Throws<ArgumentException>(() => SAssert.LessThan(actual, compared2));
		}

		[Fact]
		public void LessThanOrEqualsTest()
		{
			int actual = 32;
			int compared = 64;
			int compared2 = 32;
			SAssert.LessThanOrEqualsTo(actual, compared);
			SAssert.LessThanOrEqualsTo(actual, compared2);

			int compared3 = 12;
			XAssert.Throws<ArgumentException>(() => SAssert.LessThanOrEqualsTo(actual, compared3));
		}

		[Fact]
		public void BetweenTest()
		{
			int actual = 32;
			int lower = 20;
			int higher = 100;
			SAssert.Between(actual, lower, higher);

			int actual2 = 5;
			int actual3 = 200;
			XAssert.Throws<ArgumentException>(() => SAssert.Between(actual2, lower, higher));
			XAssert.Throws<ArgumentException>(() => SAssert.Between(actual3, lower, higher));
		}
	}
}