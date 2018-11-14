using System;
using System.IO;
using Xunit;

namespace Standard.IO.Compression.LZ4.Tests
{
	partial class Tools
	{
		public static readonly string[] CorpusNames = 
            {
			    "dickens", "mozilla", "mr", "nci",
			    "ooffice", "osdb", "reymont", "samba",
			    "sao", "webster", "x-ray", "xml"
		    };

		public static readonly string[] CorpusContents = 
			{
				"text", "exe", "picture", "database",
				"exe", "database", "pdf", "src",
				"html", "html", "picture", "html"
			};

		public static void SameFiles(string original, string decoded)
		{
			using (var streamA = File.OpenRead(original))
			using (var streamB = File.OpenRead(decoded))
			{
				Assert.Equal(streamA.Length, streamB.Length);
				var bufferA = new byte[4096];
				var bufferB = new byte[4096];

				while (true)
				{
					var readA = streamA.Read(bufferA, 0, bufferA.Length);
					var readB = streamB.Read(bufferB, 0, bufferB.Length);
					Assert.Equal(readA, readB);
					if (readA == 0)
						break;

					SameBytes(bufferA, bufferB, readA);
				}
			}
		}

		public static LZ4Settings ParseSettings(string options)
		{
			var result = new LZ4Settings { Chaining = false };

			foreach (var option in options.Split(' '))
			{
				switch (option)
				{
					case "-1":
						result.Level = LZ4CompressionLevel.Level0;
						break;
					case "-9":
						result.Level = LZ4CompressionLevel.Level9;
						break;
					case "-11":
						result.Level = LZ4CompressionLevel.Level11;
						break;
					case "-12":
						result.Level = LZ4CompressionLevel.Level12;
						break;
					case "-BD":
						result.Chaining = true;
						break;
					case "-BX":
						// ignored to be implemented
						break;
					case "-B4":
						result.BlockSize = LZ4MemoryHelper.K64;
						break;
					case "-B5":
						result.BlockSize = LZ4MemoryHelper.K256;
						break;
					case "-B6":
						result.BlockSize = LZ4MemoryHelper.M1;
						break;
					case "-B7":
						result.BlockSize = LZ4MemoryHelper.M4;
						break;
					default:
						throw new NotImplementedException($"Option '{option}' not recognized");
				}
			}

			return result;
		}

		public static void WriteRandom(string filename, int length, int seed = 0)
		{
			var random = new Random(seed);
			var buffer = new byte[0x10000];
			using (var file = File.Create(filename))
			{
				while (length > 0)
				{
					random.NextBytes(buffer);
					var chunkSize = Math.Min(length, buffer.Length);

					file.Write(buffer, 0, chunkSize);
					length -= chunkSize;
				}
			}
		}
	}
}
