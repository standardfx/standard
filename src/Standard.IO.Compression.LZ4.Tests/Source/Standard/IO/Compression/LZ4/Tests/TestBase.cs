using System;
using System.Diagnostics;
using System.Threading;
using Xunit.Abstractions;

namespace Standard.IO.Compression.LZ4.Tests
{
	public class TestBase
	{
		private readonly ITestOutputHelper _output;

		public TestBase(ITestOutputHelper output) => _output = output;

		private void WriteLine(string text) => _output.WriteLine(text);

		protected void Measure(string name, int count, Action action)
		{
			action();
			Thread.Sleep(200);
			var stopwatch = Stopwatch.StartNew();
			for (var i = 0; i < count; i++)
            {
                action();
            }
            stopwatch.Stop();
			WriteLine($"{name}: {stopwatch.Elapsed.TotalMilliseconds / count:0.0000}ms");
		}
	}
}
