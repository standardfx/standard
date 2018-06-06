using System;
using Standard.Core;

namespace Standard
{
    partial class MathUtility
    {
        private const string BaseDigitSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Converts the given number from the numeral system with the specified radix (in the range [2, 36]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// <param name="radix">The radix of the numeral system the given number is in (in the range [2, 36]).</param>
        /// <returns>
        /// The equivilance of <paramref name="number"/> in decimal radix.
        /// </returns>
        public static long BaseToDecimal(string number, int radix)
        {
            return BaseToDecimal(number, radix, BaseDigitSymbols);
        }

        /// <summary>
        /// Converts the given number from the numeral system with the specified radix (in the range [2, 36]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// <param name="radix">The radix of the numeral system the given number is in (in the range [2, 36]).</param>
        /// <param name="digits">Defines the symbol to use for each digit in the numeral system radix. The length of this string must be larger or equal to the radix being defined.</param>
        /// <returns>
        /// The equivilance of <paramref name="number"/> in decimal radix.
        /// </returns>
        public static long BaseToDecimal(string number, int radix, string digits)
        {
            if (radix < 2 || radix > digits.Length)
                throw new ArgumentException(string.Format(RS.RadixOutOfRange, digits.Length.ToString()));

            if (string.IsNullOrEmpty(number))
                return 0;

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException(RS.InvalidCharForNumeralSystem, "number");
                else if (digit >= radix)
                    throw new ArgumentException(RS.ExtraNonParsableChar, "number");

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns>
        /// The equivilance of <paramref name="decimalNumber"/> in the destinated radix.
        /// </returns>
        public static string DecimalToBase(long decimalNumber, int radix)
        {
            return DecimalToBase(decimalNumber, radix, BaseDigitSymbols);
        }

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <param name="digits">
        /// Defines the symbol to use for each digit in the numeral system radix. The length of 
        /// this string must be larger or equal to the radix being defined.
        /// </param>
        /// <returns>
        /// The equivilance of <paramref name="decimalNumber"/> in the destinated radix.
        /// </returns>
        public static string DecimalToBase(long decimalNumber, int radix, string digits)
        {
            const int bitsInLong = 64;

            if (radix < 2 || radix > digits.Length)
                throw new ArgumentException(string.Format(RS.RadixOutOfRange, digits.Length.ToString()));

            if (decimalNumber == 0)
                return "0";

            int index = bitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[bitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new string(charArray, index + 1, bitsInLong - index - 1);
            if (decimalNumber < 0)
                result = "-" + result;

            return result;
        }
    }
}
