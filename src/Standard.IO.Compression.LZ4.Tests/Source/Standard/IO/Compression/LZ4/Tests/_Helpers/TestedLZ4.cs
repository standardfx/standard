using System.IO;
using Standard.IO.Compression.LZ4Encoding;

namespace Standard.IO.Compression.LZ4.Tests
{
	public class LZ4Settings
	{
		public LZ4CompressionLevel Level { get; set; } = LZ4CompressionLevel.Level0;
		public int BlockSize { get; set; } = LZ4MemoryHelper.K64;
		public int ExtraBlocks { get; set; } = 0;
		public bool Chaining { get; set; } = true;
	}

	public class TestedLZ4
	{
		public static void Decode(string encoded, string decoded, int chunkSize)
		{
			using (var input = File.OpenRead(encoded))
			using (var output = File.Create(decoded))
			using (var decode = new LZ4DecoderStream(input, i => new LZ4ChainDecoder(i.BlockSize, 0)))
			{
				var buffer = new byte[chunkSize];
				while (true)
				{
					var read = decode.Read(buffer, 0, buffer.Length);
					if (read == 0)
						break;

					output.Write(buffer, 0, read);
				}
			}
		}

		public static void Encode(string original, string encoded, int chuckSize, LZ4Settings settings)
		{
			var frameInfo = new LZ4FrameDescriptor(null, false, settings.Chaining, false, null, settings.BlockSize);
			using (var input = File.OpenRead(original))
			using (var output = File.Create(encoded))
			using (var encode = new LZ4EncoderStream(output, frameInfo, 
                i => LZ4EncodingFactory.CreateEncoder(i.Chaining, settings.Level, i.BlockSize, settings.ExtraBlocks)))
			{
				var buffer = new byte[chuckSize];
				while (true)
				{
					var read = input.Read(buffer, 0, buffer.Length);
					if (read == 0)
						break;

					encode.Write(buffer, 0, read);
				}
			}
		}
	}
}
