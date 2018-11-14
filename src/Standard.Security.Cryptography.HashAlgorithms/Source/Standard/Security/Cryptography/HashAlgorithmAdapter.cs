using System;
using System.Security.Cryptography;

namespace Standard.Security.Cryptography
{
    /// <summary>
    /// An adapter that implements <see cref="HashAlgorithm"/>.
    /// </summary>
    public class HashAlgorithmAdapter : HashAlgorithm
    {
        private readonly Action _reset;
        private readonly Action<byte[], int, int> _update;
        private readonly Func<byte[]> _digest;

        /// <summary>
        /// Creates a new instance of the <see cref="HashAlgorithmAdapter"/> class.
        /// </summary>
        /// <param name="hashSize">Hash size (in bytes)</param>
        /// <param name="reset">Reset function.</param>
        /// <param name="update">Update function.</param>
        /// <param name="digest">Digest function.</param>
        public HashAlgorithmAdapter(int hashSize, Action reset, Action<byte[], int, int> update, Func<byte[]> digest)
        {
            _reset = reset;
            _update = update;
            _digest = digest;
            HashSize = hashSize;
        }

        /// <see cref="HashAlgorithm.HashSize"/>
        public override int HashSize { get; }

#if NETSTANDARD1_3
        /// <summary>
        /// Returns the current hash value.
        /// </summary>
        public byte[] Hash
        {
            get { return _digest(); }
        }
#else
        /// <see cref="HashAlgorithm.Hash"/>
        public override byte[] Hash
        {
            get { return _digest(); }
        }
#endif

        /// <see cref="HashAlgorithm.HashCore(byte[], int, int)"/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _update(array, ibStart, cbSize);
        }

        /// <see cref="HashAlgorithm.HashFinal()"/>
        protected override byte[] HashFinal()
        {
            return _digest();
        }

        /// <see cref="HashAlgorithm.Initialize()"/>
        public override void Initialize()
        {
            _reset();
        }
    }
}