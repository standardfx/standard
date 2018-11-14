using System;
using System.IO;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Standard.IO.Compression.LZ4.Tests
{
	public class EncoderTests
	{
		[Theory]
		[InlineData("reymont", "-1 -BD -B4", 1337)]
		[InlineData("reymont", "-9 -BD -B7", 1337)]
		[InlineData("x-ray", "-1 -BD -B4", 1337)]
		[InlineData("x-ray", "-9 -BD -B7", 1337)]
		public void OddChunkSize(string filename, string options, int chunkSize)
		{
			TestEncoder($"corpus/{filename}", chunkSize, Tools.ParseSettings(options));
		}

		[Theory]
		[InlineData("reymont", "-1 -BD -B4", LZ4MemoryHelper.M1)]
		[InlineData("x-ray", "-9 -BD -B4", LZ4MemoryHelper.M1)]
		public void LargeChunkSize(string filename, string options, int chunkSize)
		{
			TestEncoder($"corpus/{filename}", chunkSize, Tools.ParseSettings(options));
		}

		[Theory]
		[InlineData("-1 -BD -B4 -BX", LZ4MemoryHelper.K64)]
		[InlineData("-1 -BD -B4 -BX", 1337)]
		[InlineData("-1 -BD -B4 -BX", LZ4MemoryHelper.K64 + 1337)]
		[InlineData("-9 -BD -B7", LZ4MemoryHelper.K64)]
		[InlineData("-9 -BD -B7", 1337)]
		[InlineData("-9 -BD -B7", LZ4MemoryHelper.K64 + 1337)]
		public void HighEntropyData(string options, int chunkSize)
		{
			var filename = Path.GetTempFileName();
			try
			{
				Tools.WriteRandom(filename, 10 * LZ4MemoryHelper.M1 + 1337);
				TestEncoder(filename, chunkSize, Tools.ParseSettings(options));
			}
			finally
			{
				File.Delete(filename);
			}
		}

#if DEBUG
        [Theory(Skip = "Long running tests are skipped in debug mode")]
#else
		[Theory]
#endif
        [InlineData("-1 -BD -B4 -BX", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B5", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B6 -BX", LZ4MemoryHelper.K8)]
		[InlineData("-1 -BD -B7", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B4", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B5 -BX", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B6", LZ4MemoryHelper.K4)]
		[InlineData("-9 -BD -B7 -BX", LZ4MemoryHelper.K4)]
		[InlineData("-1 -B4", LZ4MemoryHelper.K4)]
		[InlineData("-1 -B7", LZ4MemoryHelper.K4)]
		[InlineData("-9 -B7 -BX", LZ4MemoryHelper.K4)]
		[InlineData("-1 -B4 -BD", LZ4MemoryHelper.M1)]
		[InlineData("-9 -B4 -BD", 1337)]
		public void WholeCorpus(string options, int chunkSize)
		{
			var settings = Tools.ParseSettings(options);
			foreach (var filename in Tools.CorpusNames)
			{
				try
				{
					TestEncoder($"corpus/{filename}", chunkSize, settings);
				}
				catch (Exception e)
				{
					throw new Exception(
						$"Failed to process: {filename} @ {options}/{chunkSize}", e);
				}
			}
		}

		private static void TestEncoder(string original, int chunkSize, LZ4Settings settings)
		{
			original = Tools.FindFile(original);
			var encoded = Path.GetTempFileName();
			var decoded = Path.GetTempFileName();
			try
			{
				TestedLZ4.Encode(original, encoded, chunkSize, settings);
				ReferenceLZ4.Decode(encoded, decoded);

				Tools.SameFiles(original, decoded);
			}
			finally
			{
				File.Delete(encoded);
				File.Delete(decoded);
			}
		}
	}
}
