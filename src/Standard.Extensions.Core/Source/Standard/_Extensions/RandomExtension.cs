using System;
using System.Text;
using Standard.Extensions.Core;

namespace Standard
{
    // http://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a random long from min (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="minValue">The inclusive minimum bound</param>
        /// <param name="maxValue">The exclusive maximum bound. Must be greater than min.</param>
        public static long NextInt64(this Random random, long minValue, long maxValue)
        {
            if (maxValue <= minValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), string.Format(RS.Err_MinGtMax, minValue, maxValue));

            // Working with ulong so that modulo works correctly with values > long.MaxValue
            ulong uRange = (ulong)(maxValue - minValue);

            // Prevent a modolo bias; see http://stackoverflow.com/a/10984975/238419 for more information.
            // In the worst case, the expected number of calls is 2 (though usually it's much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + minValue;
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="random">The given random instance.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <c>maxValue</c> must be greater than or equal to 0.</param>
        public static long NextInt64(this Random random, long maxValue)
        {
            return random.NextInt64(0, maxValue);
        }

        /// <summary>
        /// Returns a non-negative random <see cref="System.Int64" />.
        /// </summary>
        /// <param name="random">The given random instance.</param>
        /// <return>A 64-bit signed integer that is greater than or equal to 0 and less than <see cref="Int64.MaxValue">MaxValue</see>.</return>
        /// <remarks>
        /// Random.NextInt64 generates a random number whose value ranges from 0 to less than <see cref="Int64.MaxValue" />. To generate a random number 
        /// whose value ranges from 0 to some other positive number, use the <c>NextInt64(Int64)</c> method overload. To generate a 
        /// random number within a different range, use the <c>NextInt64(Int64, Int64)</c> method overload.
        /// </remarks>
        public static long NextInt64(this Random random)
        {
            return random.NextInt64(long.MinValue, long.MaxValue);
        }

        public static string NextString(this Random random, char[] chars)
        {
            return NextString(random, chars, 1);
        }

        public static string NextString(this Random random)
        {
                return NextString(random, ASCIICharacterGroup.Alphanumeric, 1, null, null);
        }

        public static string NextString(this Random random, int length)
        {
                return NextString(random, ASCIICharacterGroup.Alphanumeric, length, null, null);
        }

        public static string NextString(this Random random, ASCIICharacterGroup charGroup, int length)
        {
                return NextString(random, charGroup, length, null, null);
        }

        public static string NextString(this Random random, ASCIICharacterGroup charGroup, int length, char[] include)
        {
                return NextString(random, charGroup, length, include, null);
        }

        public static string NextString(this Random random, char[] chars, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), RS.Err_RequireGeZero);

            if (length == 0)
                return string.Empty;

            if (chars == null || chars.Length == 0)
                throw new ArgumentNullException(nameof(chars));

            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString();
        }

        public static string NextString(this Random random, ASCIICharacterGroup charGroup, int length, char[] include, char[] exclude)
        {
            string charMap = string.Empty;

            if (charGroup.HasFlag(ASCIICharacterGroup.Digit))
                charMap += "0123456789";

            if (charGroup.HasFlag(ASCIICharacterGroup.UpperCaseAToF))
                charMap += "ABCDEF";

            if (charGroup.HasFlag(ASCIICharacterGroup.UpperCaseGToZ))
                charMap += "GHIJKLMNOPQRSTUVWXYZ";

            if (charGroup.HasFlag(ASCIICharacterGroup.LowerCase))
                charMap += "abcdefghijklmnopqrstuvxwyz";

            if (charGroup.HasFlag(ASCIICharacterGroup.Space))
                charMap += " ";

            if (charGroup.HasFlag(ASCIICharacterGroup.Punctuation))
                charMap += "!\"#$&'*,-./;:?@\\^_`|~";

            if (charGroup.HasFlag(ASCIICharacterGroup.MathOperator))
                charMap += "%+<=>";

            if (charGroup.HasFlag(ASCIICharacterGroup.Bracket))
                charMap += "()[]{}";

            if (include != null)
            {
                charMap = StringExtension.Remove(charMap, include);
                charMap = string.Concat(charMap, new string(include));
            }

            if (exclude != null)
                charMap = StringExtension.Remove(charMap, exclude);

            return NextString(random, charMap.ToCharArray(), length);
        }

        public static void Shuffle<T>(this Random random, T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int n = array.Length;
            if (n <= 1)
                return;

            while (n > 1) 
            {
                int k = random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}