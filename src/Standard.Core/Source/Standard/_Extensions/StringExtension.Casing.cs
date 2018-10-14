using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Converts the input string to `PascalCase`.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The pascal case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// Pascal case (aka. upper camel case, and more formally, upper medial capitals) format represents 
        /// compound words without intervening spaces or punctuations. Instead, each word begins with a 
        /// capital letter.
        ///
        /// For example, "John Smith" will be written as "JohnSmith", and "End-of-File" will be written as 
        /// "EndOfFile".
        ///
        /// Pascal case is distinct from lower camel case, where the first letter is in lower case, such as 
        /// "johnSmith" and "endOfFile". For lower camel case conversion, use the <see cref="ToCamelCase(string)"/>
        /// function.
        /// 
        /// If you already know that <paramref name="value"/> is in medial capitals (either PascalCase or camelCase), use the 
        /// <see cref="ToPascalCase(string, bool)"/> function. Typical speed gain is about 2-3 times.
        /// </remarks>
        public static string ToPascalCase(this string value)
        {
            char[] sepchars = { ' ', '-', '_' };
            bool isMedial = true;
            foreach (char c in sepchars)
            {
                if (value.IndexOf(c) != -1)
                {
                    isMedial = false;
                    break;
                }
            }
            
            return ToPascalCase(value, isMedial);
        }

        /// <summary>
        /// Converts the input string to `PascalCase`.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <returns>The pascal case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToPascalCase(string)"/> function.
        /// </remarks>
        [SecuritySafeCritical]
        public unsafe static string ToPascalCase(this string value, bool fromMedialCaps)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (fromMedialCaps)
                return FirstToUpperInvariant(value);

            fixed (char* p = value)
            {
                char* buffer = stackalloc char[value.Length];
                int c = 0;
                char* ptr = p;
            
                pc:
                char f = *ptr;
                if (f == ' ' || f == '_' || f == '-')
                {
                    // do nothing. will just to next char
                }
                else
                {
                    if (c >= 0 && f >= 97 && f <= 122)
                        *buffer = (char)(f - 32);
                    else
                        *buffer = *ptr;
                    ++c;
                    ptr++;
                    buffer++;                    
                }
                
                while ((f = *(ptr++)) != '\0')
                {
                    if (f == ' ' || f == '_' || f == '-')
                        goto pc;
                    
                    if (f >= 65 && f <= 90)
                        *(buffer++) = (char)(f + 32);
                    else
                        *(buffer++) = f;
                        
                    ++c;
                }
                buffer -= c;

                return new string(buffer, 0, c);
            }
        }

        /// <summary>
        /// Converts the input string to `camelCase`.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The camel case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// Camel case (aka. lower camel case, and more formally, lower medial capitals) format represents 
        /// compound words without intervening spaces or punctuations. Instead, each word begins with a 
        /// capital letter, except for the first letter, which must be in lower casing.
        ///
        /// For example, "John Smith" will be written as "johnSmith", and "End-of-File" will be written as 
        /// "endOfFile".
        ///
        /// Lower camel case is distinct from upper camel case or pascal case, where the first letter is in 
        /// upper case, such as "JohnSmith" and "EndOfFile". For pascal case conversion, use the <see cref="ToPascalCase(string)"/>
        /// function.
        /// 
        /// If you already know that <paramref name="value"/> is in medial capitals (either PascalCase or camelCase), use the 
        /// <see cref="ToCamelCase(string, bool)"/> function. Typical speed gain is about 2-3 times.
        /// </remarks>
        public static string ToCamelCase(this string value)
        {
            char[] sepchars = { ' ', '-', '_' };
            bool isMedial = true;
            foreach (char c in sepchars)
            {
                if (value.IndexOf(c) != -1)
                {
                    isMedial = false;
                    break;
                }
            }
            
            return ToCamelCase(value, isMedial);
        }

        /// <summary>
        /// Converts the input string to `camelCase`.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <returns>The camel case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToCamelCase(string)"/> function.
        /// </remarks>
        [SecuritySafeCritical]
        public unsafe static string ToCamelCase(this string value, bool fromMedialCaps)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (fromMedialCaps)
                return FirstToLowerInvariant(value);

            fixed (char* p = value)
            {
                char* buffer = stackalloc char[value.Length];
                int c = 0;
                char* ptr = p;
            
                pc:
                char f = *ptr;
                if (f == ' ' || f == '_' || f == '-')
                {
                    // do nothing. will just to next char
                }
                else
                {
                    if (c == 0 && f >= 65 && f <= 90)
                        *buffer = (char)(f + 32);
                    else if (c > 0 && f >= 97 && f <= 122)
                        *buffer = (char)(f - 32);
                    else
                        *buffer = *ptr;
                    ++c;
                    ptr++;
                    buffer++;
                }

                while ((f = *(ptr++)) != '\0')
                {
                    if (f == ' ' || f == '_' || f == '-')
                        goto pc;

                    if (f >= 65 && f <= 90)
                        *(buffer++) = (char)(f + 32);
                    else
                        *(buffer++) = f;
                    ++c;
                }
                buffer -= c;

                return new string(buffer, 0, c);
            }
        }

        /// <summary>
        /// Converts the input string to title case. A space (` `) character is used to separate each word.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The title case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// Title case represents compound words with one intervening space or punctuation. Each word begins 
        /// with a capital letter.
        ///
        /// For example, "EndOfFile" will be written as "End Of File".
        ///
        /// If you already know that <paramref name="value"/> is in medial capitals (either PascalCase or camelCase), use the 
        /// <see cref="ToTitleCase(string, bool)"/> function. Typical speed gain is about 2-3 times.
        /// </remarks>
        public static string ToTitleCase(this string value)
        {
            char[] sepchars = { ' ', '-', '_' };
            bool isMedial = true;
            foreach (char c in sepchars)
            {
                if (value.IndexOf(c) != -1)
                {
                    isMedial = false;
                    break;
                }
            }

            return ToTitleCase(value, isMedial, ' ');
        }

        /// <summary>
        /// Converts the input string to title case. A space (` `) character is used to separate each word.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <returns>The title case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToTitleCase(string)"/> function.
        /// </remarks>
        public static string ToTitleCase(this string value, bool fromMedialCaps)
        {
            return ToTitleCase(value, fromMedialCaps, ' ');
        }

        /// <summary>
        /// Converts the input string to sentence case. A space (` `) character is used to separate each word.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The sentence case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// Sentence case represents compound words with one intervening space or punctuation. Only the first word begins 
        /// with a capital letter.
        ///
        /// For example, "EndOfFile" will be written as "End of file".
        ///
        /// If you already know that <paramref name="value"/> is in medial capitals (either PascalCase or camelCase), use the 
        /// <see cref="ToSentenceCase(string, bool)"/> function. Typical speed gain is about 2-3 times.
        /// </remarks>
        public static string ToSentenceCase(this string value)
        {
            char[] sepchars = { ' ', '-', '_' };
            bool isMedial = true;
            foreach (char c in sepchars)
            {
                if (value.IndexOf(c) != -1)
                {
                    isMedial = false;
                    break;
                }
            }
            
            return ToSentenceCase(value, isMedial, ' ');
        }

        /// <summary>
        /// Converts the input string to sentence case. A space (` `) character is used to separate each word.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <returns>The sentence case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToSentenceCase(string)"/> function.
        /// </remarks>
        public static string ToSentenceCase(this string value, bool fromMedialCaps)
        {
            return ToSentenceCase(value, fromMedialCaps, ' ');
        }

        /// <summary>
        /// Converts the input string to title case.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <param name="separatorChar">The character to use as separator between words.</param>
        /// <returns>The title case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToTitleCase(string)"/> function.
        /// </remarks>
        [SecuritySafeCritical]
        public unsafe static string ToTitleCase(this string value, bool fromMedialCaps, char separatorChar)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            fixed (char* p = value)
            {
                char* buffer = stackalloc char[(fromMedialCaps ? (value.Length * 2 - 1) : value.Length)];
                int c = 0;
                char* ptr = p;
                bool newWord = true;

                pc:
                char f = *ptr;
                if (f == ' ' || f == '_' || f == '-')
                {
                    newWord = false;
                }
                else
                {
                    newWord = true;
                    
                    if (c >= 0 && f >= 97 && f <= 122)
                        *(buffer++) = (char)(f - 32);
                    else
                        *(buffer++) = *ptr;
                    ++c;
                    ptr++;
                }

                while ((f = *(ptr++)) != '\0')
                {
                    if (f == ' ' || f == '_' || f == '-')
                    {
                        if (newWord)
                        {
                            *(buffer++) = ' ';
                            ++c;
                        }
                        goto pc;
                    }
                    
                    if (!fromMedialCaps && f >= 65 && f <= 90)
                    {
                        *(buffer++) = (char)(f + 32);
                    }
                    else if (fromMedialCaps && f >= 65 && f <= 90)
                    {
                        *(buffer++) = separatorChar;
                        ++c;
                        *(buffer++) = (char)(f + 32);
                    }
                    else
                    {
                        *(buffer++) = f;
                    }
                        
                    ++c;
                }
                buffer -= c;

                return new string(buffer, 0, c);
            }
        }

        /// <summary>
        /// Converts the input string to sentence case.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <param name="fromMedialCaps">If <paramref name="value"/> is already in either PascalCase or camelCase, `true`. Otherwise, `false`.</param>
        /// <param name="separatorChar">The character to use as separator between words.</param>
        /// <returns>The sentence case equivilance of <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function requires you to manually specify whether <paramref name="value"/> is in medial capitals. If automatic detection is desired, 
        /// use the <see cref="ToSentenceCase(string)"/> function.
        /// </remarks>
        [SecuritySafeCritical]
        public unsafe static string ToSentenceCase(this string value, bool fromMedialCaps, char separatorChar)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            fixed (char* p = value)
            {
                char* buffer = stackalloc char[(fromMedialCaps ? (value.Length * 2 - 1) : value.Length)];
                int c = 0;
                char* ptr = p;
                bool newWord = true;

                pc:
                char f = *ptr;
                if (f == ' ' || f == '_' || f == '-')
                {
                    newWord = false;
                }
                else
                {
                    newWord = true;
                    
                    if (c == 0 && f >= 97 && f <= 122)
                        *(buffer++) = (char)(f - 32);
                    else if (c > 0 && f >= 65 && f <= 90)
                        *(buffer++) = (char)(f + 32);
                    else
                        *(buffer++) = *ptr;

                    ++c;
                    ptr++;
                }

                while ((f = *(ptr++)) != '\0')
                {
                    if (f == ' ' || f == '_' || f == '-')
                    {
                        if (newWord)
                        {
                            *(buffer++) = separatorChar;
                            ++c;
                        }
                        goto pc;
                    }
                    
                    if (!fromMedialCaps && f >= 65 && f <= 90)
                    {
                        *(buffer++) = (char)(f + 32);
                    }
                    else if (fromMedialCaps && f >= 65 && f <= 90)
                    {
                        *(buffer++) = ' ';
                        ++c;
                        *(buffer++) = (char)(f + 32);
                    }
                    else
                    {
                        *(buffer++) = f;
                    }
                        
                    ++c;
                }
                buffer -= c;

                return new string(buffer, 0, c);
            }
        }

        /// <summary>
        /// Converts the first alphabetic character to upper casing.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The value of <paramref name="value"/>, with the first alphanetic character converted to upper casing where required.</returns>
        /// <remarks>
        /// Alphabetic character here means character from ANSI code 97 to 122 ('a' through 'z').
        /// </remarks>
        [SecuritySafeCritical]      
        public static unsafe string FirstToUpperInvariant(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

#if NETSTANDARD
            // doesn't seem to have a faster way to copy string in NetCore...perhaps 2.0 will fix this.
            string ret = new string(value.ToCharArray());
#else
            string ret = string.Copy(value);
#endif
            fixed (char* p = ret) 
            {
                char* ptr = p;
                if (*ptr >= 97 && *ptr <= 122)
                {
                    // char at position 0 is in lower case. flip it and return.
                    *ptr = (char)(*ptr - 32);
                    return ret;
                }
                else if (*ptr >= 65 && *ptr <= 90)
                {
                    // char at position 0 is in upper case. nothing to do.
                    return ret;
                }

                while (*(ptr++) != '\0')
                {
                    if (*ptr >= 97 && *ptr <= 122)
                    {
                        // first occurance of alphabet is in lower case. flip it to upper case
                        *ptr = (char)(*ptr - 32);
                        break;
                    }
                    else if (*ptr >= 65 && *ptr <= 90)
                    {
                        // first occurance of alphabet already in upper case
                        break;
                    }
                    ptr++;
                }
            }
            return ret;
        }

        /// <summary>
        /// Converts the first alphabetic character to lower casing.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to modify.</param>
        /// <returns>The value of <paramref name="value"/>, with the first alphanetic character converted to lower casing where required.</returns>
        /// <remarks>
        /// Alphabetic character here means character from ANSI code 64 to 90 ('A' through 'Z').
        /// </remarks>
        [SecuritySafeCritical]
        public static unsafe string FirstToLowerInvariant(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

#if NETSTANDARD
            string ret = new string(value.ToCharArray());
#else
            string ret = string.Copy(value);
#endif
            fixed (char* p = ret) 
            {
                char* ptr = p;

                if (*ptr >= 97 && *ptr <= 122)
                {
                    return ret;
                }
                else if (*ptr >= 65 && *ptr <= 90)
                {
                    *ptr = (char)(*ptr + 32);
                    return ret;
                }

                while (*(ptr++) != '\0')
                {
                    if (*ptr >= 65 && *ptr <= 90)
                    {
                        *ptr = (char)(*ptr + 32);
                        break;
                    }
                    else if (*ptr >= 97 && *ptr <= 122)
                    {
                        break;
                    }

                    ptr++;
                }
            }
            return ret;
        }
    }
}