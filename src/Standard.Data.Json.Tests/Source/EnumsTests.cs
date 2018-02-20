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
    public class EnumsTests
    {
		[Fact]
		public void SerializeEnumValueWithoutCaseUsingAttribute()
		{
			var value = MyEnumTestValue.V1;
			var settings = new JsonSerializerSettings { EnumAsString = true };

			var json = JsonConvert.Serialize(value, settings);
			var value2 = JsonConvert.Deserialize<MyEnumTestValue>(json, settings);
			var value3 = JsonConvert.Deserialize<MyEnumTestValue>(json.Replace("V_1", "V_2"), settings);

			Assert.True(value2 == value);
			Assert.True(value3 == MyEnumTestValue.V2);
		}

		[Fact]
		public void SerializeEnumValueUsingAttribute()
		{
			var settings = new JsonSerializerSettings { EnumAsString = true };
			var obj = new MyEnumClassTest { Name = "Test Enum", Value = MyEnumTestValue.V1 };
			var json = JsonConvert.Serialize(obj, settings);

			var obj2 = JsonConvert.Deserialize<MyEnumClassTest>(json, settings);

			var obj3 = JsonConvert.Deserialize<MyEnumClassTest>(json.Replace("V_1", "V_3"), settings);

			Assert.True(obj.Value == obj2.Value);
			Assert.True(obj3.Value == MyEnumTestValue.V3);
		}

		[Fact]
		public void TestSerializeEnumFlag()
		{
			var eStr = JsonConvert.Serialize(System.IO.FileShare.Read | System.IO.FileShare.Delete, new JsonSerializerSettings { EnumAsString = true });
			var eInt = JsonConvert.Serialize(System.IO.FileShare.Read | System.IO.FileShare.Delete, new JsonSerializerSettings { EnumAsString = false });

			Assert.True(eStr == "\"Read, Delete\"");
			Assert.True(eInt == "5");
		}

		[Fact]
		public void TestEnumHolderWithByteAndShort()
		{
			var settings = new JsonSerializerSettings { EnumAsString = false };
			var value = new EnumHolder { BEnum = ByteEnum.V2, SEnum = ShortEnum.V2 };
			var json = JsonConvert.Serialize(value, settings);

			var bJson = JsonConvert.Serialize(ByteEnum.V2, settings);
			var sJson = JsonConvert.Serialize(ShortEnum.V2, settings);

			var bValue = JsonConvert.Deserialize<ByteEnum>(bJson, settings);
			var sValue = JsonConvert.Deserialize<ShortEnum>(sJson, settings);

			var value2 = JsonConvert.Deserialize<EnumHolder>(json, settings);

			Assert.True(value.BEnum == value2.BEnum);
			Assert.True(value.SEnum == value2.SEnum);
		}


		[Fact]
		public void TestEnumFlags()
		{
			var foob = new FooA
			{
				IntVal = 1,
				EnumVal = TestFlags.A | TestFlags.B,
				Type = 2
			};

			var settings = new JsonSerializerSettings { EnumAsString = true };
			var json = JsonConvert.Serialize((FooA)foob, settings);
			var obj = JsonConvert.Deserialize<FooA>(json, settings);

			Assert.Equal(obj.EnumVal, foob.EnumVal);
		}
	}
}
