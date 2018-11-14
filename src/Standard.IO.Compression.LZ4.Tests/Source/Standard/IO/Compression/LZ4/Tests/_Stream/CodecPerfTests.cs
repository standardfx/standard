using System;
using System.IO;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Standard.IO.Compression.LZ4.Tests
{
	public class CodecPerformanceTests
	{
        private readonly ITestOutputHelper output;

		public CodecPerformanceTests(ITestOutputHelper output)
        {
        	this.output = output;
        }

        #if DEBUG
        [Theory(Skip = "Long running tests are skipped in debug mode")]
#else
		[Theory]
#endif
		// Fast Chaining - 64k, 256k, 1m, 4m
        [InlineData("-1 -BD -B4 -BX", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B5", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B6 -BX", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B7", LZ4MemoryHelper.K4)]
		// Max Chaining - 64k, 256k, 1m, 4m
		[InlineData("-9 -BD -B4", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B5 -BX", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B6", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B7 -BX", LZ4MemoryHelper.K4)]
		// Fast
		[InlineData("-1 -B4", LZ4MemoryHelper.K4)]
		[InlineData("-1 -B7", LZ4MemoryHelper.K4)]
		// Misc
		[InlineData("-9 -B7 -BX", LZ4MemoryHelper.K4)]
		[InlineData("-1 -B4 -BD", LZ4MemoryHelper.M1)]
		[InlineData("-9 -B4 -BD", 1337)]
		public void WholeCorpus(string options, int chunkSize)
		{
			for (int i = 0; i < Tools.CorpusNames.Length; i++)
			{
				string filename = Tools.CorpusNames[i];
				string fileContent = Tools.CorpusContents[i];

				try
				{
					TestEncoding($"corpus/{filename}", chunkSize, options, fileContent);
				}
				catch (Exception e)
				{
					throw new Exception($"Failed to process: {filename} @ {options}/{chunkSize}", e);
				}
			}
		}

		private void TestEncoding(string original, int chunkSize, string options, string contentType = "??")
		{
			original = Tools.FindFile(original);
			var encoded = Path.GetTempFileName();
			var decoded = Path.GetTempFileName();

			LZ4Settings settings = Tools.ParseSettings(options);

			try
			{
				var stopwatch = Stopwatch.StartNew();
				TestedLZ4.Encode(original, encoded, chunkSize, settings);
	            stopwatch.Stop();
	            var testedEncodeTime = stopwatch.Elapsed;

	            stopwatch.Restart();
				TestedLZ4.Decode(encoded, decoded, chunkSize);
				stopwatch.Stop();
				var testedDecodeTime = stopwatch.Elapsed;

				long testedEncodeSize = new FileInfo(encoded).Length;
				long testedOriginalSize = new FileInfo(original).Length;
				double testedCompress = (double)testedEncodeSize / (double)testedOriginalSize * 100;

	            stopwatch.Restart();
				ReferenceLZ4.Encode(options, original, encoded);
				stopwatch.Stop();
				var refEncodeTime = stopwatch.Elapsed;

				stopwatch.Restart();
				ReferenceLZ4.Decode(encoded, decoded);
				stopwatch.Stop();
				var refDecodeTime = stopwatch.Elapsed;

	            string encoderWinner = null;
	            if (testedEncodeTime > refEncodeTime)
	            	encoderWinner = string.Format("Native wins by {0}", (testedEncodeTime - refEncodeTime));
	            else if (refEncodeTime > testedEncodeTime)
	            	encoderWinner = string.Format("Implement wins by {0}", (refEncodeTime - testedEncodeTime));
	            else
	            	encoderWinner = "Tied!";

	            string decoderWinner = null;
	            if (testedDecodeTime > refDecodeTime)
	            	decoderWinner = string.Format("Native wins by {0}", (testedDecodeTime - refDecodeTime));
	            else if (refDecodeTime > testedDecodeTime)
	            	decoderWinner = string.Format("Implement wins by {0}", (refDecodeTime - testedDecodeTime));
	            else
	            	decoderWinner = "Tied!";

	            string settingsFormatted = string.Format("Level = {0}, BlockSize = {1}, Chaining = {2}", 
	            	settings.Level, 
	            	settings.BlockSize, 
	            	settings.Chaining);

	            string report = string.Format("Encoding payload '{0}' ({1}) implement vs native performance results: \n", original, contentType);
	            report += string.Format("*** {0} ***\n", settingsFormatted);
	            report += string.Format("- Encoder: {0} vs {1}\n", testedEncodeTime, refEncodeTime);
	            report += "  => " + encoderWinner + "\n";
	            report += string.Format("- Decoder: {0} vs {1}\n", testedDecodeTime, refDecodeTime);
	            report += "  => " + decoderWinner + "\n";
	            report += string.Format("- Reduced size from {0} to {1} => {2}% of original size\n", testedOriginalSize, testedEncodeSize, testedCompress);

	            output.WriteLine("[TOUT/PERF] " + report);
			}
			finally
			{
				File.Delete(encoded);
				File.Delete(decoded);
			}
		}
    }
}
