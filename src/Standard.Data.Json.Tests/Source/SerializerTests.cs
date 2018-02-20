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
        public void TestJsonProperty() 
        {
            //var settings = new JsonSerializerSettings { IncludeFields = true };

            var sample = new SampleSubstitionClass { ID = 100, Name = "Test Property", Number = 504 };

            var json = JsonConvert.Serialize(sample);
            var sData = JsonConvert.Deserialize<SampleSubstitionClass>(json);
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
        public void SerializeObjectWithQuotes() 
        {
            var obj = new APIQuote { createDate = DateTime.Now, value = "Hello world" };
            var json = JsonConvert.Serialize(obj);
            var obj2 = JsonConvert.Deserialize<APIQuote>(json);
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
		public void TestDeserializeNullable()
		{
			var data = JsonConvert.Deserialize<TestJson>("{\"b\": {\"val1\":1,\"val2\":null,\"val3\":3}, \"v\": [1,2,null,4,null,6], \"d\":[{\"val\":5},{\"val\":null}]}");
		}

		[Fact]
        public void SerializeAnonymous()
        {
            var test = new { ID = 100, Name = "Test", Inner = new { ID = 100, N = "ABC" } };
            var json = JsonConvert.Serialize(test);
            Assert.True(json != null);
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
        public void TestJsonFile() 
        {
            var evnts = JsonConvert.Deserialize<EvntsRoot>(
                TestHelper.GetEmbedFileContent("json.json"), 
                new JsonSerializerSettings { IgnoreCase = false });
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
    }
}
