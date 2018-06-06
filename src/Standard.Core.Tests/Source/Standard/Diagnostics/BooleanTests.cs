using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class BooleanTests
    {
        [Fact]
        public void TrueTest()
        {
			int actual = 32;
			int compared = 32;
			SAssert.True(actual == compared);

			int compared2 = 23;
			XAssert.Throws<ArgumentException>(() => SAssert.True(actual == compared2));
        }

		[Fact]
		public void FalseTest()
		{
			int actual = 32;
			int compared = 23;
			SAssert.False(actual == compared);

			int compared2 = 32;
			XAssert.Throws<ArgumentException>(() => SAssert.False(actual == compared2));
		}
	}
}