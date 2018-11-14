using System;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// LZ4 decoder used with independent blocks mode. This decoder will fail
    /// if input data has been compressed with chained blocks
    /// (<see cref="LZ4FastChainEncoder"/> and <see cref="LZ4HighChainEncoder"/>)
    /// </summary>
    internal unsafe class LZ4BlockDecoder : UnmanagedEncodingResource, ILZ4Decoder
	{
		private readonly int _blockSize;
		private readonly int _outputLength;
		private int _outputIndex;
		private readonly byte* _outputBuffer;

        /// <summary>
        /// Creates new instance of the <see cref="LZ4BlockDecoder"/> class.
        /// </summary>
        /// <param name="blockSize">Block size. Must be equal or greater to one used for compression.</param>
        public LZ4BlockDecoder(int blockSize)
        {
            blockSize = LZ4MemoryHelper.RoundUp(Math.Max(blockSize, LZ4MemoryHelper.K1), LZ4MemoryHelper.K1);
            _blockSize = blockSize;
            _outputLength = _blockSize + 8;
            _outputIndex = 0;
            _outputBuffer = (byte*)LZ4MemoryHelper.Alloc(_outputLength + 8);
        }

        /// <see cref="ILZ4Decoder.BlockSize"/>
        public int BlockSize
        {
            get { return _blockSize; }
        }

		/// <see cref="ILZ4Decoder.BytesReady"/>
		public int BytesReady
        {
            get { return _outputIndex; }
        }

		/// <see cref="ILZ4Decoder.Decode(byte*, int, int)"/>
		public int Decode(byte* source, int length, int blockSize = 0)
		{
			ThrowIfDisposed();
			
			if (blockSize <= 0)
				blockSize = _blockSize;

			if (blockSize > _blockSize)
				throw new InvalidOperationException();

			int decoded = LZ4Codec.Decode(source, length, _outputBuffer, _outputLength);
			if (decoded < 0)
				throw new InvalidOperationException();

			_outputIndex = decoded;
			return _outputIndex;
		}

		/// <see cref="ILZ4Decoder.Inject(byte*, int)"/>
		public int Inject(byte* source, int length)
		{
			ThrowIfDisposed();
			
			if (length <= 0)
				return _outputIndex = 0;

			if (length > _outputLength)
				throw new InvalidOperationException();

			LZ4MemoryHelper.Move(_outputBuffer, source, length);
			_outputIndex = length;
			return length;
		}

		/// <see cref="ILZ4Decoder.Drain(byte*, int, int)"/>
		public void Drain(byte* target, int offset, int length)
		{
			ThrowIfDisposed();

			offset = _outputIndex + offset; // negative value!
			if (offset < 0 || length < 0 || offset + length > _outputIndex)
				throw new InvalidOperationException();

			LZ4MemoryHelper.Move(target, _outputBuffer + offset, length);
		}

		/// <see cref="UnmanagedEncodingResource.ReleaseUnmanaged()"/>
		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			LZ4MemoryHelper.Free(_outputBuffer);
		}
	}
}
