using System;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Extension methods for the <see cref="Array"/> class.
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Sets the value of each member of an array to <c>null</c>.
        /// </summary>
        /// <param name="array">The array to act on.</param>
        public static void ClearAll(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }
        
        /// <summary>
        /// Sets a range of elements in the array to zero, false, or null, depending on the element type.
        /// </summary>
        /// <param name="array">The array whose elements need to be cleared.</param>
        /// <param name="index">The starting index of the range of elements to clear.</param>
        /// <param name="length">The number of elements to clear.</param>
        public static void Clear(this Array array, int index, int length)
        {
            Array.Clear(array, index, length);
        }

        /// <summary>
        /// Copies a range of elements from an array starting at the specified source index and pastes them to another array, 
        /// starting at the specified destination index. Guarantees that all changes are undone if the copy does not succeed completely.
        /// </summary>
        /// <param name="array">The array that contains the data to copy.</param>
        /// <param name="sourceIndex">A 32-bit integer that represents the index in the array at which copying begins.</param>
        /// <param name="destination">The array that receives the data.</param>
        /// <param name="destIndex">A 32-bit integer that represents the index in the array at which storing begins.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        public static void ConstrainedCopy(this Array array, int sourceIndex, Array destination, int destIndex, int length)
        {
            Array.ConstrainedCopy(array, sourceIndex, destination, destIndex, length);
        }

        /// <summary>
        /// Copies a specified number of bytes from a source array starting at a particular offset to a destination array starting at a particular offset.
        /// </summary>
        /// <param name="array">The source buffer.</param>
        /// <param name="srcOffset">The zero-based byte offset into .</param>
        /// <param name="destination">The destination buffer.</param>
        /// <param name="destOffset">The zero-based byte offset into .</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static void BlockCopy(this Array array, int srcOffset, Array destination, int destOffset, int count)
        {
            Buffer.BlockCopy(array, srcOffset, destination, destOffset, count);
        }
        
        /// <summary>
        /// Returns the number of bytes in the specified array.
        /// </summary>
        /// <param name="array">An array.</param>
        /// <returns>The number of bytes in the array.</returns>
        public static int ByteLength(this Array array)
        {
            return Buffer.ByteLength(array);
        }
        
        /// <summary>
        /// Retrieves the byte at a specified location in a specified array.
        /// </summary>
        /// <param name="array">An array.</param>
        /// <param name="index">A location in the array.</param>
        /// <returns>Returns the  byte in the array.</returns>
        public static byte GetByte(this Array array, int index)
        {
            return Buffer.GetByte(array, index);
        }
        
        /// <summary>
        /// Assigns a specified value to a byte at a particular location in a specified array.
        /// </summary>
        /// <param name="array">An array.</param>
        /// <param name="index">A location in the array.</param>
        /// <param name="value">A value to assign.</param>
        public static void SetByte(this Array array, int index, byte value)
        {
            Buffer.SetByte(array, index, value);
        }
        
        /// <summary>
        /// Check if the array is lower than the specified index.
        /// </summary>
        /// <param name="array">The array to act on.</param>
        /// <param name="index">Zero-based index.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WithinIndex(this Array array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        /// <summary>
        /// Check if the array is lower than the specified index.
        /// </summary>
        /// <param name="array">The array to act on.</param>
        /// <param name="index">Zero-based index.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool WithinIndex(this Array array, int index, int dimension)
        {
            return index >= array.GetLowerBound(dimension) && index <= array.GetUpperBound(dimension);
        }
    }
}
