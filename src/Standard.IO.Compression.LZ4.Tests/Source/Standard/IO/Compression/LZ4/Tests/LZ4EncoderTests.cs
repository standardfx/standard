using System;
using Standard.IO.Compression.LZ4Encoding;
using Xunit;
using Xunit.Abstractions;

namespace Standard.IO.Compression.LZ4.Tests
{
	public unsafe class LZ4EncoderTests
	{
		public LZ4EncoderTests(ITestOutputHelper output)
        {
        }

		[Theory]
		[InlineData(1024, 50, 0)]
		[InlineData(1024, 1024, 0)]
		[InlineData(1024, 1026, 0)]
		[InlineData(1024, 1100, 0)]
		[InlineData(1024, 0x10000, 0)]
		[InlineData(1024, 0x20000, 100)]
		public void SmallBlocksWithNoShift(int blockSize, int totalSize, int extraBlocks)
		{
			Assert.Equal(
				FastStreamManual(blockSize, totalSize),
				FastStreamEncoder(blockSize, totalSize, extraBlocks));
		}

		[Theory]
		[InlineData(0x10000, 50, 0)]
		[InlineData(0x10000, 0x10000, 0)]
		[InlineData(0x10000, 0x20000, 5)]
		[InlineData(0x10000, 0x50000, 5)]
		public void MediumBlocksWithNoShift(int blockSize, int totalSize, int extraBlocks)
		{
			Assert.Equal(
				FastStreamManual(blockSize, totalSize),
				FastStreamEncoder(blockSize, totalSize, extraBlocks));
		}

		[Theory]
		[InlineData(0x20000, 0x50000, 1)]
		[InlineData(0x20000, 0x100000, 1)]
		public void LargeBlocksWithDictShifting(int blockSize, int totalSize, int extraBlocks)
		{
			Assert.Equal(
				FastStreamManual(blockSize, totalSize),
				FastStreamEncoder(blockSize, totalSize, extraBlocks));
		}

		[Fact]
		public void CompressionRatio()
		{
			var input = new byte[0x10000];
			var output = new byte[0x10000];

			var encoded = LZ4Codec.Encode(
				input, 0, input.Length, 
				output, 0, output.Length, 
				LZ4CompressionLevel.Level12);

			Assert.True(encoded < input.Length / 200);
		}
		
		[Fact]
		public void HighEntropyRepeated()
		{
			var random = new Random(0);
			var encoder = new LZ4FastChainEncoder(256);
			var source = new byte[256];
			random.NextBytes(source);
			var target = new byte[1024];
			
			Assert.Equal(256, encoder.Topup(source, 0, 256));
			Assert.Equal(-256, encoder.Encode(target, 0, 1024, true));
			
			Assert.Equal(256, encoder.Topup(source, 0, 256));
			Assert.True(encoder.Encode(target, 0, 1024, true) < 32);
		}

		public uint FastStreamEncoder(int blockLength, int sourceLength, int extraBlocks = 0)
		{
			sourceLength = LZ4MemoryHelper.RoundUp(sourceLength, blockLength);
			var targetLength = 2 * sourceLength;

			var source = new byte[sourceLength];
			var target = new byte[targetLength];

			Lorem.Fill(source, 0, source.Length);

			using (var encoder = new LZ4FastChainEncoder(blockLength, extraBlocks))
			{
				var sourceP = 0;
				var targetP = 0;

				while (sourceP < sourceLength && targetP < targetLength)
				{
					encoder.TopupAndEncode(
						source, sourceP, Math.Min(blockLength, sourceLength - sourceP),
						target, targetP, targetLength - targetP,
						true, false,
						out var loaded,
						out var encoded);
					sourceP += loaded;
					targetP += encoded;
				}

				return Tools.Adler32(target, 0, targetP);
			}
		}

		public uint FastStreamManual(int blockLength, int sourceLength)
		{
			sourceLength = LZ4MemoryHelper.RoundUp(sourceLength, blockLength);
			var targetLength = 2 * sourceLength;

			var context = (LZ4Engine.StreamT*) LZ4MemoryHelper.AllocZero(sizeof(LZ4Engine.StreamT));
			var source = (byte*) LZ4MemoryHelper.Alloc(sourceLength);
			var target = (byte*) LZ4MemoryHelper.Alloc(targetLength);

			try
			{
				Lorem.Fill(source, sourceLength);

				var sourceP = 0;
				var targetP = 0;

				while (sourceP < sourceLength && targetP < targetLength)
				{
					targetP += LZ4Engine64.CompressFastContinue(
						context,
						source + sourceP,
						target + targetP,
						Math.Min(blockLength, sourceLength - sourceP),
						targetLength - targetP,
						1);
					sourceP += blockLength;
				}

				return Tools.Adler32(target, targetP);
			}
			finally
			{
				LZ4MemoryHelper.Free(context);
				LZ4MemoryHelper.Free(source);
				LZ4MemoryHelper.Free(target);
			}
		}
	}
}
