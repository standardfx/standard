//using DeepEqual.Syntax;
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

#if NETSTANDARD
using Microsoft.Extensions.DependencyModel;
#endif

namespace Standard.Data.Json.Tests 
{
    // for ($i = 0; $i -lt $a.Length; $i++) {
    // if ($a[$i].Trim().Contains('[Fact]')) {
    // $fact = $a[$i + 1].Trim()
    // if ($fact.StartsWith('public void ')) { $fact = $fact.Substring('public void '.Length) }
    // if ($fact.EndsWith('()')) { $fact = $fact.Substring(0, $fact.Length - '()'.Length) }
    // $fact
    // }}

    public class SerializerTests 
    {
#if NETSTANDARD1_3
        public SerializerTests()
        {
            JsonConvert.EntryAssembly = typeof(SerializerTests).GetTypeInfo().Assembly;
            JsonConvert.LoadedAssemblies = DependencyContext.Default.GetDefaultAssemblyNames().Select(x => Assembly.Load(x)).ToArray();
        }           
#endif
		private class MyPrivateClass
		{
			public int ID { get; set; }
			public string Name { get; set; }
			public MyPrivateClass Inner { get; set; }
		}

		private class StubbornClass
		{
			public string FileName { get; set; }
			public double Lat { get; set; }
			public double Long { get; set; }
		}

		/*
        public void TestComplexObjectWithByteArray()
        {
            JsonConvert.IncludeTypeInformation = true;
            var obj = new ComplexObject();
            var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { EnumAsString = true });
            var obj2 = JsonConvert.Deserialize<ComplexObject>(json, new JsonSerializerSettings() { EnumAsString = true });

            Assert.True(obj.Thing6.IsDeepEqual(obj2.Thing6));
        }

        public void TestComplexObjectWithByteArrayWithSerializeType()
        {
            JsonConvert.IncludeTypeInformation = true;
            var obj = new ComplexObject();
            var json = JsonConvert.Serialize(typeof(ComplexObject), obj);
            var obj2 = JsonConvert.Deserialize<ComplexObject>(json);

            Assert.True(obj.Thing6.IsDeepEqual(obj2.Thing6));
        }

        [Fact]
        public void TestRootObjectWithInfiniteLoop() 
        {
            //JsonConvert.GenerateAssembly = true;
            //var json = TestHelper.GetEmbedFileContent("json_test.txt");
            //var root = JsonConvert.Deserialize<Root2>(json);
        }
        */

        [Fact]
        public void InvalidUnicodeEscapeSequenceShouldThrowException()
        {
            Assert.Throws<InvalidJsonException>(() => JsonConvert.Deserialize<string>("abc\\u00A"));
        }

        [Fact]
        public void SerializeEnumInDictionaryObject() 
        {
            var dict = new Dictionary<string, object>();
            dict["Test"] = MyEnumTest.Test2;
            dict["Text"] = "Hello World";

            var json = JsonConvert.Serialize(dict, new JsonSerializerSettings { EnumAsString = true });
        }

        [Fact]
        public void AutoDetectQuotes() 
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






		[Fact]
        public void CanSerializeMccUserDataObject() 
        {
            var obj = new MccUserData() { arr = new int?[] { 10, null, 20 } };

            //var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { IncludeFields = true });
            //var mjson = JsonConvert.Deserialize<MccUserData>(json);
            //var r = mjson.arr.Length;
        }

        [Fact]
        public void TestSkippingProperty() 
        {
            var ss = "{\"aaaaaaaaaa\":\"52\",\"aaaaaURL\":\"x\"}";
            var yy = JsonConvert.Deserialize<Foo>(ss);
        }

        [Fact]
        public void TestSerializeException() 
        {
            var exception = new ExceptionInfoEx 
            {
                Data = new Dictionary<string, string> { { "Test1", "Hello" } },
                ExceptionType = typeof(InvalidCastException),
                HelpLink = "HelloWorld",
                InnerException = new ExceptionInfoEx { HelpLink = "Inner" },
                Message = "Nothing here",
                Source = "Not found",
                StackTrace = "I am all here"
            };

            var json = JsonConvert.Serialize(exception);

            var exceptionResult = JsonConvert.Deserialize<ExceptionInfoEx>(json);
        }

        [Fact]
        public void TestSimpleObjectSerializationWithNull() 
        {
            var json = "{\"Id\":108591,\"EmailAddress\":\"james.brown@dummy.com\",\"FirstName\":\"James\",\"Surname\":\"Brown\",\"TitleId\":597,\"Address\":null}";
            var simple = JsonConvert.Deserialize<SimpleObjectWithNull>(json);
        }

        [Fact]
        public void TestDictionaryWithColon() 
        {
            var dict = new Dictionary<string, string>();
            dict["Test:Key"] = "Value";
            var json = JsonConvert.Serialize(dict);
            var ddict = JsonConvert.Deserialize<Dictionary<string, string>>(json);
        }

        [Fact]
        public void TestSerializeTypeClass() 
        {
            var type = typeof(String);
            var value = JsonConvert.Serialize(type);

            var typeType = JsonConvert.Deserialize<Type>(value);

            var typeHolder = new TypeHolder { Type = typeof(int) };
            var valueHolder = JsonConvert.Serialize(typeHolder);

            var typeHolderType = JsonConvert.Deserialize<TypeHolder>(valueHolder);
        }

        [Fact]
        public void StringSkippingCauseInfiniteLoop2() 
        {
            string jsonData = "{ \"token\":\"sFdDNKjLPZJSm0+gvsD1PokoJd3YzbbsClttbWLWz50=\",\"product\":\"productblabla\",\"status\":\"SUCCESS\",\"error\":\"\" }";

            var data = JsonConvert.Deserialize<BaseApiResponse>(jsonData, new JsonSerializerSettings { OptimizeString = true });
        }

        [Fact]
        public void TestJsonProperty() 
        {
            //var settings = new JsonSerializerSettings { IncludeFields = true };

            var sample = new SampleSubstitionClass { ID = 100, Name = "Test Property", Number = 504 };

            var json = JsonConvert.Serialize(sample);
            var sData = JsonConvert.Deserialize<SampleSubstitionClass>(json);
        }

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
            var settings =  new JsonSerializerSettings 
            { 
                DateFormat = JsonDateTimeHandling.ISO,
                TimeZone = JsonTimeZoneHandling.UtcAsLocal,
                QuoteType = JsonQuoteHandling.Double
            };
            var settings2 =  new JsonSerializerSettings 
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
        public void NullableWithDefaultValueSetSerializes()
        {
            var obj = new NullableTest { x = 0, y = null };
            var settings = new JsonSerializerSettings { SkipDefaultValue = true };
            var json = JsonConvert.Serialize(obj, settings);
            Assert.Equal("{\"x\":0}", json);
        }

        [Fact]
        public void NonDefaultNullableValueSerializes()
        {
            var obj = new NullableTest { x = 5 };
            var settings = new JsonSerializerSettings { SkipDefaultValue = true };
            var json = JsonConvert.Serialize(obj, settings);
            Assert.Equal("{\"x\":5}", json);
        }

        [Fact]
        public void TestDeserializeNullable() 
        {
            var data = JsonConvert.Deserialize<TestJson>("{\"b\": {\"val1\":1,\"val2\":null,\"val3\":3}, \"v\": [1,2,null,4,null,6], \"d\":[{\"val\":5},{\"val\":null}]}");
        }

        public class InvalidJsonStringClass 
        {
            public string ScreenId { get; set; }
            public string StepType { get; set; }
            public string Text { get; set; }
            public string Title { get; set; }
        }

        [Fact]
        public void TestInvalidJson() 
        {
            var str1 = @"{
    ""ScreenId"": ""Error"",
    ""StepType"": ""Message"",
    ""Text"": ""No se ha encontrado la pagina a la que usted queria ingresar."",
    ""Title"": ""Pagina no encontrada""
}";
            var str2 = @"{
    ""ScreenId"": ""CRM.IDENTIFICADOR"",
    ""StepType"": ""Screen"",
    ""Title"": ""Identificaci&oacute;n de cliente""
}";
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++) 
            {
                tasks.Add(Task.Run(() => 
                {
                    var data = JsonConvert.Deserialize<InvalidJsonStringClass>(str1);
                    var data2 = JsonConvert.Deserialize<Dictionary<string, string>>(str2);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        [Fact]
        public void SerializeObjectWithQuotes() 
        {
            var obj = new APIQuote { createDate = DateTime.Now, value = "Hello world" };
            var json = JsonConvert.Serialize(obj);
            var obj2 = JsonConvert.Deserialize<APIQuote>(json);
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
        public void TestSerializeAlwaysContainsQuotesEvenAfterBeenSerializedInDifferentThreads() 
        {
            var api = new APIQuote { value = "Test" };
            var json = JsonConvert.Serialize(api, new JsonSerializerSettings { QuoteType = JsonQuoteHandling.Single });
            var json2 = string.Empty;
            Task.Run(() => 
            {
                json2 = JsonConvert.Serialize(api, new JsonSerializerSettings { QuoteType = JsonQuoteHandling.Single });
            }).Wait();

            Assert.True(json.Equals(json2), json2);
        }

        [Fact]
        public void TestSerializeDictionaryWithComplexDictionaryString() 
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
        public void SerializeAnonymous()
        {
            var test = new { ID = 100, Name = "Test", Inner = new { ID = 100, N = "ABC" } };
            var json = JsonConvert.Serialize(test);
            Assert.True(json != null);
        }


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
        public void WhenSerializingAnonymousObjects()
        {
            var logEvents = Enumerable.Range(1, 100)
            .Select(n => new LogEvent
            {
                Timestamp = DateTime.UtcNow,
                Level = n % 2 == 0 ? Level.Debug : Level.Trace,
                Entry = n.ToString()
            });

            var anonymousObjects = logEvents
                .Select(x => new
                {
                    TimestampEpoch = x.Timestamp,
                    x.Level,
                    Message = x.Entry
                });

            var json = JsonConvert.Serialize(anonymousObjects);
            var resultAsDynamic = JsonConvert.Deserialize<dynamic>(json);
            var resultAsObject = JsonConvert.Deserialize<object>(json);
            var resultAsProjected = JsonConvert.Deserialize<List<Projected>>(json);
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
        public void TestObjectDeserialize() 
        {
            var value = "\"Test\"";
            var obj = JsonConvert.Deserialize<object>(value);
        }

        [Fact]
        public void TestJsonPropertyWithTrackerClassAndFailedJson()
        {
            string text = "{\n\"Tracker_SortBy\":\"Relevance\",\n\"Tracker_Name\":\"Wind Power Org\",\n\"Profile_Tracker_ID\":1428,\n\"Tracker_ContentType\":\"1\",\n\"Tracker_SearchTerm\":\"Wind Power\",\n\"Tracker_Facets\":[\n{\"Tracker_Facet\":\"{fb8d09e1-5024-419e-9703-598945af8139}\"},\n{\"Tracker_Facet\":\"{90ec93d4-e1bd-4215-acf0-eac1e2fd5f6d}\"}]\n}";
            Tracker t = JsonConvert.Deserialize<Tracker>(text);

            Assert.True(t.FacetCollection.Count > 0);
            Assert.True(t.ID > 0);
            Assert.True(t.ContentType != null);
            Assert.True(t.Name != null);
            Assert.True(t.SearchTerm != null);
            Assert.True(t.SortBy != null);
        }

        [Fact]
        public void TestJsonPropertyTrackerClass()
        {
            var json = @"{  
      ""Tracker_Name"":""xxxx x"",  
      ""Tracker_SearchTerm"":""Wind Power"",    
      ""Tracker_SortBy"":""Relevance""  
    }";

            var json2 = @"{
""Tracker_Name"":""xxxx x x"",
""Tracker_SearchTerm"":""Wind Power"",
""Tracker_SortBy"":""Relevance""
}";
            
            var tracker = JsonConvert.Deserialize<Tracker>(json);
            var tracker2 = JsonConvert.Deserialize<Tracker>(json2);
            Tracker tracker3 = null;

            using (TextReader reader = new StringReader(TestHelper.GetEmbedFileContent("tracker.json")))
            {
                tracker3 = JsonConvert.Deserialize<Tracker>(reader);
            }

            Assert.True(tracker.Name == "xxxx x", tracker.Name);
            Assert.True(tracker.SearchTerm == "Wind Power", tracker.SearchTerm);
            Assert.True(tracker.SortBy == "Relevance", tracker.SortBy);

            Assert.True(tracker2.Name == "xxxx x x", tracker2.Name);
            Assert.True(tracker2.SearchTerm == "Wind Power", tracker2.SearchTerm);
            Assert.True(tracker2.SortBy == "Relevance", tracker2.SortBy);

            Assert.True(tracker3.Name == "xxxx x", tracker3.Name);
            Assert.True(tracker3.SearchTerm == "Wind Power", tracker3.SearchTerm);
            Assert.True(tracker3.SortBy == "Relevance", tracker3.SortBy);
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
        public void TestEscapeSequenceForPrimitiveTypes()
        {
            Assert.Equal("\"abc", JsonConvert.Deserialize<string>("\"abc"));
            Assert.Equal("abc\"", JsonConvert.Deserialize<string>("abc\""));
            Assert.Equal("abc", JsonConvert.Deserialize<string>("abc\\"));
        }

        [Fact]
        public void TestNullPrimitiveTypes()
        {
            var value = JsonConvert.Deserialize<string>(default(string));

            Assert.Null(value);
        }

        [Fact]
        public void TestJsonFile() 
        {
            var evnts = JsonConvert.Deserialize<EvntsRoot>(
                TestHelper.GetEmbedFileContent("json.json"), 
                new JsonSerializerSettings { IgnoreCase = false });
        }

        [Fact]
        public void TestSerializeTuple() 
        {
            var tuple = new Tuple<int, string>(100, "Hello World");

            var json = JsonConvert.Serialize(tuple);
            var ttuple = JsonConvert.Deserialize<Tuple<int, string>>(json);
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

        [Fact]
        public void DtoSerialization() 
        {
            string json;
            List<MyDto> clone;
            int count = 30000;
            var list = new List<MyDto>();
            for (int i = 0; i < count; i++) 
            {
                list.Add(new MyDto { ID = i + 1 });
            }

            json = JsonConvert.Serialize(list);
            clone = JsonConvert.Deserialize<List<MyDto>>(json);

            Assert.True(clone.Count == count);
        }

        [Fact]
        public void TestSerializeComplexTuple() 
        {
            var tuple = new Tuple<int, DateTime, string,
                Tuple<double, List<string>>>(1, DateTime.Now, "xisbound",
                	new Tuple<double, List<string>>(45.45, new List<string> { "hi", "man" }));

            var json = JsonConvert.Serialize(tuple);
            var ttuple = JsonConvert.Deserialize<Tuple<int, DateTime, string, Tuple<double, List<string>>>>(json);
        }

        [Fact]
        public void StringSkippingCauseInfiniteLoop() 
        {
            string jsonData = "{\"jsonrpc\":\"2.0\",\"result\":{\"availableToBetBalance\":602.15,\"exposure\":0.0,\"retainedCommission\":0.0,\"exposureLimit\":-10000.0,\"discountRate\":2.0,\"pointsBalance\":1181,\"wallet\":\"UK\"},\"id\":1}";

            var data = JsonConvert.Deserialize<JsonRpcResponse<AccountFundsResponse>>(jsonData);
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
        public void PrettifyString() 
        {
            var data = new StructWithProperties { x = 10, y = 2, Value = "Data Source=[DataSource,];Initial Catalog=[Database,];User ID=[User,];Password=[Password,];Trusted_Connection=[TrustedConnection,False]" };
            var json = JsonConvert.Serialize(data, new JsonSerializerSettings { Indent = JsonIndentHandling.Prettify });
            var count = json.Split('\n').Length;

            Assert.True(count > 1);
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
        public void TestPossibleInfiniteLoopReproduced() 
        {
            //var obj = new TestNullableNullClass { ID  = 1, Name = "Hello" };
            var json = "{\"ID\": 2, \"Name\": \"Hello world\"}";
            var obj = JsonConvert.Deserialize<TestNullableNullClass>(json);
        }

        [Fact]
        public void NestedGraphDoesNotThrow() 
        {
            var o = new GetTopWinsResponse() 
            {
                TopWins = new List<TopWinDto>()
                {
                    new TopWinDto()
                    {
                        Amount = 1,
                        LandBasedCasino = new TopWinLandBasedCasino()
                        {
                            Location = "Location",
                            MachineName = "Machinename"
                        },
                        Nickname = "Nickname",
                        OnlineCasino = new TopWinOnlineCasino()
                        {
                            GameId = "GameId"
                        },
                        OnlineSports = new TopWinOnlineSports()
                        {
                            AwayTeam = "AwayTeam"
                        },
                        Timestamp = DateTime.Now,
                        Type = TopWinType.LandBasedCasinoWin
                    }
                }
            };

            var actual = JsonConvert.Serialize(o.GetType(), o);
            var data = JsonConvert.Deserialize<GetTopWinsResponse>(actual);
            Assert.True(o.TopWins.Count() == data.TopWins.Count());
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
        public void CanGenerateCamelCaseProperty() 
        {
            var obj = new TopWinOnlineCasino { GameId = "TestGame" };
            var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { CamelCase = true });
            Assert.Contains("gameId", json);
        }

        [Fact]
        public void CannotGenerateCamelCaseProperty() 
        {
            var obj = new TopWinOnlineCasino { GameId = "TestGame" };
            var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { CamelCase = false });
            Assert.Contains("GameId", json);
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
        public void ShouldFailWithBadJsonException()
        {
            Assert.Throws<InvalidJsonException>(() => JsonConvert.Deserialize<Person>(""));
        }

        [Fact]
        public void CaseSensitiveGlossary() 
        {
            var json = @"{
    ""glossary"": {
        ""title"": ""example glossary"",
        ""GlossDiv"": {
            ""title"": ""S"",
            ""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": ""SGML"",
                    ""SortAs"": ""SGML"",
                    ""GlossTerm"": ""Standard Generalized Markup Language"",
                    ""Acronym"": ""SGML"",
                    ""Abbrev"": ""ISO 8879:1986"",
                    ""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
                        ""GlossSeeAlso"": [""GML"", ""XML""]
                    },
                    ""GlossSee"": ""markup""
                }
            }
        }
    }
}";
            var obj = JsonConvert.Deserialize<GlossaryContainer>(json, new JsonSerializerSettings { IgnoreCase = true });
            Assert.NotNull(obj.glossary.glossdiv);
        }

        [Fact]
        public void TestPersonClassWithMultipleNonDefaultConstructor() 
        {
            var json = "{ \"name\": \"boss\", \"age\": 2, \"reasonForUnknownAge\": \"he is the boss\" }";
            var data = JsonConvert.Deserialize<PersonTest>(json, new JsonSerializerSettings { IgnoreCase = true });
            Assert.True(data.Age == 2);
        }

        [Fact]
        public void DeserializeStubbornClass()
        {
            var one = "{\"FileName\":\"973c6d92-819f-4aa1-a0b4-7a645cfea189\",\"Lat\":0,\"Long\":0}";
            var two = "{\"FileName\":\"973c6d92-819f-4aa1-a0b4-7a645cfea189\",\"Lat\":0,\"Long\":0}\n";

            var stubbornOne = JsonConvert.Deserialize(typeof(StubbornClass), one);
            var stubbornTwo = JsonConvert.Deserialize(typeof(StubbornClass), two);
        }

        [Fact]
        public void DeserializeJsonWithMissingQuote()
        {
            var json = @"{
	""document"": ""base64string,
	""documentName"": ""test.pdf"",
	""label1"": ""someLabel"",
	""packageId"": ""7db3eacf-1d2b-4142-9eab-b1bce4630570"",
	""initiator"": ""somerandom @email.com""
}";
            var ex = default(Exception);

            try
            {
                JsonConvert.Deserialize<Dictionary<string, string>>(json);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.NotNull(ex);
        }

        [Fact]
        public void TestSkipDefaultValueWithSetting() 
        {
            var model = new Computer 
            {
                Timestamp = 12345,
                Processes = Enumerable.Range(0, 100)
                    .Select(x => new Process 
                    {
                        Id = (uint)x,
                        Name = "P: " + x.ToString(),
                        Description = "This is process " + x.ToString()
                    })
                    .ToArray(),

                OperatingSystems = Enumerable.Range(0, 50)
                    .Select(x => new OperatingSystem 
                    {
                        Name = "OS - " + x.ToString(),
                        Version = "0.0.0." + x.ToString(),
                        Price = (decimal)(x * 0.412),
                        Disks = new[]
                        {
		                    new Disk
		                    {
		                        Name = "Disk: " + x.ToString(),
		                        Capacity = x * 100
		                    },
		                    new Disk
		                    {
		                        Name = "Disk: " + x.ToString(),
		                        Capacity = x * 100
		                    }
                        }
                    })
                    .ToArray()
            };

            var setting = new JsonSerializerSettings { SkipDefaultValue = false };

            var modelAsJson = JsonConvert.Serialize(model, setting);
            var modelFromJson = JsonConvert.Deserialize<Computer>(modelAsJson, setting);
            Assert.Equal(model.Processes[0].Id, modelFromJson.Processes[0].Id);
        }

        [Fact]
        public void TestIEnumerableClassHolder() 
        {
            var d = new TestEnumerableClass { Data = new List<string> { "a", "b" } };
            var json = JsonConvert.Serialize(d);
            var d2 = JsonConvert.Deserialize<TestEnumerableClass>(json);
            Assert.True(d2.Data.Count() == d.Data.Count());
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
