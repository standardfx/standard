using System;
using System.IO;
using System.Diagnostics;
using Standard;
using Xunit;

namespace Standard.Data.Json.Tests
{
    public class DateTimeTests
    {
		[Fact]
		public void TestDateTimeFormat()
		{
			var json = "{\"DateTimeValue\":\"\\/Date(1447003080000+0200)\\/\"}";
			var json2 = "{\"DateTimeValue\":\"2015-11-08T19:18:00+02:00\"}";

			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.Default, TimeZone = JsonTimeZoneHandling.Default };
			var settings2 = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.JsonNetISO, TimeZone = JsonTimeZoneHandling.Default };

			var obj = JsonConvert.Deserialize<TestDateTimeFormatting>(json, settings);
			var sobj = JsonConvert.Serialize(obj, settings);

			var obj2 = JsonConvert.Deserialize<TestDateTimeFormatting>(json2, settings2);
			var sobj2 = JsonConvert.Serialize(obj2, settings2);
		}

		[Fact]
		public void TestDateTimeWithMissingZ()
		{
			var settings = new JsonSerializerSettings
			{
				DateFormat = JsonDateTimeHandling.ISO,
				TimeZone = JsonTimeZoneHandling.UtcAsLocal,
				QuoteType = JsonQuoteHandling.Double
			};
			var settings2 = new JsonSerializerSettings
			{
				DateFormat = JsonDateTimeHandling.ISO,
				TimeZone = JsonTimeZoneHandling.Utc,
				QuoteType = JsonQuoteHandling.Double
			};

			var dateString = "{\"DateTimeValue\":\"2015-11-08T19:18:00\"}";
			var date = JsonConvert.Deserialize<TestDateTimeFormatting>(dateString, settings);
			//Assert.Null(date);

			var date2 = JsonConvert
				.Serialize(date, settings2)
				.Replace(".0Z", string.Empty);

			Assert.Equal(dateString, date2);
		}

		[Fact]
		public void TestSerializeDateWithMillisecondDefaultFormatLocal()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.Default, TimeZone = JsonTimeZoneHandling.Local };

			var date = DateTime.Now; // DateTime.UtcNow;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateUtcNowWithMillisecondDefaultFormatUtc()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.Default, TimeZone = JsonTimeZoneHandling.Utc };
			var date = DateTime.UtcNow;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateNowWithMillisecondDefaultFormatUtc()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.Default, TimeZone = JsonTimeZoneHandling.UtcAsLocal };
			var date = DateTime.Now;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateWithMillisecondDefaultFormatUnspecified()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.Default, TimeZone = JsonTimeZoneHandling.Default };
			var date = DateTime.Now;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateWithISOFormatUnspecified()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.ISO, TimeZone = JsonTimeZoneHandling.Default };
			var date = DateTime.Now;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateWithISOFormatLocal()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.ISO, TimeZone = JsonTimeZoneHandling.Local };
			var date = DateTime.Now;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateWithISOFormatUTC()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.ISO, TimeZone = JsonTimeZoneHandling.Utc };
			var date = new DateTime(2010, 12, 05, 1, 1, 30, 99);
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void TestSerializeDateNowUtcWithISOFormatUTC()
		{
			var settings = new JsonSerializerSettings { DateFormat = JsonDateTimeHandling.ISO, TimeZone = JsonTimeZoneHandling.Utc };

			var date = DateTime.UtcNow;
			var djson = JsonConvert.Serialize(date, settings);
			var ddate = JsonConvert.Deserialize<DateTime>(djson, settings);
			Assert.True(date == ddate);
		}

		[Fact]
		public void SerializeDateTimeOffSet()
		{
			var settings = new JsonSerializerSettings { TimeZone = JsonTimeZoneHandling.Local, DateFormat = JsonDateTimeHandling.ISO };

			var dateTimeOffset = new DateTimeOffset(DateTime.Now);
			var json = JsonConvert.Serialize(dateTimeOffset, settings);

			var dateTimeOffset2 = JsonConvert.Deserialize<DateTimeOffset>(json, settings);

			Assert.Equal(dateTimeOffset, dateTimeOffset2);
		}

		[Fact]
		public void SerializeDateTimeOffSetWithDifferentOffset()
		{
			var settings = new JsonSerializerSettings { TimeZone = JsonTimeZoneHandling.Local, DateFormat = JsonDateTimeHandling.ISO };

			var now = DateTime.Now;
			var dateTimeOffset = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, new TimeSpan(2, 0, 0));

			var json = JsonConvert.Serialize(dateTimeOffset, settings);

			var dateTimeOffset2 = JsonConvert.Deserialize<DateTimeOffset>(json, settings);

			Assert.Equal(dateTimeOffset, dateTimeOffset2);
		}

		[Fact]
		public void CanDeserialiseNullableDateTime()
		{
			var itm = new DateTime(2015, 12, 15);
			var testObj = new NullableTestType<DateTime>(itm);
			var serialised = JsonConvert.Serialize(testObj);
			var deserialised = JsonConvert.Deserialize<NullableTestType<DateTime>>(serialised);

			Assert.NotNull(deserialised);
			Assert.NotNull(deserialised.TestItem);
			Assert.Equal(testObj.TestItem.Value, itm);
		}

		[Fact]
		public void CanDeserialiseNullableTimespan()
		{
			var itm = new TimeSpan(1500);
			var testObj = new NullableTestType<TimeSpan>(itm);
			var serialised = JsonConvert.Serialize(testObj);
			var deserialised = JsonConvert.Deserialize<NullableTestType<TimeSpan>>(serialised);

			Assert.NotNull(deserialised);
			Assert.NotNull(deserialised.TestItem);
			Assert.Equal(testObj.TestItem.Value, itm);
		}

		[Fact]
		public void TestSerializationForMicrosoftJavascriptSerializer()
		{
			// 2017-3-31 7:28:53
			var json = "{\"CreatorId\":35,\"udtCreationDate\":\"\\/Date(1490945333848)\\/\"}";
			var data = JsonConvert.Deserialize<MicrosoftJavascriptSerializerTestData>(
				json,
				new JsonSerializerSettings
				{
					DateFormat = JsonDateTimeHandling.MicrosoftJsonDate,
					TimeZone = JsonTimeZoneHandling.Utc
				});

			Assert.Equal(35, data.CreatorId);
			Assert.Equal(31, data.udtCreationDate.Day);
			Assert.Equal(3, data.udtCreationDate.Month);
			Assert.Equal(2017, data.udtCreationDate.Year);
			Assert.Equal(7, data.udtCreationDate.Hour);
			Assert.Equal(28, data.udtCreationDate.Minute);
			Assert.Equal(53, data.udtCreationDate.Second);
		}
	}
}
