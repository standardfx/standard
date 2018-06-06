using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SAssert = Standard.Diagnostics.Assert;
using XAssert = Xunit.Assert;

namespace Standard.Diagnostics.Core.Tests
{
    public class DefaultTests
    {
		internal struct Foo
		{
			private int x;

			public Foo(int x)
			{
				this.x = x;
			}
		}

        [Fact]
        public void DefaultStructTest()
        {
			Foo f = new Foo(32);
			Foo f2 = default(Foo);

			SAssert.NotDefault(f);
			XAssert.Throws<ArgumentException>(() => SAssert.NotDefault(f2));
        }
	}
}