using System;
// fast decoder context
using LZ4Context = Standard.IO.Compression.LZ4Engine.StreamDecodeT;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// LZ4 decoder for handling dependent blocks.
    /// </summary>
    internal unsafe class LZ4ChainDecoder: UnmanagedEncodingResource, ILZ4Decoder
	{
		private readonly LZ4Context* _context;
		private readonly int _blockSize;
		private readonly byte* _outputBuffer;
		private readonly int _outputLength;
		private int _outputIndex;

		/// <summary>
        /// Creates new instance of the <see cref="LZ4ChainDecoder"/> class.
        /// </summary>
		/// <param name="blockSize">Block size.</param>
		/// <param name="extraBlocks">Number of extra blocks.</param>
		public LZ4ChainDecoder(int blockSize, int extraBlocks)
		{
			blockSize = LZ4MemoryHelper.RoundUp(Math.Max(blockSize, LZ4MemoryHelper.K1), LZ4MemoryHelper.K1);
			extraBlocks = Math.Max(extraBlocks, 0);

			_blockSize = blockSize;
			_outputLength = LZ4MemoryHelper.K64 + (1 + extraBlocks) * _blockSize + 8;
			_outputIndex = 0;
			_outputBuffer = (byte*) LZ4MemoryHelper.Alloc(_outputLength + 8);
			_context = (LZ4Context*) LZ4MemoryHelper.AllocZero(sizeof(LZ4Context));
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
		public int Decode(byte* source, int length, int blockSize)
		{
			if (blockSize <= 0)
				blockSize = _blockSize;

			Prepare(blockSize);

			int decoded = DecodeBlock(source, length, _outputBuffer + _outputIndex, blockSize);

			if (decoded < 0)
				throw new InvalidOperationException();

			_outputIndex += decoded;

			return decoded;
		}

		/// <see cref="ILZ4Decoder.Inject(byte*, int)"/>
		public int Inject(byte* source, int length)
		{
			if (length <= 0)
				return 0;

			if (length > Math.Max(_blockSize, LZ4MemoryHelper.K64))
				throw new InvalidOperationException();

			if (_outputIndex + length < _outputLength)
			{
				LZ4MemoryHelper.Move(_outputBuffer + _outputIndex, source, length);
				_outputIndex = ApplyDict(_outputIndex + length);
			} 
			else if (length >= LZ4MemoryHelper.K64)
			{
				LZ4MemoryHelper.Move(_outputBuffer, source, length);
				_outputIndex = ApplyDict(length);
			}
			else
			{
				int tailSize = Math.Min(LZ4MemoryHelper.K64 - length, _outputIndex);
				LZ4MemoryHelper.Move(_outputBuffer, _outputBuffer + _outputIndex - tailSize, tailSize);
				LZ4MemoryHelper.Move(_outputBuffer + tailSize, source, length);
				_outputIndex = ApplyDict(tailSize + length);
			}

			return length;
		}

		/// <see cref="ILZ4Decoder.Drain(byte*, int, int)"/>
		public void Drain(byte* target, int offset, int length)
		{
			offset = _outputIndex + offset; // negative value!

			// #todo more helpful error required!
			if (offset < 0 || length < 0 || offset + length > _outputIndex)
				throw new InvalidOperationException();

			LZ4MemoryHelper.Move(target, _outputBuffer + offset, length);
		}

		private void Prepare(int blockSize)
		{
			if (_outputIndex + blockSize <= _outputLength)
				return;

			_outputIndex = CopyDict(_outputIndex);
		}

		private int CopyDict(int index)
		{
			int dictStart = Math.Max(index - LZ4MemoryHelper.K64, 0);
			int dictSize = index - dictStart;
			LZ4MemoryHelper.Move(_outputBuffer, _outputBuffer + dictStart, dictSize);
			LZ4Engine.SetStreamDecode(_context, _outputBuffer, dictSize);
			return dictSize;
		}

		private int ApplyDict(int index)
		{ 
			int dictStart = Math.Max(index - LZ4MemoryHelper.K64, 0);
			int dictSize = index - dictStart;
			LZ4Engine.SetStreamDecode(_context, _outputBuffer + dictStart, dictSize);
			return index;
		}

		private int DecodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
        {
            return LZ4Engine.DecompressSafeContinue(_context, source, target, sourceLength, targetLength);
        }

        /// <see cref="UnmanagedEncodingResource.ReleaseUnmanaged()"/>
        protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			LZ4MemoryHelper.Free(_context);
			LZ4MemoryHelper.Free(_outputBuffer);
		}
	}
}
