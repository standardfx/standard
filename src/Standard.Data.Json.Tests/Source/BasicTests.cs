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
    public class BasicTests
    {
		[Fact]
		public void StringSkippingCauseInfiniteLoop()
		{
			string jsonData = "{\"jsonrpc\":\"2.0\",\"result\":{\"availableToBetBalance\":602.15,\"exposure\":0.0,\"retainedCommission\":0.0,\"exposureLimit\":-10000.0,\"discountRate\":2.0,\"pointsBalance\":1181,\"wallet\":\"UK\"},\"id\":1}";

			var data = JsonConvert.Deserialize<JsonRpcResponse<AccountFundsResponse>>(jsonData);
		}

		[Fact]
		public void TestPossibleInfiniteLoopReproduced()
		{
			//var obj = new TestNullableNullClass { ID  = 1, Name = "Hello" };
			var json = "{\"ID\": 2, \"Name\": \"Hello world\"}";
			var obj = JsonConvert.Deserialize<TestNullableNullClass>(json);
		}

		[Fact]
		public void StringSkippingCauseInfiniteLoop2()
		{
			string jsonData = "{ \"token\":\"sFdDNKjLPZJSm0+gvsD1PokoJd3YzbbsClttbWLWz50=\",\"product\":\"productblabla\",\"status\":\"SUCCESS\",\"error\":\"\" }";

			var data = JsonConvert.Deserialize<BaseApiResponse>(jsonData, new JsonSerializerSettings { OptimizeString = true });
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
		public void TestSerializeTuple()
		{
			var tuple = new Tuple<int, string>(100, "Hello World");

			var json = JsonConvert.Serialize(tuple);
			var ttuple = JsonConvert.Deserialize<Tuple<int, string>>(json);
		}
	}
}
