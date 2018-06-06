using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;

namespace Standard
{
    partial class StringExtension
    {
        //<#
        // .SYNOPSIS
        //      Converts the input string to PascalCase.
        //
        // .PARAMETER value
        //      The input string to be converted to PascalCase
        //
        // .OUTPUT
        //      The pascal case equivilance of @[value].
        //
        // .REMARKS
        //      <template name="ToPascalCaseRemarksShared">
        //      Pascal case (aka. upper camel case, and more formally, upper medial capitals) format represents 
        //      compound words without intervening spaces or punctuations. Instead, each word begins with a 
        //      capital letter.
        //
        //      For example, "John Smith" will be written as "JohnSmith", and "End-of-File" will be written as 
        //      "EndOfFile".
        //
        //      Pascal case is distinct from lower camel case, where the first letter is in lower case, such as 
        //      "johnSmith" and "endOfFile". For lower camel case conversion, use the @"ToCamelCase(string)"
        //      function.
        //      </template>
        //
        //      <template name="MedialCaseDetectionRemark">
        //      This function detect medial capitals (which consists of both pascal and lower camel case) internally
        //      by testing for the presence of the following characters: (space), `-`, `_`. If any of these characters 
        //      are detected, it is regarded as non-medial capital.
        //      </template>
        //
        //      For performance sensitive operations, you can use the function @"ToPascalCase(string, bool)" to 
        //      specify the format manually. Typical speed gain is about 2 to 3 times.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to PascalCase.
        //
        // .PARAMETER value
        //      The input string to be converted to PascalCase
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .OUTPUT
        //      The pascal case equivilance of @[value].
        //
        // .REMARKS
        //      <include template="ToPascalCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToPascalCase(string)" function.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to camelCase.
        //
        // .PARAMETER value
        //      The input string to be converted to camelCase
        //
        // .OUTPUT
        //      The camel case equivilance of @[value].
        //
        // .REMARKS
        //      <template name="ToCamelCaseRemarksShared">
        //      Camel case (aka. lower camel case, and more formally, lower medial capitals) format represents 
        //      compound words without intervening spaces or punctuations. Instead, each word begins with a 
        //      capital letter, except for the first letter, which must be in lower casing.
        //
        //      For example, "John Smith" will be written as "johnSmith", and "End-of-File" will be written as 
        //      "endOfFile".
        //
        //      Lower camel case is distinct from upper camel case or pascal case, where the first letter is in 
        //      upper case, such as "JohnSmith" and "EndOfFile". For pascal case conversion, use the 
        //      @"ToPascalCase(string)" function.
        //      </template>
        //
        //      <include template="MedialCaseDetectionRemark" />
        //
        //      For performance sensitive operations, you can use the function @"ToCamelCase(string, bool)" to 
        //      specify the format manually. Typical speed gain is about 2 to 3 times.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to camelCase.
        //
        // .PARAMETER value
        //      The input string to be converted to camelCase.
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .OUTPUT
        //      The camel case equivilance of @[value].
        //
        // .REMARKS
        //      <include template="ToCamelCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToCamelCase(string)" function.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to TitleCase.
        //
        // .PARAMETER value
        //      The input string to be converted to title case.
        //
        // .OUTPUT
        //      The title case equivilance of @[value], using space as separator.
        //
        // .REMARKS
        //      <template name="ToTitleCaseRemarksShared">
        //      Title case represents compound words with one intervening space or punctuation. Each word begins with a 
        //      capital letter.
        //
        //      For example, "EndOfFile" will be written as "End Of File".
        //      </template>
        //
        //      <include template="MedialCaseDetectionRemark" />
        //
        //      For performance sensitive operations, you can use the function @"ToTitleCase(string, bool)" to 
        //      specify the format manually. Typical speed gain is about 2 to 3 times.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to TitleCase.
        //
        // .PARAMETER value
        //      The input string to be converted to title case.
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .OUTPUT
        //      The title case equivilance of @[value], using space as separator.
        //
        // .REMARKS
        //      <include template="ToTitleCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToTitleCase(string)" function.
        //#>
        public static string ToTitleCase(this string value, bool fromMedialCaps)
        {
            return ToTitleCase(value, fromMedialCaps, ' ');
        }

        //<#
        // .SYNOPSIS
        //      Converts the input string to sentence case.
        //
        // .PARAMETER value
        //      The input string to be converted to sentence case.
        //
        // .OUTPUT
        //      The sentence case equivilance of @[value], using space as separator.
        //
        // .REMARKS
        //      <template name="ToSentenceCaseRemarksShared">
        //      Sentence case represents compound words with one intervening space or punctuation. Only the first word begins 
        //      with a capital letter.
        //
        //      For example, "EndOfFile" will be written as "End of file".
        //      </template>
        //
        //      <include template="MedialCaseDetectionRemark" />
        //
        //      For performance sensitive operations, you can use the function @"ToSentenceCase(string, bool)" to 
        //      specify the format manually. Typical speed gain is about 2 to 3 times.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to sentence case.
        //
        // .PARAMETER value
        //      The input string to be converted to sentence case.
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .OUTPUT
        //      The sentence case equivilance of @[value], using space as separator.
        //
        // .REMARKS
        //      <include template="ToSentenceCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToSentenceCase(string)" function.
        //#>
        public static string ToSentenceCase(this string value, bool fromMedialCaps)
        {
            return ToSentenceCase(value, fromMedialCaps, ' ');
        }

        //<#
        // .SYNOPSIS
        //      Converts the input string to TitleCase.
        //
        // .PARAMETER value
        //      The input string to be converted to title case.
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .PARAMETER separatorChar
        //      The character to use as separator between words.
        //
        // .OUTPUT
        //      The title case equivilance of @[value].
        //
        // .REMARKS
        //      <include template="ToTitleCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToTitleCase(string)" function.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the input string to sentence case.
        //
        // .PARAMETER value
        //      The input string to be converted to sentence case.
        //
        // .PARAMETER fromMedialCaps
        //      `true` if @[value] is in either PascalCase or camelCase, otherwise `false`.
        //
        // .PARAMETER separatorChar
        //      The character to use as separator between words.
        //
        // .OUTPUT
        //      The sentence case equivilance of @[value].
        //
        // .REMARKS
        //      <include template="ToSentenceCaseRemarksShared" />
        //
        //      This function requires you to manually specify whether the input @[value] is in medial capitals. If 
        //      automatic detection is desired, use the @"ToSentenceCase(string)" function.
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the first alphabetic character to upper casing.
        //
        // .PARAMETER value
        //      The input string to be converted.
        //
        // .OUTPUT
        //      The same as @[value], but with the first alphabetic character converted to upper casing.
        //
        // .REMARKS
        //      Alphabetic character here means character from ANSI code 97 to 122 ('a' through 'z').
        //#>
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

        //<#
        // .SYNOPSIS
        //      Converts the first alphabetic character to lower casing.
        //
        // .PARAMETER value
        //      The input string to be converted.
        //
        // .OUTPUT
        //      The same as @[value], but with the first alphabetic character converted to lower casing.
        //
        // .REMARKS
        //      Alphabetic character here means character from ANSI code 64 to 90 ('A' through 'Z').
        //#>
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