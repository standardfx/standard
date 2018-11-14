using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Standard.Security.Cryptography;
using Standard.IO.Compression.LZ4Encoding;
using Standard.IO.Compression.LZ4;

namespace Standard.IO.Compression
{
    /// <summary>
    /// LZ4 decompression stream handling. Use an decoding method in <see cref="LZ4Stream"/> to create an instance of this class.
    /// </summary>
    public class LZ4DecoderStream : Stream, IDisposable
	{
		private readonly bool _interactive = true;
		private readonly bool _leaveOpen;

		private readonly Stream _inner;
		private readonly byte[] _buffer16 = new byte[16];
		private int _index16;

		private readonly Func<ILZ4FrameDescriptor, ILZ4Decoder> _decoderFactory;

		private ILZ4FrameDescriptor _frameInfo;
		private ILZ4Decoder _decoder;
		private int _decoded;
		private byte[] _buffer;

		private long _position;

        /// <summary>
        /// Creates a new instance of the <see cref="LZ4DecoderStream"/> class.
        /// </summary>
        /// <param name="inner">Inner stream.</param>
        /// <param name="decoderFactory">A function to return the appropriate encoder according to the frame descriptor.</param>
		/// <param name="leaveOpen">Indicates whether <paramref name="inner"/> stream should be left open after disposing.</param>
        internal LZ4DecoderStream(Stream inner, Func<ILZ4FrameDescriptor, ILZ4Decoder> decoderFactory, bool leaveOpen = false)
		{
			_inner = inner;
			_decoderFactory = decoderFactory;
			_leaveOpen = leaveOpen;
			_position = 0;
		}

		/// <see cref="Stream.Flush()"/>
		public override void Flush()
        {
            _inner.Flush();
        }

        /// <see cref="Stream.FlushAsync(CancellationToken)"/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        /// <see cref="Stream.Read(byte[], int, int)"/>
        public override int Read(byte[] buffer, int offset, int count)
		{
			EnsureFrame();

			int read = 0;
			while (count > 0)
			{
				if (_decoded <= 0 && (_decoded = ReadBlock()) == 0)
					break;

				if (ReadDecoded(buffer, ref offset, ref count, ref read))
					break;
			}

			return read;
		}

		/// <see cref="Stream.ReadByte()"/>
		public override int ReadByte()
        {
            return Read(_buffer16, _index16, 1) > 0 
                ? _buffer16[_index16] 
                : -1;
        }

        private void EnsureFrame()
		{
			if (_decoder == null)
				ReadFrame();
		}

		private void ReadFrame()
		{
			Read0();

			uint? magic = TryRead32();
			if (magic != 0x184D2204)
				throw new InvalidDataException(RS.ExpectLZ4MagicNumber);

            Read0();

			ushort flgBd = Read16();

			int flg = flgBd & 0xFF;
			int bd = (flgBd >> 8) & 0xFF;

			int version = (flg >> 6) & 0x11;

			if (version != 1)
				throw new InvalidDataException(string.Format(RS.LZ4VersionNotSupported, version)); 

			bool blockChaining = ((flg >> 5) & 0x01) == 0;
            bool blockChecksum = ((flg >> 4) & 0x01) != 0;
            bool hasContentSize = ((flg >> 3) & 0x01) != 0;
            bool contentChecksum = ((flg >> 2) & 0x01) != 0;
            bool hasDictionary = (flg & 0x01) != 0;
			int blockSizeCode = (bd >> 4) & 0x07;

			long? contentLength = hasContentSize ? (long?) Read64() : null;
			uint? dictionaryId = hasDictionary ? (uint?) Read32() : null;

			byte actualHC = (byte)(XXHash32.DigestOf(_buffer16, 0, _index16) >> 8);
			byte expectedHC = Read8();

			if (actualHC != expectedHC)
                throw new InvalidDataException(RS.BadLZ4FrameHeaderChecksum);

			int blockSize = MaxBlockSize(blockSizeCode);

			if (hasDictionary)
            {
                // Write32(dictionaryId);
                throw new NotImplementedException(string.Format(RS.FeatureNotImplementedInType, "Predefined Dictionaries", GetType().Name));
            }

			_frameInfo = new LZ4FrameDescriptor(contentLength, contentChecksum, blockChaining, blockChecksum, dictionaryId, blockSize);
			_decoder = _decoderFactory(_frameInfo);
			_buffer = new byte[blockSize];
		}

		private void CloseFrame()
		{
			if (_decoder == null)
				return;

			try
			{
				_frameInfo = null;
				_buffer = null;

				// if you need any exceptions throw them here

				_decoder.Dispose();
			}
			finally
			{
				_decoder = null;
			}
		}

		private static int MaxBlockSize(int blockSizeCode)
		{
			switch (blockSizeCode)
			{
				case 7: return LZ4MemoryHelper.M4;
				case 6: return LZ4MemoryHelper.M1;
				case 5: return LZ4MemoryHelper.K256;
				case 4: return LZ4MemoryHelper.K64;
				default: return LZ4MemoryHelper.K64;
			}
		}

		private unsafe int ReadBlock()
		{
			Read0();

			int blockLength = (int) Read32();
			if (blockLength == 0)
			{
				if (_frameInfo.ContentChecksum)
					Read32();
				CloseFrame();
				return 0;
			}

			bool uncompressed = (blockLength & 0x80000000) != 0;
			blockLength &= 0x7FFFFFFF;

			ReadN(_buffer, 0, blockLength);

			if (_frameInfo.BlockChecksum)
				Read32();

			fixed (byte* bufferPtr = _buffer)
            {
                return uncompressed
                    ? _decoder.Inject(bufferPtr, blockLength)
                    : _decoder.Decode(bufferPtr, blockLength);
            }
        }

		private bool ReadDecoded(byte[] buffer, ref int offset, ref int count, ref int read)
		{
			if (_decoded <= 0)
				return true;

			int length = Math.Min(count, _decoded);
			_decoder.Drain(buffer, offset, -_decoded, length);
			_position += length;
			_decoded -= length;
			offset += length;
			count -= length;
			read += length;

			return _interactive;
		}

		private int ReadN(byte[] buffer, int offset, int count, bool optional = false)
		{
			int index = 0;
			while (count > 0)
			{
				int read = _inner.Read(buffer, index + offset, count);
				if (read == 0)
				{
					if (index == 0 && optional)
						return 0;

					throw new EndOfStreamException(RS.UnexpectedEndOfStream);
                }

				index += read;
				count -= read;
			}

			return index;
		}

		private bool ReadN(int count, bool optional = false)
		{
			if (count == 0)
                return true;

			int read = ReadN(_buffer16, _index16, count, optional);
			_index16 += read;

			return read > 0;
		}

		private void Read0()
        {
            _index16 = 0;
        }

		private ulong Read64()
		{
			ReadN(sizeof(ulong));
			return BitConverter.ToUInt64(_buffer16, _index16 - sizeof(ulong));
		}

		private uint? TryRead32()
		{
			if (!ReadN(sizeof(uint), true))
				return null;

			return BitConverter.ToUInt32(_buffer16, _index16 - sizeof(uint));
		}

		private uint Read32()
		{
			ReadN(sizeof(uint));
			return BitConverter.ToUInt32(_buffer16, _index16 - sizeof(uint));
		}

		private ushort Read16()
		{
			ReadN(sizeof(ushort));
			return BitConverter.ToUInt16(_buffer16, _index16 - sizeof(ushort));
		}

		private byte Read8()
		{
			ReadN(sizeof(byte));
			return _buffer16[_index16 - 1];
		}

        /// <see cref="Stream.Dispose()"/>
        public new void Dispose()
		{
			Dispose(true);
			base.Dispose();
		}

		/// <see cref="Stream.Dispose(bool)"/>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (!disposing)
				return;

			CloseFrame();
			if (!_leaveOpen)
				_inner.Dispose();
		}

		/// <see cref="Stream.CanRead"/>
		public override bool CanRead
        {
            get { return _inner.CanRead; }
        }

		/// <see cref="Stream.CanSeek"/>
		public override bool CanSeek
        {
            get { return false; }
        }

		/// <see cref="Stream.CanWrite"/>
		public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Length of stream.
        /// </summary>
        /// <remarks>
        /// This will only work if original LZ4 stream has <see cref="ILZ4FrameDescriptor.ContentLength"/> set in the descriptor. 
        /// Otherwise, the returned value will always be -1.
        /// </remarks>
        public override long Length
		{
			get
			{
				EnsureFrame();
				return _frameInfo?.ContentLength ?? -1;
			}
		}

		/// <summary>
		/// Position within the stream. Position can be read, but cannot be set as LZ4 stream does
		/// not have <see cref="Stream.Seek(long, SeekOrigin)"/> capability.
		/// </summary>
		public override long Position
		{
            get { return _position; }
            set
            {
                throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "SetPosition", GetType().Name));
            }
		}

		/// <see cref="Stream.CanTimeout"/>
		public override bool CanTimeout
        {
            get { return _inner.CanTimeout; }
        }

		/// <see cref="Stream.WriteTimeout"/>
		public override int WriteTimeout
		{
            get { return _inner.WriteTimeout; }
            set { _inner.WriteTimeout = value; }
		}

		/// <see cref="Stream.ReadTimeout"/>
		public override int ReadTimeout
		{
            get { return _inner.ReadTimeout; }
            set { _inner.ReadTimeout = value; }
		}

		/// <see cref="Stream.Seek(long, SeekOrigin)"/>
		public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "Seek", GetType().Name));
        }

        /// <see cref="Stream.SetLength(long)"/>
        public override void SetLength(long value)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "SetLength", GetType().Name));
        }

        /// <see cref="Stream.Write(byte[], int, int)"/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "Write", GetType().Name));
        }

        /// <see cref="Stream.WriteByte(byte)"/>
        public override void WriteByte(byte value)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "WriteByte", GetType().Name));
        }

        /// <see cref="Stream.WriteAsync(byte[], int, int, CancellationToken)"/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "WriteAsync", GetType().Name));
        }
	}
}
