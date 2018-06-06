using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            char[] arr = value.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string Reverse(this string value, bool unicode)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (!unicode)
                return Reverse(value);

            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(value);
            List<string> elements = new List<string>();

            while (enumerator.MoveNext())
            {
                elements.Add(enumerator.GetTextElement());
            }

            elements.Reverse();

            return string.Concat(elements);
        }
    }
}