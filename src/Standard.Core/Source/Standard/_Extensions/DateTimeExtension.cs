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
    }
}
