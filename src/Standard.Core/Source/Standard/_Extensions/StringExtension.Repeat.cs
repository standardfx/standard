using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        public static string Repeat(this string value)
        {
            return Repeat(value, 1);
        }

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