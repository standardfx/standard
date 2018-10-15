using System;
using System.Text;
using System.Collections.Generic;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// Utility class for parsing a Json+ path.
    /// </summary>
    /// <remarks><![CDATA[
    /// You can select a Json+ element using path query expressions. A Json+ path consists of dot separated keys. 
    /// However, if the key itself contains a dot, that key must be surrounded with double or single quotes. If 
    /// the key also contains single or double quote characters, it must also be escaped with the backslash 
    /// character.
    /// 
    /// For example, the expression `foo.bar.tar` means nested elements in the structure `foo` -> `bar` -> `tar`.
    ///
    /// The expression `foo."b.ar".tar` means nested elements in the structure `foo` -> `b.ar` -> `tar`.
    ///
    /// The expression `foo."b.a'r".tar` means nested elements in the structure `foo` -> `b.a'r` -> `tar`. You can 
    /// also use single quotes around the element `b.a'r`, but the single quote inside must be escaped: 
    /// 
    /// ```json+
    //  foo.'b.a\'r'.tar
    /// ```
    /// 
    /// Note that the backslash symbol is only interpreted as an escape sequence if the character immediately 
    /// after it is a quote character that must be escaped in the context. For example, `'b.a\\r'` is interpreted 
    /// as `b.a\\r`, and `'b.a.\"r'` is interpreted as `b.a.\"r`.
    /// ]]></remarks>
    public class JPlusPath
    {
        private readonly string _pathStr;
        private string[] _pathElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusPath"/> class.
        /// </summary>
        /// <param name="pathStr">Query path to a Json+ element.</param>
        public JPlusPath(string pathStr)
        {
            _pathStr = pathStr;
        }

        /// <summary>
        /// Split a Json+ query path into its consituting parts.
        /// </summary>
        /// <returns>A string array consisting of the query path items.</returns>
        public string[] AsArray()
        {
            return _pathElements ?? (_pathElements = Parse(_pathStr));
        }

        private static string[] Parse(string input)
        {
            // just do a string.split if path does not have quote chars inside
            if (!(input.Contains("'") || input.Contains("\"")))
                return input.Split('.');

            List<string> parsed = new List<string>(); // holds output
            StringBuilder current = new StringBuilder(); // holds a key
            char quoteMatch = '.'; // dot means key is not quoted, or " or ' to indicate quote char used.
            bool matching = false; // true if parsing the first token, or if the previous token is a separator.

            // path expression cannot end or start with dot
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            else if (input.StartsWith("."))
                throw new FormatException(RS.Err_PathCannotStartEndWithSeparator);
            else if (input.EndsWith("."))
                throw new FormatException(RS.Err_PathCannotStartEndWithSeparator);

            // no dots inside expression, so one big quote
            // "bar' => untouched
            // "bar" => bar
            if (input.IndexOf(".") == -1)
            {
                if ((input.StartsWith("'") && input.EndsWith("'")) ||
                    (input.StartsWith("\"") && input.EndsWith("\"")))
                {
                    parsed.Add(input.Substring(1, input.Length - 2));
                }
                else
                {
                    parsed.Add(input);
                }
                return parsed.ToArray();
            }

            for (int i = 0; i < input.Length; i++)
            {
                char token = input[i];

                if (matching == false)
                {
                    if (token == '.')
                    {
                        // first char after a . cannot be another another .
                        // foo..bar
                        // exception foo."a.b".bar -> ". is an exception
                        if ((i != 0) && (quoteMatch == '"' || quoteMatch == '\'') && (input[i - 1] == quoteMatch))
                            continue;
                        else
                            throw new FormatException(string.Format(RS.Err_PathCannotHaveDoubleSeparator, input));
                    }

                    // EOF. Just add the token as the last member.
                    if (i == input.Length - 1)
                    {
                        parsed.Add(token.ToString());
                        continue;
                    }

                    // if token is a quote char, set quoteMatch
                    // otherwise consume token
                    if (token == '"' || token == '\'')
                    {
                        quoteMatch = token;
                        matching = true;
                        continue; // advance to next token
                    }
                    else
                    {
                        quoteMatch = '.';
                        current.Append(token); // current already cleared.
                        matching = true;
                        continue;
                    }
                }
                else
                {
                    // EOF. Consume token and add current as last member.
                    // token == quoteMatch logic at the end.
                    if ((i == input.Length - 1) && (token != quoteMatch))
                    {
                        current.Append(token);
                        parsed.Add(current.ToString());
                        current.Clear();
                        matching = false;
                        continue;
                    }

                    if ((quoteMatch != '.') && (token == '\\'))
                    {
                        // look ahead to check if \ is an escape symbol

                        token = input[i + 1];
                        if (token == quoteMatch)
                        {
                            current.Append(token);
                            i += 1; // advance 2 tokens

                            // will advance 2 tokens take us to EOF?
                            if (i == input.Length - 1)
                            {
                                parsed.Add(current.ToString());
                                current.Clear();
                                matching = false;
                            }
                            continue;
                        }
                        else
                        {
                            current.Append('\\');
                            continue;
                        }
                    }
                    else if (token == quoteMatch)
                    {
                        // we've matched the starting token.
                        // add current as member, clear it and exit match
                        
                        parsed.Add(current.ToString());
                        current.Clear();
                        matching = false;
                        continue;
                    }
                    else
                    {
                        current.Append(token);
                        continue;
                    }
                }
            }

            return parsed.ToArray();
        }
    }
}
