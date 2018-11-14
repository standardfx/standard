using System;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// Extends the functionality of various LZ4 encoders on top of the <see cref="ILZ4Encoder"/> interface.
    /// </summary>
    internal static class LZ4EncoderExtensions
	{
		/// <summary>
        /// Tops encoder up with some data.
        /// </summary>
		/// <param name="encoder">The encoder instance.</param>
		/// <param name="source">Buffer pointer, will be shifted after operation by the number of bytes actually loaded.</param>
		/// <param name="length">Length of buffer.</param>
		/// <returns>`true` if buffer was topped up; or `false` if no bytes were loaded.</returns>
		public static unsafe bool Topup(this ILZ4Encoder encoder, ref byte* source, int length)
		{
			int loaded = encoder.Topup(source, length);
			source += loaded;
			return loaded != 0;
		}

        /// <summary>
        /// Tops encoder up with some data.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="source">The buffer data.</param>
        /// <param name="offset">Offset in the buffer.</param>
        /// <param name="length">Length of buffer.</param>
        /// <returns>
        /// Number of bytes actually loaded.
        /// </returns>
        public static unsafe int Topup(this ILZ4Encoder encoder, byte[] source, int offset, int length)
		{
			fixed (byte* sourcePtr = source)
            {
                return encoder.Topup(sourcePtr + offset, length);
            }
        }

        /// <summary>
        /// Tops encoder up with some data.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="source">The buffer data.</param>
        /// <param name="offset">Buffer offset, will be increased after operation by the number of bytes actually loaded.</param>
        /// <param name="length">Length of buffer.</param>
		/// <returns>`true` if buffer was topped up; or `false` if no bytes were loaded.</returns>
        public static bool Topup(this ILZ4Encoder encoder, byte[] source, ref int offset, int length)
		{
			int loaded = encoder.Topup(source, offset, length);
			offset += loaded;
			return loaded != 0;
		}

        /// <summary>
        /// Encodes all bytes currently stored in the encoder into target buffer.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="target">The target buffer.</param>
        /// <param name="offset">Offset in target buffer.</param>
        /// <param name="length">Length of target buffer.</param>
        /// <param name="allowCopy">If `true`, copying bytes is allowed.</param>
        /// <returns>
        /// Number of bytes encoded. If bytes were copied than this value is negative.
        /// </returns>
        public static unsafe int Encode(this ILZ4Encoder encoder, byte[] target, int offset, int length, bool allowCopy)
		{
			fixed (byte* targetPtr = target)
            {
                return encoder.Encode(targetPtr + offset, length, allowCopy);
            }
        }

        /// <summary>
        /// Encodes all bytes currently stored in encoder into target buffer.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="target">Target buffer.</param>
        /// <param name="offset">Offset in target buffer. Will be updated after operation.</param>
        /// <param name="length">Length of target buffer.</param>
        /// <param name="allowCopy">If `true`, copying bytes is allowed.</param>
        /// <returns>
        /// Result of this action. Bytes can be copied (<see cref="EncoderAction.Copied"/>),
        /// encoded (<see cref="EncoderAction.Encoded"/>) or nothing could have
        /// happened (<see cref="EncoderAction.None"/>).
        /// </returns>
        public static EncoderAction Encode(this ILZ4Encoder encoder, byte[] target, ref int offset, int length, bool allowCopy)
		{
			int encoded = encoder.Encode(target, offset, length, allowCopy);
			offset += Math.Abs(encoded);
			return encoded == 0 ? EncoderAction.None :
				encoded < 0 ? EncoderAction.Copied :
				EncoderAction.Encoded;
		}

        /// <summary>
        /// Encodes all bytes currently stored in encoder into target buffer.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="target">Target buffer. Will be updated after operation.</param>
        /// <param name="length">Length of buffer.</param>
        /// <param name="allowCopy">If `true`, copying bytes is allowed.</param>
        /// <returns>
        /// Result of this action. Bytes can be copied (<see cref="EncoderAction.Copied"/>),
        /// encoded (<see cref="EncoderAction.Encoded"/>) or nothing could have
        /// happened (<see cref="EncoderAction.None"/>).
        /// </returns>
        public static unsafe EncoderAction Encode(this ILZ4Encoder encoder, ref byte* target, int length, bool allowCopy)
		{
			int encoded = encoder.Encode(target, length, allowCopy);
			target += Math.Abs(encoded);
			return encoded == 0 ? EncoderAction.None :
				encoded < 0 ? EncoderAction.Copied :
				EncoderAction.Encoded;
		}

        /// <summary>
        /// Tops encoder and encodes content.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="source">Source buffer (used to top up from).</param>
        /// <param name="sourceLength">Source buffer length.</param>
        /// <param name="target">Target buffer (used to encode into).</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="forceEncode">Forces encoding even if encoder is not full.</param>
        /// <param name="allowCopy">Allows to copy bytes if compression was not possible.</param>
        /// <param name="loaded">Number of bytes loaded (topped up).</param>
        /// <param name="encoded">Number if bytes encoded or copied. Value is 0 if no encoding was done.</param>
        /// <returns>
        /// An <see cref="EncoderAction"/> indicating the action performed.
        /// </returns>
        public static unsafe EncoderAction TopupAndEncode(this ILZ4Encoder encoder, byte* source, int sourceLength, byte* target, int targetLength, bool forceEncode, bool allowCopy, 
            out int loaded, out int encoded)
		{
			loaded = 0;
			encoded = 0;

			if (sourceLength > 0)
				loaded = encoder.Topup(source, sourceLength);

			return encoder.FlushAndEncode(target, targetLength, forceEncode, allowCopy, loaded, out encoded);
		}

        /// <summary>
        /// Tops encoder and encodes content.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="source">Source buffer (used to top up from).</param>
        /// <param name="sourceOffset">Offset within source buffer.</param>
        /// <param name="sourceLength">Source buffer length.</param>
        /// <param name="target">Target buffer (used to encode into).</param>
        /// <param name="targetOffset">Offset within target buffer.</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="forceEncode">Forces encoding even if encoder is not full.</param>
        /// <param name="allowCopy">Allows to copy bytes if compression was not possible.</param>
        /// <param name="loaded">Number of bytes loaded (topped up).</param>
        /// <param name="encoded">Number if bytes encoded or copied. Value is 0 if no encoding was done.</param>
        /// <returns>
        /// An <see cref="EncoderAction"/> indicating the action performed.
        /// </returns>
        public static unsafe EncoderAction TopupAndEncode(this ILZ4Encoder encoder, byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, bool forceEncode, bool allowCopy,
			out int loaded, out int encoded)
		{
			fixed (byte* sourcePtr = source)
			fixed (byte* targetPtr = target)
            {
                return encoder.TopupAndEncode(
                    sourcePtr + sourceOffset, sourceLength,
                    targetPtr + targetOffset, targetLength,
                    forceEncode, allowCopy,
                    out loaded, out encoded);
            }
        }

		private static unsafe EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte* target, int targetLength, bool forceEncode, bool allowCopy, int loaded, out int encoded)
		{
			encoded = 0;

			int blockSize = encoder.BlockSize;
			int bytesReady = encoder.BytesReady;

			if (bytesReady < (forceEncode ? 1 : blockSize))
				return loaded > 0 ? EncoderAction.Loaded : EncoderAction.None;

			encoded = encoder.Encode(target, targetLength, allowCopy);
			if (allowCopy && encoded < 0)
			{
				encoded = -encoded;
				return EncoderAction.Copied;
			}
			else
			{
				return EncoderAction.Encoded;
			}
		}

        /// <summary>
        /// Encoded remaining bytes in the encoder.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="target">Target buffer.</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="allowCopy">Allows to copy bytes if compression was not possible.</param>
        /// <param name="encoded">Number if bytes encoded or copied. Value is 0 if no encoding was done.</param>
        /// <returns>
        /// An <see cref="EncoderAction"/> indicating the action performed.
        /// </returns>
        public static unsafe EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte* target, int targetLength, bool allowCopy, out int encoded)
        {
            return encoder.FlushAndEncode(target, targetLength, true, allowCopy, 0, out encoded);
        }

        /// <summary>
        /// Encoded remaining bytes in the encoder.
        /// </summary>
        /// <param name="encoder">The encoder instance.</param>
        /// <param name="target">Target buffer.</param>
        /// <param name="targetOffset">Offset within target buffer.</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="allowCopy">Allows to copy bytes if compression was not possible.</param>
        /// <param name="encoded">Number if bytes encoded or copied. Value is 0 if no encoding was done.</param>
        /// <returns>
        /// An <see cref="EncoderAction"/> indicating the action performed.
        /// </returns>
        public static unsafe EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte[] target, int targetOffset, int targetLength, bool allowCopy, out int encoded)
		{
			fixed (byte* targetPtr = target)
            {
                return encoder.FlushAndEncode(targetPtr + targetOffset, targetLength, true, allowCopy, 0, out encoded);
            }
        }

        /// <summary>
        /// Drains decoder by reading all bytes which are ready.
        /// </summary>
        /// <param name="decoder">The decoder instance.</param>
        /// <param name="target">Target buffer.</param>
        /// <param name="targetOffset">Offset within target buffer.</param>
        /// <param name="offset">Offset in decoder relatively to decoder's head. This should be a negative value.</param>
        /// <param name="length">Number of bytes.</param>
        public static unsafe void Drain(this ILZ4Decoder decoder, byte[] target, int targetOffset, int offset, int length)
		{
			fixed (byte* targetPtr = target)
            {
                decoder.Drain(targetPtr + targetOffset, offset, length);
            }
        }

        /// <summary>
        /// Decodes data and immediately drains it into target buffer.
        /// </summary>
        /// <param name="decoder">The decoder instance.</param>
        /// <param name="source">Source buffer (with compressed data, to be decoded).</param>
        /// <param name="sourceLength">Source buffer length.</param>
        /// <param name="target">Target buffer (to drained into).</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="decoded">Number of bytes actually decoded.</param>
        /// <returns>
        /// `true` if the decoder was drained; otherwise, `false`.
        /// </returns>
        public static unsafe bool DecodeAndDrain(this ILZ4Decoder decoder, byte* source, int sourceLength, byte* target, int targetLength, out int decoded)
		{
			decoded = 0;

			if (sourceLength <= 0)
				return false;

			decoded = decoder.Decode(source, sourceLength);
			if (decoded <= 0 || targetLength < decoded)
				return false;

			decoder.Drain(target, -decoded, decoded);

			return true;
		}

        /// <summary>
        /// Decodes data and immediately drains it into target buffer.
        /// </summary>
        /// <param name="decoder">The decoder instance.</param>
        /// <param name="source">Source buffer (with compressed data, to be decoded).</param>
        /// <param name="sourceOffset">Offset within source buffer.</param>
        /// <param name="sourceLength">Source buffer length.</param>
        /// <param name="target">Target buffer (to drained into).</param>
        /// <param name="targetOffset">Offset within target buffer.</param>
        /// <param name="targetLength">Target buffer length.</param>
        /// <param name="decoded">Number of bytes actually decoded.</param>
        /// <returns>
        /// `true` if the decoder was drained; otherwise, `false`.
        /// </returns>
        public static unsafe bool DecodeAndDrain(this ILZ4Decoder decoder, 
            byte[] source, int sourceOffset, int sourceLength, 
			byte[] target, int targetOffset, int targetLength,
			out int decoded)
		{
			fixed (byte* sourcePtr = source)
			fixed (byte* targetPtr = target)
            {
                return decoder.DecodeAndDrain(
                    sourcePtr + sourceOffset,
                    sourceLength,
                    targetPtr + targetOffset,
                    targetLength,
                    out decoded);
            }
        }
	}
}
