using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class Int64Tests
    {
        [Fact]
        public void EqualityTest()
        {
			long actual = 32;
			long compared = 32;
			SAssert.Equals(actual, compared);

			long compared2 = 23;
			XAssert.Throws<ArgumentException>(() => SAssert.Equals(actual, compared2));
        }

		[Fact]
		public void NonEqualityTest()
		{
			long actual = 32;
			long compared = 23;
			SAssert.NotEquals(actual, compared);

			long compared2 = 32;
			XAssert.Throws<ArgumentException>(() => SAssert.NotEquals(actual, compared2));
		}

		[Fact]
		public void GreaterThanTest()
		{
			long actual = 32;
			long compared = 23;
			SAssert.GreaterThan(actual, compared);

			int compared2 = 64;
			XAssert.Throws<ArgumentException>(() => SAssert.GreaterThan(actual, compared2));
		}

		[Fact]
		public void GreaterThanOrEqualsTest()
		{
			long actual = 32;
			long compared = 23;
			long compared2 = 32;
			SAssert.GreaterThanOrEqualsTo(actual, compared);
			SAssert.GreaterThanOrEqualsTo(actual, compared2);

			int compared3 = 64;
			XAssert.Throws<ArgumentException>(() => SAssert.GreaterThan(actual, compared3));
		}

		[Fact]
		public void LessThanTest()
		{
			long actual = 23;
			long compared = 32;
			SAssert.LessThan(actual, compared);

			long compared2 = 12;
			XAssert.Throws<ArgumentException>(() => SAssert.LessThan(actual, compared2));
		}

		[Fact]
		public void LessThanOrEqualsTest()
		{
			long actual = 32;
			long compared = 64;
			long compared2 = 32;
			SAssert.LessThanOrEqualsTo(actual, compared);
			SAssert.LessThanOrEqualsTo(actual, compared2);

			long compared3 = 12;
			XAssert.Throws<ArgumentException>(() => SAssert.LessThanOrEqualsTo(actual, compared3));
		}

		[Fact]
		public void BetweenTest()
		{
			long actual = 32;
			long lower = 20;
			long higher = 100;
			SAssert.Between(actual, lower, higher);

			long actual2 = 5;
			long actual3 = 200;
			XAssert.Throws<ArgumentException>(() => SAssert.Between(actual2, lower, higher));
			XAssert.Throws<ArgumentException>(() => SAssert.Between(actual3, lower, higher));
		}
	}
}