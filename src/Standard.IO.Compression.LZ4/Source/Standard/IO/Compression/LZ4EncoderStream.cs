using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Standard.IO.Compression.LZ4Encoding;
using Standard.Security.Cryptography;
using Standard.IO.Compression.LZ4;

namespace Standard.IO.Compression
{
	/// <summary>
	/// LZ4 compression stream. Use an encoding method in <see cref="LZ4Stream"/> to create an instance of this class.
	/// </summary>
	public class LZ4EncoderStream : Stream, IDisposable
	{
		private readonly Stream _inner;
		private readonly byte[] _buffer16 = new byte[16];
		private int _index16;

		private ILZ4Encoder _encoder;
		private readonly Func<ILZ4FrameDescriptor, ILZ4Encoder> _encoderFactory;

		private readonly ILZ4FrameDescriptor _descriptor;
		private readonly bool _leaveOpen;

		private byte[] _buffer;

		/// <summary>
        /// Creates a new instance of the <see cref="LZ4EncoderStream"/> class.
        /// </summary>
		/// <param name="inner">Inner stream.</param>
		/// <param name="descriptor">LZ4 descriptor.</param>
		/// <param name="encoderFactory">A function to return the appropriate encoder according to the <paramref name="descriptor"/>.</param>
		/// <param name="leaveOpen">Indicates whether <paramref name="inner"/> stream should be left open after disposing.</param>
		internal LZ4EncoderStream(Stream inner, ILZ4FrameDescriptor descriptor, Func<ILZ4FrameDescriptor, ILZ4Encoder> encoderFactory, bool leaveOpen = false)
		{
			_inner = inner;
			_descriptor = descriptor;
			_encoderFactory = encoderFactory;
			_leaveOpen = leaveOpen;
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

#if NETFX || NETSTANDARD2_0
		/// <see cref="Stream.Close()"/>
		public override void Close() 
        { 
            CloseFrame(); 
        }
#else
        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            CloseFrame();
        }
#endif

		/// <see cref="Stream.WriteByte(byte)"/>
		public override void WriteByte(byte value)
		{
			_buffer16[_index16] = value;
			Write(_buffer16, _index16, 1);
		}

		/// <see cref="Stream.Write(byte[], int, int)"/>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_encoder == null)
				WriteFrame();

			while (count > 0)
			{
				EncoderAction action = _encoder.TopupAndEncode(
					buffer, offset, count,
					_buffer, 0, _buffer.Length,
					false, true,
					out int loaded,
					out int encoded);

				WriteBlock(encoded, action);

				offset += loaded;
				count -= loaded;
			}
		}

		private void WriteFrame()
		{
			Write32(0x184D2204);
			Flush16();

			const int versionCode = 0x01;
			bool blockChaining = _descriptor.Chaining;
			bool blockChecksum = _descriptor.BlockChecksum;
			bool contentChecksum = _descriptor.ContentChecksum;
			bool hasContentSize = _descriptor.ContentLength.HasValue;
			bool hasDictionary = _descriptor.Dictionary.HasValue;

			int flg = (versionCode << 6) |
				((blockChaining ? 0 : 1) << 5) |
				((blockChecksum ? 1 : 0) << 4) |
				((hasContentSize ? 1 : 0) << 3) |
				((contentChecksum ? 1 : 0) << 2) |
				(hasDictionary ? 1 : 0);

			int blockSize = _descriptor.BlockSize;

			int bd = MaxBlockSizeCode(blockSize) << 4;

			Write16((ushort) ((flg & 0xFF) | (bd & 0xFF) << 8));

			if (hasContentSize)
            {
                // Write64(contentSize)
                throw new NotImplementedException(string.Format(RS.FeatureNotImplementedInType, "Content Size", GetType().Name));
            }

			if (hasDictionary)
            {
                // Write32(dictionaryId)
                throw new NotImplementedException(string.Format(RS.FeatureNotImplementedInType, "Predefined Dictionaries", GetType().Name));
            }

			byte hc = (byte)(XXHash32.DigestOf(_buffer16, 0, _index16) >> 8);

			Write8(hc);
			Flush16();

			_encoder = CreateEncoder();
			_buffer = new byte[LZ4Codec.MaximumOutputSize(blockSize)];
		}

		private ILZ4Encoder CreateEncoder()
		{
			ILZ4Encoder encoder = _encoderFactory(_descriptor);
			if (encoder.BlockSize > _descriptor.BlockSize)
                throw new ArgumentException(RS.BlockSizeLargerThanExpected);

			return encoder;
		}

		private void CloseFrame()
		{
			if (_encoder == null)
				return;

			try
			{
				EncoderAction action = _encoder.FlushAndEncode(_buffer, 0, _buffer.Length, true, out int encoded);
				WriteBlock(encoded, action);

				Write32(0);
				Flush16();

				if (_descriptor.ContentChecksum)
                    throw new NotImplementedException(string.Format(RS.FeatureNotImplementedInType, "Content Checksum", GetType().Name));

				_buffer = null;

				_encoder.Dispose();
			}
			finally
			{
				_encoder = null;
			}
		}

		private int MaxBlockSizeCode(int blockSize)
        {
            return blockSize <= LZ4MemoryHelper.K64 ? 4 :
                blockSize <= LZ4MemoryHelper.K256 ? 5 :
                blockSize <= LZ4MemoryHelper.M1 ? 6 :
                blockSize <= LZ4MemoryHelper.M4 ? 7 :
                throw new ArgumentException(string.Format(RS.BadBlockSizeInType, blockSize, GetType().Name));
        }

        private void WriteBlock(int length, EncoderAction action)
		{
			switch (action)
			{
				case EncoderAction.Copied:
					WriteBlock(length, false);
					break;
				case EncoderAction.Encoded:
					WriteBlock(length, true);
					break;
			}
		}

		private void WriteBlock(int length, bool compressed)
		{
			if (length <= 0)
				return;

			Write32((uint) length | (compressed ? 0 : 0x80000000));
			Flush16();

			_inner.Write(_buffer, 0, length);

			if (_descriptor.BlockChecksum)
                throw new NotImplementedException(string.Format(RS.FeatureNotImplementedInType, "Block Checksum", GetType().Name));
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

		private void Write8(byte value)
        {
            _buffer16[_index16++] = value;
        }

        private void Write16(ushort value)
        {
            _buffer16[_index16 + 0] = (byte)value;
            _buffer16[_index16 + 1] = (byte)(value >> 8);
            _index16 += 2;
        }

        private void Write32(uint value)
        {
            _buffer16[_index16 + 0] = (byte)value;
            _buffer16[_index16 + 1] = (byte)(value >> 8);
            _buffer16[_index16 + 2] = (byte)(value >> 16);
            _buffer16[_index16 + 3] = (byte)(value >> 24);
            _index16 += 4;
        }

        /*
		private void Write64(ulong value)
		{
		    _buffer16[_index16 + 0] = (byte) value;
		    _buffer16[_index16 + 1] = (byte) (value >> 8);
		    _buffer16[_index16 + 2] = (byte) (value >> 16);
		    _buffer16[_index16 + 3] = (byte) (value >> 24);
		    _buffer16[_index16 + 4] = (byte) (value >> 32);
		    _buffer16[_index16 + 5] = (byte) (value >> 40);
		    _buffer16[_index16 + 6] = (byte) (value >> 48);
		    _buffer16[_index16 + 7] = (byte) (value >> 56);
		    _index16 += 8;
		}
		*/

        private void Flush16()
		{
			if (_index16 > 0)
				_inner.Write(_buffer16, 0, _index16);

			_index16 = 0;
		}

        /// <see cref="Stream.CanRead"/>
		public override bool CanRead
        {
            get { return false; }
        }

        /// <see cref="Stream.CanSeek"/>
		public override bool CanSeek
        {
            get { return false; }
        }

        /// <see cref="Stream.CanWrite"/>
        public override bool CanWrite
        {
            get { return _inner.CanWrite; }
        }

		/// <summary>
        /// Length of the stream.
        /// </summary>
        /// <remarks>
        /// This feature is not implemented in this version. The property will always return -1.
        /// </remarks>
		public override long Length
        {
            get { return -1; }
        }

		/// <summary>
        /// Position in the stream.
        /// </summary>
        /// <remarks>
        /// This feature is not implemented in this version. The property will always return -1, and 
        /// will throw an <see cref="InvalidOperationException"/> when set.
        /// </remarks>
		public override long Position
		{
            get { return -1; }
            set
            {
                throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "Position", GetType().Name));
            }
		}

        /// <see cref="Stream.CanTimeout"/>
        public override bool CanTimeout
        {
            get { return _inner.CanTimeout; }
        }


		/// <see cref="Stream.ReadTimeout"/>
		public override int ReadTimeout
		{
            get { return _inner.ReadTimeout; }
            set { _inner.ReadTimeout = value; }
		}

		/// <see cref="Stream.WriteTimeout"/>
		public override int WriteTimeout
		{
            get { return _inner.WriteTimeout; }
            set { _inner.WriteTimeout = value; }
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

        /// <see cref="Stream.Read(byte[], int, int)"/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "Read", GetType().Name));
        }

        /// <see cref="Stream.ReadAsync(byte[], int, int, CancellationToken)"/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "ReadAsync", GetType().Name));
        }

        /// <see cref="Stream.ReadByte()"/>
        public override int ReadByte()
        {
            throw new InvalidOperationException(string.Format(RS.OperationNotAllowedInType, "ReadByte", GetType().Name));
        }
	}
}
