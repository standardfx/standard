using System;

namespace Standard
{
    /// <summary>
    ///  Utility class for working with <see cref="DateTime"/> and <see cref="DateTimeOffset"/> objects.
    /// </summary>
    public static class DateTimeUtility
    {
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Converts a Unix epoch time to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="unixTime">he Unix epoch time, represented by an <see cref="Int64"/>.</param>
        /// <param name="msPrecision">`true` means that the <paramref name="unixTime"/> is in milliseconds; `false` means that the <paramref name="unixTime"/> is in seconds.</param>
        /// <returns>A <see cref="DateTime"/> representation of Unix epoch time.</returns>
        public static DateTime ToDateTime(long unixTime, bool msPrecision = false)
        {
            return ToDateTimeOffset(unixTime, msPrecision).DateTime;
        }

        /// <summary>
        /// Converts a Unix epoch time to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="unixTime">he Unix epoch time, represented by an <see cref="Int64"/>.</param>
        /// <param name="msPrecision">`true` means that the <paramref name="unixTime"/> is in milliseconds; `false` means that the <paramref name="unixTime"/> is in seconds.</param>
        /// <returns>A <see cref="DateTimeOffset"/> representation of Unix epoch time.</returns>

        public static DateTimeOffset ToDateTimeOffset(long unixTime, bool msPrecision = false)
        {
            if (!msPrecision)
                return UnixEpoch.AddSeconds((long)unixTime);
            else
                return UnixEpoch.AddMilliseconds((long)unixTime);
        }
    }
}
