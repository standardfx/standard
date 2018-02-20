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
    public class MalformTests
    {
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
		public void ShouldFailWithBadJsonException()
		{
			Assert.Throws<InvalidJsonException>(() => JsonConvert.Deserialize<Person>(""));
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
		public void TestDeserializeForNullErrorJsonString()
		{
			var json = "{\"success\":true,\"message\":\"\",\"result\":[{\"OrderUuid\":\"47628e98-934f-42dc-9998-eda41174214f\",\"Exchange\":\"BTC-DGB\",\"TimeStamp\":\"2017-09-05T01:30:55.613\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00000419,\"Quantity\":20000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00020950,\"Price\":0.08380000,\"PricePerUnit\":0.00000419000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-05T01:30:55.723\"},{\"OrderUuid\":\"a088f51b-0bb6-48e8-9afa-f08057f60e2b\",\"Exchange\":\"BTC-CVC\",\"TimeStamp\":\"2017-09-04T23:58:11.61\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00008820,\"Quantity\":1200.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00026459,\"Price\":0.10584299,\"PricePerUnit\":0.00008820000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T23:58:33.39\"},{\"OrderUuid\":\"ba00a497-19a7-42e8-bed9-573f12363c5a\",\"Exchange\":\"USDT-ETH\",\"TimeStamp\":\"2017-09-04T18:23:54.803\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":286.38999998,\"Quantity\":6.41404536,\"QuantityRemaining\":0.00000000,\"Commission\":4.59229611,\"Price\":1836.91845050,\"PricePerUnit\":286.38999997000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T18:24:05.197\"},{\"OrderUuid\":\"572fad3b-764f-4b23-915e-980ad59023ee\",\"Exchange\":\"BTC-NEO\",\"TimeStamp\":\"2017-09-04T09:13:44.42\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00514894,\"Quantity\":15.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00019307,\"Price\":0.07723409,\"PricePerUnit\":0.00514893000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T09:13:45.297\"},{\"OrderUuid\":\"80ed6fee-a682-47e6-a52b-883ebe228b66\",\"Exchange\":\"BTC-PTOY\",\"TimeStamp\":\"2017-09-03T23:15:11.16\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00008100,\"Quantity\":1000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00020250,\"Price\":0.08100000,\"PricePerUnit\":0.00008100000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T08:06:30.217\"},{\"OrderUuid\":\"5c25cbf5-5140-4165-86a4-cac7a62a4b77\",\"Exchange\":\"BTC-CVC\",\"TimeStamp\":\"2017-09-04T04:26:24.96\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00009111,\"Quantity\":1200.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00027333,\"Price\":0.10933200,\"PricePerUnit\":0.00009111000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T04:26:25.02\"},{\"OrderUuid\":\"6a60a3fe-2f44-43a8-b915-f841d86584a1\",\"Exchange\":\"BTC-DGB\",\"TimeStamp\":\"2017-09-04T04:25:39.733\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00000534,\"Quantity\":20000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00026699,\"Price\":0.10679998,\"PricePerUnit\":0.00000533000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-04T04:25:39.95\"},{\"OrderUuid\":\"d2409707-3641-42ee-92d1-3c1469fd787c\",\"Exchange\":\"BTC-NEO\",\"TimeStamp\":\"2017-09-01T23:17:20.593\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00600000,\"Quantity\":15.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00022500,\"Price\":0.09000000,\"PricePerUnit\":0.00600000000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-02T01:39:59.697\"},{\"OrderUuid\":\"a92fd25c-259b-428b-a281-9df1994b1cc0\",\"Exchange\":\"BTC-DGB\",\"TimeStamp\":\"2017-09-01T15:51:42.077\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00000474,\"Quantity\":15000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00017812,\"Price\":0.07125000,\"PricePerUnit\":0.00000475000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-01T15:51:42.187\"},{\"OrderUuid\":\"dd56bde2-5b08-4b13-963e-9b5b64aed0ab\",\"Exchange\":\"BTC-BTS\",\"TimeStamp\":\"2017-09-01T02:49:38.153\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00003260,\"Quantity\":3000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00024375,\"Price\":0.09750000,\"PricePerUnit\":0.00003250000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-09-01T02:49:38.87\"},{\"OrderUuid\":\"01c592e4-e8a7-4a29-a282-06da9eaed847\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-31T01:32:14.533\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00361000,\"Quantity\":25.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00022601,\"Price\":0.09040712,\"PricePerUnit\":0.00361628000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-31T01:32:14.69\"},{\"OrderUuid\":\"257cc430-2f54-4173-9914-6b9af428a5ff\",\"Exchange\":\"BTC-SNT\",\"TimeStamp\":\"2017-08-31T01:27:03.177\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00001045,\"Quantity\":2000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00005224,\"Price\":0.02089999,\"PricePerUnit\":0.00001044000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-31T01:27:03.647\"},{\"OrderUuid\":\"a878d615-2674-4113-94a9-42de4d655ebf\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-30T07:25:16.247\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00384998,\"Quantity\":25.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00023750,\"Price\":0.09500000,\"PricePerUnit\":0.00380000000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-30T07:25:16.307\"},{\"OrderUuid\":\"ed06edad-4028-488e-93f9-b3d191505ce5\",\"Exchange\":\"BTC-LTC\",\"TimeStamp\":\"2017-08-29T21:30:17.88\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.01350000,\"Quantity\":30.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00101250,\"Price\":0.40500030,\"PricePerUnit\":0.01350001000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-29T21:30:17.987\"},{\"OrderUuid\":\"6ed65196-fe1e-4beb-abb3-3630073075ab\",\"Exchange\":\"BTC-FCT\",\"TimeStamp\":\"2017-08-28T01:29:52.777\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00677000,\"Quantity\":15.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00025386,\"Price\":0.10154999,\"PricePerUnit\":0.00676999000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-29T13:48:59.93\"},{\"OrderUuid\":\"d1224d72-a014-41c6-a641-631f4505dc25\",\"Exchange\":\"BTC-MTL\",\"TimeStamp\":\"2017-08-29T04:35:07.067\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00222246,\"Quantity\":30.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00016668,\"Price\":0.06667380,\"PricePerUnit\":0.00222246000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-29T04:35:07.177\"},{\"OrderUuid\":\"7670ad26-36cc-4db3-a585-ae0d5a6cf0e9\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-29T01:57:13.33\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00597699,\"Quantity\":25.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00037356,\"Price\":0.14942475,\"PricePerUnit\":0.00597699000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-29T01:57:14.723\"},{\"OrderUuid\":\"488896b7-9a7f-4619-bf74-32a4f6e50e56\",\"Exchange\":\"BTC-DGB\",\"TimeStamp\":\"2017-08-28T23:26:07.003\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00000368,\"Quantity\":15000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00013762,\"Price\":0.05504999,\"PricePerUnit\":0.00000366000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T23:26:07.19\"},{\"OrderUuid\":\"869e97ca-2c10-44f8-9d17-b26dc0d70dac\",\"Exchange\":\"BTC-SNT\",\"TimeStamp\":\"2017-08-28T02:00:21.013\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00001169,\"Quantity\":5000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00014612,\"Price\":0.05845000,\"PricePerUnit\":0.00001169000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T02:00:21.527\"},{\"OrderUuid\":\"f2a1299b-c486-4548-a64b-7350b963d523\",\"Exchange\":\"BTC-NXT\",\"TimeStamp\":\"2017-08-28T01:24:20.477\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00002605,\"Quantity\":3200.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00020840,\"Price\":0.08336000,\"PricePerUnit\":0.00002605000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:28:04.66\"},{\"OrderUuid\":\"bca820a3-7aaf-4f28-9808-7c9b59a135e2\",\"Exchange\":\"BTC-GAME\",\"TimeStamp\":\"2017-08-28T01:09:55.843\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00053247,\"Quantity\":150.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00019967,\"Price\":0.07987049,\"PricePerUnit\":0.00053246000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:21:47.413\"},{\"OrderUuid\":\"5054479b-4ea5-4591-9355-84cf6bb1d5dd\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-28T01:11:30.16\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00366017,\"Quantity\":25.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00022874,\"Price\":0.09150424,\"PricePerUnit\":0.00366016000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:13:36.31\"},{\"OrderUuid\":\"3b60e7f5-2082-447b-9c5e-9f402a06332c\",\"Exchange\":\"BTC-BAT\",\"TimeStamp\":\"2017-08-28T01:12:56.447\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00004913,\"Quantity\":1800.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00022108,\"Price\":0.08843400,\"PricePerUnit\":0.00004913000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:12:56.51\"},{\"OrderUuid\":\"e3419c0f-e252-4b61-a92b-398032ced5f3\",\"Exchange\":\"BTC-XRP\",\"TimeStamp\":\"2017-08-28T01:07:16.203\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00004618,\"Quantity\":1700.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00019626,\"Price\":0.07850600,\"PricePerUnit\":0.00004618000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:07:16.423\"},{\"OrderUuid\":\"62dac844-8f5f-48f6-8b47-4384cf584ac8\",\"Exchange\":\"BTC-LTC\",\"TimeStamp\":\"2017-08-28T01:03:58.97\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.01427100,\"Quantity\":50.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00178462,\"Price\":0.71385946,\"PricePerUnit\":0.01427718000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-28T01:03:59.267\"},{\"OrderUuid\":\"2a3b4183-8a3e-4b90-94eb-d9d74104b199\",\"Exchange\":\"BTC-VTC\",\"TimeStamp\":\"2017-08-27T05:47:09.587\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00021552,\"Quantity\":1000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00053879,\"Price\":0.21551998,\"PricePerUnit\":0.00021551000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-27T05:48:06.34\"},{\"OrderUuid\":\"21a80737-ee52-4175-8184-5eb8260bd977\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-27T00:21:11.477\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.00384073,\"Quantity\":30.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00028875,\"Price\":0.11550000,\"PricePerUnit\":0.00385000000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-27T00:21:11.65\"},{\"OrderUuid\":\"708f5cfe-94ab-4936-88a9-0a9e82619b86\",\"Exchange\":\"BTC-MCO\",\"TimeStamp\":\"2017-08-26T01:30:37.887\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00243500,\"Quantity\":30.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00018262,\"Price\":0.07304999,\"PricePerUnit\":0.00243499000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-26T01:46:34.627\"},{\"OrderUuid\":\"9cd60d42-e432-46b8-ba20-fbfc710f95ce\",\"Exchange\":\"BTC-BCC\",\"TimeStamp\":\"2017-08-19T16:22:05.547\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.20500000,\"Quantity\":2.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00102498,\"Price\":0.40999999,\"PricePerUnit\":0.20499999000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-19T16:22:06.797\"},{\"OrderUuid\":\"63297fe5-d07b-40ce-97a2-f960c0b107d3\",\"Exchange\":\"BTC-BCC\",\"TimeStamp\":\"2017-08-18T01:57:00.053\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":0.11124700,\"Quantity\":3.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00083435,\"Price\":0.33374100,\"PricePerUnit\":0.11124700000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-18T01:57:00.837\"},{\"OrderUuid\":\"bf1b3b89-2fce-49c0-b353-0c89bea18c24\",\"Exchange\":\"BTC-VTC\",\"TimeStamp\":\"2017-08-16T22:57:19.757\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00010100,\"Quantity\":1000.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00025249,\"Price\":0.10100000,\"PricePerUnit\":0.00010100000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-17T00:05:27.51\"},{\"OrderUuid\":\"c6b11b9b-495e-4ac8-8a94-5b73d6e233bc\",\"Exchange\":\"BTC-FCT\",\"TimeStamp\":\"2017-08-16T21:54:31.163\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.00435940,\"Quantity\":13.44236162,\"QuantityRemaining\":0.00000000,\"Commission\":0.00014650,\"Price\":0.05860063,\"PricePerUnit\":0.00435939000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-16T21:57:45.983\"},{\"OrderUuid\":\"daab0461-8061-43e1-af48-92dcf38bda3f\",\"Exchange\":\"USDT-BTC\",\"TimeStamp\":\"2017-08-16T21:46:22.13\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":4357.00000000,\"Quantity\":0.16000000,\"QuantityRemaining\":0.00000000,\"Commission\":1.74280000,\"Price\":697.12000000,\"PricePerUnit\":4357.00000000000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-16T21:46:40.347\"},{\"OrderUuid\":\"ed50ef1f-79a4-4474-966d-756f668164cd\",\"Exchange\":\"USDT-BTC\",\"TimeStamp\":\"2017-08-08T17:23:48.917\",\"OrderType\":\"LIMIT_SELL\",\"Limit\":3363.00000001,\"Quantity\":0.30055068,\"QuantityRemaining\":0.00000000,\"Commission\":2.52687984,\"Price\":1010.75193684,\"PricePerUnit\":3363.00000000000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-08T17:23:50.433\"},{\"OrderUuid\":\"e7d95fcb-f0d4-433a-80d2-823c24a00e93\",\"Exchange\":\"BTC-GBYTE\",\"TimeStamp\":\"2017-08-07T18:33:41.96\",\"OrderType\":\"LIMIT_BUY\",\"Limit\":0.12000011,\"Quantity\":1.00000000,\"QuantityRemaining\":0.00000000,\"Commission\":0.00028301,\"Price\":0.11320604,\"PricePerUnit\":0.11320604000000000000,\"IsConditional\":false,\"Condition\":\"NONE\",\"ConditionTarget\":null,\"ImmediateOrCancel\":false,\"Closed\":\"2017-08-07T18:33:42.083\"}]}";

			API_ImportResult result = JsonConvert.Deserialize<API_ImportResult>(json);

			Assert.NotNull(result);

			Assert.True(result.result.Count > 0);
		}
	}

	public class InvalidJsonStringClass
	{
		public string ScreenId { get; set; }
		public string StepType { get; set; }
		public string Text { get; set; }
		public string Title { get; set; }
	}

	public class API_ImportResult
	{
		public class CoinResults
		{
			public string OrderUuid { get; set; }

			public string Exchange { get; set; }

			public string TimeStamp { get; set; }

			public string OrderType { get; set; }

			public string Limit { get; set; }

			public string Quantity { get; set; }

			public string QuantityRemaining { get; set; }

			public string Commission { get; set; }

			public string Price { get; set; }

			public string PricePerUnit { get; set; }

			public bool IsConditional { get; set; }

			public string Condition { get; set; }

			public bool ImmediateOrCancel { get; set; }

			public string Closed { get; set; }
		}

		public bool success { get; set; }

		public string message { get; set; }

		public ICollection<CoinResults> result { get; set; }
	}
}
