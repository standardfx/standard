using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class StringTests
    {
        [Fact]
        public void NotNullTest()
        {
			string actual = "afds";
			SAssert.NotNull(actual);

			XAssert.Throws<ArgumentException>(() => SAssert.NotNull(null));
        }

		[Fact]
		public void NotNullOrEmptyTest()
		{
			string actual = "afds";
			SAssert.NotNullOrEmpty(actual);

			XAssert.Throws<ArgumentException>(() => SAssert.NotNullOrEmpty(null));
			XAssert.Throws<ArgumentException>(() => SAssert.NotNullOrEmpty(string.Empty));
		}

		[Fact]
		public void NotEmptyTest()
		{
			string actual = "afds";
			SAssert.NotEmpty(actual);
			SAssert.NotEmpty(null);

			XAssert.Throws<ArgumentException>(() => SAssert.NotEmpty(string.Empty));
		}

		[Fact]
		public void NotNullOrWhiteSpaceTest()
		{
			string actual = "afds";
			SAssert.NotNullOrWhiteSpace(actual);

			XAssert.Throws<ArgumentException>(() => SAssert.NotNullOrWhiteSpace(null));
			XAssert.Throws<ArgumentException>(() => SAssert.NotNullOrWhiteSpace(string.Empty));
			XAssert.Throws<ArgumentException>(() => SAssert.NotNullOrWhiteSpace("     \t  "));
		}

		[Fact]
		public void EqualityTest()
		{
			string actual = "afds";
			string compared = "afds";
			string compared2 = "foo";
			SAssert.Equals(actual, compared);
			XAssert.Throws<ArgumentException>(() => SAssert.Equals(actual, compared2));
		}

		[Fact]
		public void NonEqualityTest()
		{
			string actual = "afds";
			string compared = "foo";
			string compared2 = "afds";

			SAssert.NotEquals(actual, compared);
			XAssert.Throws<ArgumentException>(() => SAssert.NotEquals(actual, compared2));
		}

		[Fact]
		public void MaxLengthTest()
		{
			string actual = "afds";
			SAssert.MaxLength(actual, 5);
			SAssert.MaxLength(actual, 4);
			XAssert.Throws<ArgumentException>(() => SAssert.MaxLength(actual, 2));
		}

		[Fact]
		public void MinLengthTest()
		{
			string actual = "afds";
			SAssert.MinLength(actual, 2);
			SAssert.MinLength(actual, 4);
			XAssert.Throws<ArgumentException>(() => SAssert.MinLength(actual, 6));
		}

		[Fact]
		public void LengthTest()
		{
			string actual = "afds";
			SAssert.Length(actual, 4);
			XAssert.Throws<ArgumentException>(() => SAssert.Length(actual, 6));
			XAssert.Throws<ArgumentException>(() => SAssert.Length(actual, 2));
		}

		[Fact]
		public void RegexTest()
		{
			string actual = "1~~5~~4";
			string actual2 = "a~~b~~c";
			string pattern = "[0-9]~~[0-9]~~[0-9]";
			SAssert.IsMatch(actual, pattern);
			XAssert.Throws<ArgumentException>(() => SAssert.IsMatch(actual2, pattern));
		}

		[Fact]
		public void WildcardTest()
		{
			string actual = "foo123.txt";
			string pattern = "foo*.txt";
			string pattern2 = "bar*.txt";
			SAssert.IsLike(actual, pattern);
			XAssert.Throws<ArgumentException>(() => SAssert.IsLike(actual, pattern2));
		}
	}
}