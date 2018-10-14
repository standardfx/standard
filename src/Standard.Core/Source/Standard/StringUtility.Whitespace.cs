using System;

namespace Standard
{
    /// <summary>
    /// Utility class for string manipulation.
    /// </summary>
    public static partial class StringUtility
    {
        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="value"/> parameter is null or <see cref="string.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.
        /// </returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
#if NET35
            if (value != null)
            {
                for (int index = 0; index < value.Length; ++index)
                {
                    if (!char.IsWhiteSpace(value[index]))
                        return false;
                }
            }

            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }
    }
}
