namespace Standard
{
    /// <summary>
    /// Extensions for binary shifting operations on integers.
    /// </summary>
    public static class NumberShiftExtension
    {
        /// <summary>
        /// Performs binary right shift as an unsigned 32-bit integer.
        /// </summary>
        public static int UShiftRight(this int value, int count)
        {
            return (int)((uint)value >> count);
        }

        /// <summary>
        /// Performs binary left shift as an unsigned 32-bit integer.
        /// </summary>
        public static int UShiftLeft(this int value, int count)
        {
            return (int)((uint)value << count);
        }

        /// <summary>
        /// Performs binary right shift as an unsigned 64-bit integer.
        /// </summary>
        public static long UShiftRight(this long value, int count)
        {
            return (long)((ulong)value >> count);
        }

        /// <summary>
        /// Performs binary left shift as an unsigned 64-bit integer.
        /// </summary>
        public static long UShiftLeft(this long value, int count)
        {
            return (long)((ulong)value << count);
        }
    }
}
