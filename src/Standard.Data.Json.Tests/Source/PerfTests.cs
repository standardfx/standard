using System;
using System.IO;
using System.Diagnostics;
using Standard;
using Xunit;
using Xunit.Abstractions;

namespace Standard.Data.Json.Tests
{
	public class PerfTests
	{
		private readonly ITestOutputHelper output;

		public PerfTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void DeserializeFatherToDynamic()
		{
			var sw = new Stopwatch();
			sw.Restart();
            using (TextReader reader = new StringReader(TestHelper.GetEmbedFileContent("father.json")))
            {
                JsonConvert.Deserialize<dynamic>(reader);
            }
            sw.Stop();

			output.WriteLine("[UTOUT] Perf: {0}", sw.Elapsed);
			Assert.True(true);
		}
	}
}
