using System;

namespace System.Text
{
    /// <summary>
    /// Categorizes ASCII characters for various encoding and encryption purposes.
    /// </summary>
    [Flags]
    public enum ASCIICharacterGroup
    {
        /// <summary>
        /// Numbers from 0 to 9.
        /// </summary>
        Digit = 1,

        /// <summary>
        /// Upper case letters from 'A' to 'F'.
        /// </summary>
        UpperCaseAToF = 2,

        /// <summary>
        /// Upper case letters from 'G' to 'Z'.
        /// </summary>
        UpperCaseGToZ = 4,

        /// <summary>
        /// All lower case letters from 'a' to 'z'.
        /// </summary>
        LowerCase = 8,

        /// <summary>
        /// Space (' ') character.
        /// </summary>
        Space = 16,

        /// <summary>
        /// The symbols '!', '"', '#', '$', '&amp;', ''', '*', ',', '-', '.', /', ';', ':', '?', '@', '\', '^', _', `' , '|', and '~'.
        /// </summary>
        Punctuation = 32,

        /// <summary>
        /// Characters '%', '+', '&lt;', '=', and '&gt;'.
        /// </summary>
        MathOperator = 64,

        /// <summary>
        /// Characters '(', ')', '[', ']', '{', and '}'.
        /// </summary>
        Bracket = 128,

        /// <summary>
        /// The 'backspace' control character.
        /// </summary>
        Backspace = 256,

        /// <summary>
        /// The 'horizontal tab' control character.
        /// </summary>
        HorizontalTab = 512,

        /// <summary>
        /// The 'line feed' control character.
        /// </summary>
        LineFeed = 1024,

        /// <summary>
        /// The 'vertical tab' control character.
        /// </summary>
        VerticalTab = 2048,

        /// <summary>
        /// The 'carriage return' control character.
        /// </summary>
        CarriageReturn = 4096,

        /// <summary>
        /// Legacy control characters.
        /// </summary>
        LegacyControl = 8192,

        /// <summary>
        /// The 'carriage return' + 'line feed' control characters sequence, which is used on Windows systems to denote newlines.
        /// </summary>
        CarriageReturnLineFeed = CarriageReturn | LineFeed,

        /// <summary>
        /// Commonly used control characters: 'backspace', 'horizontal tab', 'line feed', 'vertical tab', and 'carriage return'.
        /// </summary>
        CommonControl = Backspace | HorizontalTab | LineFeed | VerticalTab | CarriageReturn,

        /// <summary>
        /// All control characters.
        /// </summary>
        Control = CommonControl | LegacyControl,

        /// <summary>
        /// Upper case letters from 'A' to 'Z'.
        /// </summary>
        UpperCase = UpperCaseAToF | UpperCaseGToZ,

        /// <summary>
        /// Numbers from 0 to 9, and upper case letters from 'A' to 'F'.
        /// </summary>
        Hexadecimal = Digit | UpperCaseAToF,

        /// <summary>
        /// Numberd from 0 to 9, upper case letters from 'A' to 'Z', and lower case letters from 'a' to 'z'.
        /// </summary>
        Alphanumeric = Digit | UpperCase | LowerCase,

        /// <summary>
        /// Numberd from 0 to 9, and lower case letters from 'a' to 'z'.
        /// </summary>
        AlphanumericLowerCase = Digit | LowerCase,

        /// <summary>
        /// Numberd from 0 to 9, and upper case letters from 'A' to 'Z'.
        /// </summary>
        AlphanumericUpperCase = Digit | UpperCase,

        /// <summary>
        /// Upper case and lower case letters ('A' to 'Z', 'a' to 'z', and 0 to 9), space (' '), 
        /// plus symbols defined under the 'Punctuation', 'MathOperator', and 'Bracket' enum members.
        /// </summary>
        Printable = UpperCase | LowerCase | Digit | Space | Punctuation | MathOperator | Bracket,
    }    
}