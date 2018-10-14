using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Repeat a string.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <returns>The value of <paramref name="value"/> repeated twice.</returns>
        public static string Repeat(this string value)
        {
            return Repeat(value, 1);
        }

        /// <summary>
        /// Repeat a string the number of times specified.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="times">Determines how many times <paramref name="value"/> should be repeated.</param>
        /// <returns>The value of <paramref name="value"/> repeated <paramref name="times"/> times.</returns>
        public static string Repeat(this string value, int times)
        {
            if (value == null)
                throw new ArgumentNullException((value));
            if (value == string.Empty)
                return value;

            // Repeat(1) actually means there should be 2 copies in total
            // "foo".Repeat(1) == "foofoo"
            times += 1;

            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times), RS.Err_RequireGtZero);
            else if (times == 1)
                return value;

            // performance boost by initializing StringBuilder to exact capacity
            int sbCapacity = value.Length * times;
            StringBuilder sb = new StringBuilder(sbCapacity, sbCapacity);
            for (int i = 0; i < times; i++)
            {
                sb.Append(value);
            }

            return sb.ToString();
        }
    }
}