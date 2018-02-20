using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace Standard.Data.Json.Tests
{
	public class InterfaceTests
	{
		[Fact]
		public void ShouldSerializeInterfaceType()
		{
			JsonConvert.IncludeTypeInfo = true;

			IPerson p1 = new PersonX { Name = "Bob" };

			var json = JsonConvert.Serialize(p1);

			Assert.True(!string.IsNullOrEmpty(json));

			JsonConvert.IncludeTypeInfo = false;
		}
	}

	public class AbstractClassTests
	{
		[Fact]
		public void ShouldSerializeAbstractClass()
		{
			PersonAbstract p1 = new PersonX2 { Name = "Bob" };
			var json = JsonConvert.Serialize(p1);

			Assert.True(!string.IsNullOrEmpty(json));
		}
	}

	public interface IPerson
	{
		string Name { get; set; }
	}

	public class PersonX : IPerson
	{
		public string Name { get; set; }
	}

	public abstract class PersonAbstract
	{
		public abstract string Name { get; set; }
	}

	public class PersonX2 : PersonAbstract
	{
		public override string Name { get; set; }
	}
}
