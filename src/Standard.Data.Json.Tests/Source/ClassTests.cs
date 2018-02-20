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
#if FEAT_SERIALIZE_INTERFACE

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

#endif

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

	public class AccessTests
	{
		private class MyPrivateClass
		{
			public int ID { get; set; }
			public string Name { get; set; }
			public MyPrivateClass Inner { get; set; }
		}

		[Fact]
		public void TestSerializeDeserializeNonPublicType()
		{
			string s;
			var e = new List<E> { new E { V = 1 }, new E { V = 2 } };
			s = JsonConvert.Serialize(e);
			JsonConvert.Serialize(JsonConvert.Deserialize<List<E>>(s = JsonConvert.Serialize(e)));
			Assert.True(!string.IsNullOrWhiteSpace(s));
		}

		[Fact]
		public void SerializeNonPublicType()
		{
			var test = new MyPrivateClass { ID = 100, Name = "Test", Inner = new MyPrivateClass { ID = 200, Name = "Inner" } };
			var json = JsonConvert.Serialize(test);
			var data = JsonConvert.Deserialize<MyPrivateClass>(json);
			Assert.True(json != null);
		}

		[Fact]
		public void TestSerializeDeserializeNonPublicSetter()
		{
			var model = new Person("John", 12);

			var json = JsonConvert.Serialize(model);

			//var settings = new JsonSerializerSettings { IncludeTypeInformation = true };
			JsonConvert.IncludeTypeInfo = true;
			var deserializedModel = JsonConvert.Deserialize<Person>(json);
			Assert.Equal("John", deserializedModel.Name);
			Assert.Equal(12, deserializedModel.Age);
			JsonConvert.IncludeTypeInfo = false;
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
