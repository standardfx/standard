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
    public class EscapingTests
    {
		[Fact]
		public void TestEscapeSequenceForPrimitiveTypes()
		{
			Assert.Equal("\"abc", JsonConvert.Deserialize<string>("\"abc"));
			Assert.Equal("abc\"", JsonConvert.Deserialize<string>("abc\""));
			Assert.Equal("abc", JsonConvert.Deserialize<string>("abc\\"));
		}

		[Fact]
		public void ShouldDeserializeValueWithDoubleQuotes()
		{
			var settings = new JsonSerializerSettings
			{
				IgnoreCase = true
			};

			// Actual string { Val : "\"sampleValue\""} before escape characters
			var stringToDeserialize = "{ \"Val\" : \"\\\"sampleValue\\\"\"}";

			var result = JsonConvert.Deserialize<DoubleQuoter>(stringToDeserialize, settings);

			Assert.NotNull(result);
			Assert.NotNull(result.Val);
		}

		[Fact]
		public void ShouldThrowOnInvalidUnicodeEscape()
		{
			Assert.Throws<InvalidJsonException>(() => JsonConvert.Deserialize<string>("abc\\u00A"));
		}

		[Fact]
		public void ShouldAutoDetectQuotes()
		{
			var dict = new Dictionary<string, string>();
			dict["Test"] = "Test2";
			dict["Test2"] = "Test3";

			var list = new List<string>
			{
				"Test",
				"Test2"
			};

			var str = "Test";
			var settings = new JsonSerializerSettings { QuoteType = JsonQuoteHandling.Single };

			var json = JsonConvert.Serialize(dict, settings);
			var jsonList = JsonConvert.Serialize(list, settings);
			var jsonStr = JsonConvert.Serialize(str, settings);

			var jsonWithDouble = json.Replace("'", "\"");
			var jsonListWithDouble = jsonList.Replace("'", "\"");
			var jsonStrWithDouble = jsonStr.Replace("'", "\"");

			var result = JsonConvert.Deserialize<Dictionary<string, string>>(jsonWithDouble);
			var result2 = JsonConvert.Deserialize<List<string>>(jsonListWithDouble);
			var result3 = JsonConvert.Deserialize<string>(jsonStrWithDouble);
		}
	}

	public class DoubleQuoter
	{
		public string Val { get; set; }
	}
}
