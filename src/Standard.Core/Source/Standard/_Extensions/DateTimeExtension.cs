using System;

namespace Standard
{
    /// <summary>
    /// Extension methods for the <see cref="DateTime"/> class.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> object to Unix epoch time.
        /// </summary>
        /// <param name="dateTime">A <see cref="DateTime"/> object.</param>
        /// <returns>
        /// The Unix epoch time.
        /// </returns>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            if (dateTime == null)   
                throw new ArgumentNullException(nameof(dateTime));

            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> object to Unix epoch time, using high precision milliseconds.
        /// </summary>
        /// <param name="dateTime">A <see cref="DateTime"/> object.</param>
        /// <returns>
        /// The Unix epoch time in milliseconds.
        /// </returns>
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            if (dateTime == null)   
                throw new ArgumentNullException(nameof(dateTime));

            return ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        }

        // DateTimeOffset.ToUnixTimeSeconds() api requires a min target of net46, netcore10, or netstandard1.3
#if NET45 || NET40 || NET35 || NET20 || NET11 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2
        private const long UnixEpochSeconds = 62135596800L;
        private const long UnixEpochMilliseconds = UnixEpochSeconds * 1000;
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> object to Unix epoch time.
        /// </summary>
        /// <param name="offset">A <see cref="DateTimeOffset"/> object.</param>
        /// <returns>
        /// The Unix epoch time in seconds.
        /// </returns>
        public static long ToUnixTimeSeconds(this DateTimeOffset offset)
        {
            long seconds = offset.UtcDateTime.Ticks / TicksPerSecond;
            return seconds - UnixEpochSeconds;
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> object to Unix epoch time, using high precision milliseconds.
        /// </summary>
        /// <param name="offset">A <see cref="DateTimeOffset"/> object.</param>
        /// <returns>
        /// The Unix epoch time in milliseconds.
        /// </returns>
        public static long ToUnixTimeMilliseconds(this DateTimeOffset offset)
        {
            long milliseconds = offset.UtcDateTime.Ticks / TicksPerMillisecond;
            return milliseconds - UnixEpochMilliseconds;
        }
#endif
    }
}
