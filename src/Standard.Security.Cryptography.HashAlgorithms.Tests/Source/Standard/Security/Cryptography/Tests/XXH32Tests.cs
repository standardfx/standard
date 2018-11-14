using System;
using System.Security.Cryptography;
using Xunit;
using Xunit.Abstractions;
using _XXH = System.Data.HashFunction.xxHash.xxHashFactory;

namespace Standard.Security.Cryptography.Tests
{
    public class XXH32Tests
    {
        private readonly ITestOutputHelper _output;

        public XXH32Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("")]
        [InlineData("quick brown fox jumped over the lazy dog")]
        [InlineData("thirteenchars")]
        public void SingleBlockXxh32MatchesTheirs(string text)
        {
            var input = System.Text.Encoding.UTF8.GetBytes(text);
            var expected = Theirs32(input);
            var actual = XXHash32.DigestOf(input, 0, input.Length);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("")]
        [InlineData("quick brown fox jumped over the lazy dog")]
        [InlineData("thirteenchars")]
        public void SingleBlockXxh32UsingSpanMatchesTheirs(string text)
        {
            var input = System.Text.Encoding.UTF8.GetBytes(text);
            var expected = Theirs32(input);
            var actual = XXHash32.DigestOf(input.AsSpan());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public unsafe void EmptyHash()
        {
            var input = Array.Empty<byte>();

            var expected = Theirs32(input);

            var actual1 = XXHash32.DigestOf(input, 0, input.Length);
            Assert.Equal(expected, actual1);

            fixed (byte* inputPtr = input)
            {
                var actual2 = XXHash32.DigestOf(inputPtr, 0);
                Assert.Equal(expected, actual2);
            }

            var actual3 = XXHash32.EmptyHash;
            Assert.Equal(expected, actual3);
        }

        [Theory]
        [InlineData(0, 10, 3)]
        [InlineData(1, 100, 33)]
        [InlineData(2, 100, 5)]
        [InlineData(3, 100, 13)]
        [InlineData(4, 100, 1000)]
        public void RestartableHashReturnsSameResultsAsSingleBlock(int seed, int length, int chunk)
        {
            var random = new Random(seed);
            var bytes = new byte[length];
            random.NextBytes(bytes);

            var expected = XXHash32.DigestOf(bytes, 0, bytes.Length);

            var transform = new XXHash32();
            var index = 0;
            while (index < length)
            {
                var l = Math.Min(chunk, length - index);
                transform.Update(bytes, index, l);
                index += l;
            }

            var actual = transform.Digest();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(1, 200)]
        [InlineData(2, 300)]
        public void EveryCallToDigestReturnsSameHash(int seed, int length)
        {
            var random = new Random(seed);
            var bytes = new byte[length];
            random.NextBytes(bytes);

            var expected = XXHash32.DigestOf(bytes, 0, bytes.Length);

            var transform = new XXHash32();
            transform.Update(bytes, 0, length);

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(expected, transform.Digest());
            }
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(1, 200)]
        [InlineData(2, 300)]
        public void HashAlgorithmWrapperReturnsSameResults(int seed, int length)
        {
            var bytes = new byte[length];
            new Random(seed).NextBytes(bytes);

            var expected = XXHash32.DigestOf(bytes, 0, bytes.Length);
            //var actual = new XXHash32().AsHashAlgorithm().ComputeHash(bytes);
            var actual = ((HashAlgorithm)new XXHash32()).ComputeHash(bytes);

            Assert.Equal(expected, BitConverter.ToUInt32(actual, 0));
        }

        private static uint Theirs32(byte[] bytes)
        {
            var xxHashConfig = new System.Data.HashFunction.xxHash.xxHashConfig
            {
                    HashSizeInBits = sizeof(uint) * 8
                };
            var algorithm = _XXH.Instance.Create(xxHashConfig);
            return BitConverter.ToUInt32(algorithm.ComputeHash(bytes).Hash, 0);
        }
    }
}