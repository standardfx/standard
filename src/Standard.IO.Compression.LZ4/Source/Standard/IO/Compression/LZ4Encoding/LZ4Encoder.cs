using System;
using Standard.IO.Compression.LZ4;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// Base class for LZ4 encoders. Provides basic functionality shared by various LZ4 
    /// encoder implements, such as <see cref="LZ4BlockEncoder"/>, <see cref="LZ4FastChainEncoder"/>,
    /// and <see cref="LZ4HighChainEncoder"/>. This class should not 
    /// be used directly.
    /// </summary>
    internal abstract unsafe class LZ4Encoder: UnmanagedEncodingResource, ILZ4Encoder
	{
		private readonly byte* _inputBuffer;
		private readonly int _inputLength;
		private readonly int _blockSize;

		private int _inputIndex;
		private int _inputPointer;

		/// <summary>
        /// Creates a new instance of the <see cref="LZ4Encoder"/> class.
        /// </summary>
		/// <param name="chaining">Needs to be `true` if using dependent blocks.</param>
		/// <param name="blockSize">Block size.</param>
		/// <param name="extraBlocks">Number of extra blocks.</param>
		protected LZ4Encoder(bool chaining, int blockSize, int extraBlocks)
		{
			blockSize = LZ4MemoryHelper.RoundUp(Math.Max(blockSize, LZ4MemoryHelper.K1), LZ4MemoryHelper.K1);
			extraBlocks = Math.Max(extraBlocks, 0);
			int dictSize = chaining ? LZ4MemoryHelper.K64 : 0;

			_blockSize = blockSize;
			_inputLength = dictSize + (1 + extraBlocks) * blockSize + 8;
			_inputIndex = _inputPointer = 0;
			_inputBuffer = (byte*) LZ4MemoryHelper.Alloc(_inputLength + 8);
		}

		/// <see cref="ILZ4Encoder.BlockSize"/>
		public int BlockSize
        {
            get { return _blockSize; }
        }

		/// <see cref="ILZ4Encoder.BytesReady"/>
		public int BytesReady
        {
            get { return _inputPointer - _inputIndex; }
        }

		/// <see cref="ILZ4Encoder.Topup(byte*, int)"/>
		public int Topup(byte* source, int length)
		{
			ThrowIfDisposed();

			if (length == 0)
				return 0;

			int spaceLeft = _inputIndex + _blockSize - _inputPointer;
			if (spaceLeft <= 0)
				return 0;

			int chunk = Math.Min(spaceLeft, length);
			LZ4MemoryHelper.Move(_inputBuffer + _inputPointer, source, chunk);
			_inputPointer += chunk;

			return chunk;
		}

		/// <see cref="ILZ4Encoder.Encode(byte*, int, bool)"/>
		public int Encode(byte* target, int length, bool allowCopy)
		{
			ThrowIfDisposed();

			int sourceLength = _inputPointer - _inputIndex;
			if (sourceLength <= 0)
				return 0;

			int encoded = EncodeBlock(_inputBuffer + _inputIndex, sourceLength, target, length);

			if (encoded <= 0)
				throw new InvalidOperationException(RS.EncodeChunkTargetBufferTooSmall);

			if (allowCopy && encoded >= sourceLength)
			{
				LZ4MemoryHelper.Move(target, _inputBuffer + _inputIndex, sourceLength);
				encoded = -sourceLength;
			}

			Commit();

			return encoded;
		}

		private void Commit()
		{
			_inputIndex = _inputPointer;
			if (_inputIndex + _blockSize <= _inputLength)
				return;

			_inputIndex = _inputPointer = CopyDict(_inputBuffer, _inputPointer);
		}

		/// <summary>
        /// Encodes single block using the appropriate algorithm.
        /// </summary>
		/// <param name="source">Source buffer.</param>
		/// <param name="sourceLength">Source buffer length.</param>
		/// <param name="target">Target buffer.</param>
		/// <param name="targetLength">Target buffer length.</param>
		/// <returns>
        /// Number of bytes actually written to target buffer.
        /// </returns>
		protected abstract int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength);

		/// <summary>
        /// Copies the current dictionary.
        /// </summary>
		/// <param name="target">Target buffer.</param>
		/// <param name="dictionaryLength">Dictionary length.</param>
		/// <returns>
        /// The length of the dictionary.
        /// </returns>
		protected abstract int CopyDict(byte* target, int dictionaryLength);

		/// <see cref="UnmanagedEncodingResource.ReleaseUnmanaged()"/>
		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			LZ4MemoryHelper.Free(_inputBuffer);
		}
	}
}
