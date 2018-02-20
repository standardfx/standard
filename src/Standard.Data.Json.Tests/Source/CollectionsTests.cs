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
    public class CollectionsTests
    {
		[Fact]
		public void TestIEnumerableClassHolder()
		{
			var d = new TestEnumerableClass { Data = new List<string> { "a", "b" } };
			var json = JsonConvert.Serialize(d);
			var d2 = JsonConvert.Deserialize<TestEnumerableClass>(json);
			Assert.True(d2.Data.Count() == d.Data.Count());
		}

		[Fact]
		public void TestFailedDeserializeOfNullableList()
		{
			var listObj = new List<TestJsonClass>()
			{
				new TestJsonClass { time = DateTime.UtcNow.AddYears(1) , id = 1 },
				new TestJsonClass { time = DateTime.UtcNow.AddYears(2), id = 2 },
				new TestJsonClass { time = DateTime.UtcNow.AddYears(3), id = 3 }
			};

			var settings = new JsonSerializerSettings { SkipDefaultValue = true, DateFormat = JsonDateTimeHandling.ISO };

			var json = JsonConvert.Serialize(listObj, settings);
			//[{"id":0,"time":"1-01-01T00:00:00.0"},{"id":0,"time":"1-01-01T00:00:00.0"},{"id":0,"time":"1-01-01T00:00:00.0"}]

			listObj = JsonConvert.Deserialize<List<TestJsonClass>>(json);
		}

		[Fact]
		public void SerializeDictionaryWithShortType()
		{
			//This is not required since short is already handled
			//JsonConvert.RegisterTypeSerializer<short>(ToShortString);

			short shortVar = 1;
			int intVar = 1;
			Dictionary<string, object> diccionario = new Dictionary<string, object>();
			diccionario.Add("exampleKey one", shortVar);
			string result = JsonConvert.Serialize(diccionario);
			//Console.WriteLine(result);
			diccionario.Add("exampleKey two", intVar);
			result = JsonConvert.Serialize(diccionario);
		}

		[Fact]
		public void ShouldSerializeDictionaryWithComplexDictionaryString()
		{
			//var settings = new JsonSerializerSettings { IncludeFields = true };

			Dictionary<string, string> sub1 = new Dictionary<string, string> { { "k1", "v1\"well" }, { "k2", "v2\"alsogood" } };
			var sub1Json = JsonConvert.Serialize(sub1);
			Dictionary<string, string> main = new Dictionary<string, string>
			{
				{ "MK1", sub1Json },
				{ "MK2", sub1Json }
			};

			//At this moment we got in dictionary 2 keys with string values. Every string value is complex and actually is the other serialized Dictionary
			string final = JsonConvert.Serialize(main);

			//Trying to get main dictionary back and it fails
			var l1 = JsonConvert.Deserialize<Dictionary<string, string>>(final);
			Assert.True(l1.Count == 2);
		}

		[Fact]
		public void ShouldSerializeDictionaryWithColon()
		{
			var dict = new Dictionary<string, string>();
			dict["Test:Key"] = "Value";
			var json = JsonConvert.Serialize(dict);
			var ddict = JsonConvert.Deserialize<Dictionary<string, string>>(json);
		}

		[Fact]
		public void DictionaryWithEncodedStringParsingWithDictionaryStringObject()
		{
			const string testJsonString = "{\"foo\":\"bar \\\"xyzzy\\\" \"}";

			var deserializedDictionary = (Dictionary<string, object>)JsonConvert.Deserialize<object>(testJsonString);

			var fooValue = (string)deserializedDictionary["foo"];

			Assert.Equal("bar \"xyzzy\" ", fooValue);
		}

		[Fact]
		public void ShouldSerializeEnumInDictionaryObject()
		{
			var dict = new Dictionary<string, object>();
			dict["Test"] = MyEnumTest.Test2;
			dict["Text"] = "Hello World";

			var json = JsonConvert.Serialize(dict, new JsonSerializerSettings { EnumAsString = true });
		}
	}
}
