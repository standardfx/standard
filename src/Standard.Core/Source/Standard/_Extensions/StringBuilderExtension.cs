using System;
using System.Text;

namespace Standard
{
    /// <summary>
    /// Extension methods for the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtension
    {
        /// <summary>
        /// Removes the specified number of characters from the end of a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder"/> object.</param>
        /// <param name="count">The number of characters to remove.</param>
        /// <returns>
        /// A new <see cref="StringBuilder"/> instance with characters removed from the end.
        /// </returns>
        public static StringBuilder RemoveEnd(this StringBuilder sb, int count)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            return sb.Remove(sb.Length - count, count);
        }

        /// <summary>
        /// Removes the specified number of characters from the start of a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder"/> object.</param>
        /// <param name="count">The number of characters to remove.</param>
        /// <returns>
        /// A new <see cref="StringBuilder"/> instance with characters removed from the start.
        /// </returns>
        public static StringBuilder RemoveStart(this StringBuilder sb, int count)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
                
            return sb.Remove(0, count);
        }
    }
}