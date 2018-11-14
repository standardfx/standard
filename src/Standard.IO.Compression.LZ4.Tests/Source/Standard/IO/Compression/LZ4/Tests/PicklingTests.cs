using System;
using Xunit;
using Standard.IO.Compression;

namespace Standard.IO.Compression.LZ4.Tests
{
	public class PicklingTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(10)]
		[InlineData(32)]
		[InlineData(1337)]
		[InlineData(1337, LZ4CompressionLevel.Level9)]
		[InlineData(0x10000)]
		[InlineData(0x172a5, LZ4CompressionLevel.Level0)]
		[InlineData(0x172a5, LZ4CompressionLevel.Level9)]
		[InlineData(0x172a5, LZ4CompressionLevel.Level11)]
		[InlineData(0x172a5, LZ4CompressionLevel.Level12)]
		[InlineData(LZ4MemoryHelper.M4, LZ4CompressionLevel.Level12)]
		public void PickleLorem(int length, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
		{
			var original = new byte[length];
			Lorem.Fill(original, 0, length);

			var pickled = LZ4Codec.Compress(original, level);
			var unpickled = LZ4Codec.Expand(pickled);

			Tools.SameBytes(original, unpickled);
		}

		[Theory]
		[InlineData(1, 15)]
		[InlineData(2, 1024)]
		[InlineData(3, 1337, LZ4CompressionLevel.Level9)]
		[InlineData(3, 1337, LZ4CompressionLevel.Level12)]
		[InlineData(4, LZ4MemoryHelper.K64, LZ4CompressionLevel.Level12)]
		[InlineData(5, LZ4MemoryHelper.M4, LZ4CompressionLevel.Level12)]
		public void PickleEntropy(int seed, int length, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
		{
			var original = new byte[length];
			new Random(seed).NextBytes(original);

			var pickled = LZ4Codec.Compress(original, level);
			var unpickled = LZ4Codec.Expand(pickled);

			Tools.SameBytes(original, unpickled);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(0, 1337)]
		[InlineData(1337, 1337)]
		[InlineData(1337, 1)]
		[InlineData(1337, 0)]
		public void PicklingSpansGivesIdenticalResults(int offset, int length)
		{
			var source = new byte[offset + length + offset];
			Lorem.Fill(source, 0, source.Length);

			var array = LZ4Codec.Compress(source, offset, length);
			var span = LZ4Codec.Compress(source.AsSpan(offset, length));

			Assert.Equal(array, span);

			Assert.Equal(
                LZ4Codec.Expand(array),
                LZ4Codec.Expand(span.AsSpan()));
		}
	}
}
