using System;
using System.IO;
using System.Diagnostics;
using Standard;
using Xunit;

namespace Standard.Data.Json.Tests
{
    public class PrimitiveTests
    {
		[Fact]
		public void TestObjectDeserialize()
		{
			var value = "\"Test\"";
			var obj = JsonConvert.Deserialize<object>(value);
		}

		[Fact]
		public void CanDeserialiseNullableGuid()
		{
			var itm = new Guid("10b5a72b-815f-4e64-90bf-cb250840e989");
			var testObj = new NullableTestType<Guid>(itm);
			var serialised = JsonConvert.Serialize(testObj);
			var deserialised = JsonConvert.Deserialize<NullableTestType<Guid>>(serialised);

			Assert.NotNull(deserialised);
			Assert.NotNull(deserialised.TestItem);
			Assert.Equal(testObj.TestItem.Value, itm);
		}

		[Fact]
		public void TestNullPrimitiveTypes()
		{
			var value = JsonConvert.Deserialize<string>(default(string));

			Assert.Null(value);
		}

		[Fact]
		public void TestSerializeByteArray()
		{
			var buffer = new byte[10];
			new Random().NextBytes(buffer);
			var json = JsonConvert.Serialize(buffer);
			var data = JsonConvert.Deserialize<byte[]>(json);
			Assert.True(data.Length == buffer.Length);
		}

		[Fact]
		public void TestSerializePrimitiveTypes()
		{
			var x = 10;
			var s = "Hello World";
			var d = DateTime.Now;

			var xjson = JsonConvert.Serialize(x);
			var xx = JsonConvert.Deserialize<int>(xjson);

			var sjson = JsonConvert.Serialize(s);
			var ss = JsonConvert.Deserialize<string>(sjson);

			var djson = JsonConvert.Serialize(d);
			var dd = JsonConvert.Deserialize<DateTime>(djson);

			var ejson = JsonConvert.Serialize(SampleEnum.TestEnum1);
			var ee = JsonConvert.Deserialize<SampleEnum>(ejson);

			var bjson = JsonConvert.Serialize(true);
			var bb = JsonConvert.Deserialize<bool>(bjson);
		}

		[Fact]
		public void ShouldNotThrowInvalidJsonForNullPrimitiveTypes()
		{
			var value = JsonConvert.Deserialize<string>(default(string));
			Assert.Null(value);
		}

		[Fact]
		public void ShouldNotThrowInvalidJsonForPrimitiveTypes()
		{
			var value = JsonConvert.Deserialize<string>("\"abc");
			Assert.Equal("\"abc", value);
		}
	}
}
