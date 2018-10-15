using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class contains methods used to tokenize a string.
    /// </summary>
    public class Tokenizer
    {
        private readonly string _source;
        private int _index;
        private readonly Stack<int> _indexStack = new Stack<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="source">The string that contains the text to tokenize.</param>
        public Tokenizer(string source)
        {
            this._source = source;
        }

        /// <summary>
        /// Gets the length of the source string.
        /// </summary>
        public int Length
        {
            get { return _source.Length; }
        }

        /// <summary>
        /// Gets the current stack position.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Enters a stack scope.
        /// </summary>
        public void Push()
        {
            _indexStack.Push(_index);
        }

        /// <summary>
        /// Exit the current stack scope.
        /// </summary>
        public void Pop()
        {
            _index = _indexStack.Pop();
        }

        /// <summary>
        /// Indicates whether the tokenizer has reached the end of the string.
        /// </summary>
        public bool EOF
        {
            get
            {
                return _index >= _source.Length;
            }
        }

        /// <summary>
        /// Determines whether the pattern specified matches the value at the current
        /// position of the tokenizer.
        /// </summary>
        /// <param name="pattern">The string that contains the characters to match.</param>
        /// <returns>`true` if the pattern matches. Otherwise, `false`.</returns>
        public bool Matches(string pattern)
        {
            if (pattern.Length + _index > _source.Length)
                return false;

            // added this to make it easier to set a breakpoint to debug config issues
            string selected = _source.Substring(_index, pattern.Length);
            if (selected == pattern)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether any of the given patterns match the value at the current
        /// position of the tokenizer.
        /// </summary>
        /// <param name="patterns">The string array that contains the characters to match.</param>
        /// <returns>`true` if any one of the patterns match. Otherwise, `false`.</returns>
        public bool Matches(params string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                if (pattern.Length + _index >= _source.Length)
                    continue;

                if (_source.Substring(_index, pattern.Length) == pattern)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a string of the given length from the current position of the tokenizer.
        /// </summary>
        /// <param name="length">The length of the string to return.</param>
        /// <returns>The string of the given length. If the length exceeds where the current index is located, `null` is returned.</returns>
        public string Take(int length)
        {
            if (_index + length > _source.Length)
                return null;

            string s = _source.Substring(_index, length);
            _index += length;
            return s;
        }

        /// <summary>
        /// Returns the next character in the tokenizer without advancing the tokenizer's position.
        /// </summary>
        /// <returns>The character at the tokenizer's current position.</returns>
        public char Peek()
        {
            if (EOF)
                return (char)0;

            return _source[_index];
        }

        /// <summary>
        /// Returns the next character in the tokenizer and advance the tokenizer position by 1.
        /// </summary>
        /// <returns>The character at the tokenizer's current position.</returns>
        public char Take()
        {
            if (EOF)
                return (char)0;

            return _source[_index++];
        }

        /// <summary>
        /// Advances the tokenizer to the next non-whitespace character.
        /// </summary>
        public void PullWhitespace()
        {
            while (!EOF && char.IsWhiteSpace(Peek()))
            {
                Take();
            }
        }

        /// <summary>
        /// Returns a message consisting of a substring of the source text. This message is used for debug purposes.
        /// </summary>
        /// <param name="index">The index position in the source text where the output substring should be selected.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>A substring in the source text, formatted for debugging purposes.</returns>
        public string GetHelpTextAtIndex(int index, int length = 0)
        {
            if (length == 0)
                length = Length - index;

            int l = Math.Min(20, length);

            string snippet = _source.Substring(index, l);
            if (length > l)
                snippet = snippet + "...";

            //escape snippet
            snippet = snippet.Replace("\r", "\\r").Replace("\n", "\\n");

            return string.Format("#{0}: `{1}`", index, snippet);
        }
    }

    /// <summary>
    /// This class contains methods used to tokenize Json+ strings.
    /// </summary>
    public class JPlusTokenizer : Tokenizer
    {
        private const string NotInUnquotedKey = "$\"{}'[]:=,#`^?!@*&\\.";
        private const string NotInUnquotedText = "$\"{}'[]:=,#`^?!@*&\\";

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusTokenizer"/> class.
        /// </summary>
        /// <param name="source">The string that contains the text to tokenize.</param>
        public JPlusTokenizer(string source)
            : base(source)
        {
        }

        /// <summary>
        /// Advances the tokenizer to the next non-whitespace, non-comment token.
        /// </summary>
        public void PullWhitespaceAndComments()
        {
            do
            {
                PullWhitespace();
                while (IsStartOfComment())
                {
                    PullComment();
                }
            } while (IsWhitespace());
        }

        /// <summary>
        /// Returns the current line from where the current token is located in the string.
        /// </summary>
        /// <returns>The current line from where the current token is located.</returns>
        public string PullRestOfLine()
        {
            StringBuilder sb = new StringBuilder();
            while (!EOF)
            {
                char c = Take();
                if (c == '\n')
                    break;

                //ignore
                if (c == '\r')
                    continue;

                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Returns the next token from the source text.
        /// </summary>
        /// <exception cref="FormatException">An unknown token is encountered.</exception>
        /// <returns>The next token contained in the source text.</returns>
        public JPlusToken PullNext()
        {
            PullWhitespaceAndComments();
            int start = Index;
            if (IsDot())
                return PullDot();

            if (IsObjectStart())
                return PullStartOfObject();

            if (IsObjectEnd())
                return PullObjectEnd();

            if (IsAssignment())
                return PullAssignment();

            if (IsInclude())
                return PullInclude();

            if (IsStartOfQuotedKey())
                return PullQuotedKey();

            if (IsStartOfSingleQuotedKey())
                return PullSingleQuotedKey();

            if (IsUnquotedKeyStart())
                return PullUnquotedKey();

            if (IsArrayStart())
                return PullArrayStart();

            if (IsArrayEnd())
                return PullArrayEnd();

            if (EOF)
                return new JPlusToken(TokenType.EOF, Index, 0);

            throw new JPlusTokenizerException(string.Format(RS.ErrUnknownToken, GetHelpTextAtIndex(start)));
        }

        private bool IsStartOfQuotedKey()
        {
            return Matches("\"");
        }

        private bool IsStartOfSingleQuotedKey()
        {
            return Matches("'");
        }

        /// <summary>
        /// Returns a <see cref="TokenType.ArrayEnd"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ArrayEnd"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullArrayEnd()
        {
            int start = Index;
            if (!IsArrayEnd())
                throw new JPlusTokenizerException(string.Format(RS.ErrExpectEndOfArray, GetHelpTextAtIndex(start)));

            Take();
            return new JPlusToken(TokenType.ArrayEnd, start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ArrayEnd"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsArrayEnd()
        {
            return Matches("]");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ArrayStart"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsArrayStart()
        {
            return Matches("[");
        }

        /// <summary>
        /// Returns a <see cref="TokenType.ArrayStart"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ArrayStart"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullArrayStart()
        {
            int start = Index;
            Take();
            return new JPlusToken(TokenType.ArrayStart, Index, Index - start);
        }

        /// <summary>
        /// Returns a <see cref="TokenType.Dot"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Dot"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullDot()
        {
            int start = Index;
            Take();
            return new JPlusToken(TokenType.Dot, start, Index - start);
        }

        /// <summary>
        /// Returns a <see cref="TokenType.Comma"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Comma"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullComma()
        {
            int start = Index;
            Take();
            return new JPlusToken(TokenType.Comma, start, Index - start);
        }

        /// <summary>
        /// Returns a <see cref="TokenType.ObjectStart"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ObjectStart"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullStartOfObject()
        {
            int start = Index;
            Take();
            return new JPlusToken(TokenType.ObjectStart, start, Index - start);
        }

        /// <summary>
        /// Returns a <see cref="TokenType.ObjectEnd"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ObjectEnd"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullObjectEnd()
        {
            int start = Index;
            if (!IsObjectEnd())
                throw new JPlusTokenizerException(string.Format(RS.ErrExpectEndOfObject, GetHelpTextAtIndex(Index)));

            Take();
            return new JPlusToken(TokenType.ObjectEnd, start, Index - start);
        }

        /// <summary>
        /// Returns a <see cref="TokenType.Assign"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Assign"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullAssignment()
        {
            int start = Index;
            Take();
            return new JPlusToken(TokenType.Assign, start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Comma"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsComma()
        {
            return Matches(",");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Dot"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsDot()
        {
            return Matches(".");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ObjectStart"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsObjectStart()
        {
            return Matches("{");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ObjectEnd"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsObjectEnd()
        {
            return Matches("}");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Assign"/> token.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsAssignment()
        {
            return Matches("=", ":");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a single quoted string.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsStartOfSingleQuotedText()
        {
            return Matches("'");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a quoted string.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsStartOfQuotedText()
        {
            return Matches("\"");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a triple quoted string.
        /// </summary>
        /// <returns>`true` if the token matches. Otherwise, `false`.</returns>
        public bool IsStartOfTripleQuotedText()
        {
            return Matches("\"\"\"");
        }

        /// <summary>
        /// Returns a <see cref="TokenType.Comment"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Comment"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullComment()
        {
            int start = Index;
            PullRestOfLine();
            return new JPlusToken(TokenType.Comment, start, Index - start);
        }

        /// <summary>
        /// Returns an unquoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullUnquotedKey()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (!EOF && IsUnquotedKey())
            {
                sb.Append(Take());
            }

            return JPlusToken.Key((sb.ToString().Trim()), start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token is an unquoted key.
        /// </summary>
        /// <returns>`true` if the token is an unquoted key. Otherwise, `false`.</returns>
        public bool IsUnquotedKey()
        {
            return (!EOF && !IsStartOfComment() && !NotInUnquotedKey.Contains(Peek()));
        }

        /// <summary>
        /// Determines whether the current token is the start of an unquoted key.
        /// </summary>
        /// <returns>`true` if the token is the start of an unquoted key. Otherwise, `false`.</returns>
        public bool IsUnquotedKeyStart()
        {
            return (!EOF && !IsWhitespace() && !IsStartOfComment() && !NotInUnquotedKey.Contains(Peek()));
        }

        /// <summary>
        /// Determines whether the current token is a whitespace character.
        /// </summary>
        /// <returns>`true` if the token is a whitespace character. Otherwise, `false`.</returns>
        public bool IsWhitespace()
        {
            return char.IsWhiteSpace(Peek());
        }

        /// <summary>
        /// Determines whether the current token is a whitespace character or comment.
        /// </summary>
        /// <returns>`true` if the token is a whitespace character or comment. Otherwise, `false`.</returns>
        public bool IsWhitespaceOrComment()
        {
            return IsWhitespace() || IsStartOfComment();
        }

        /// <summary>
        /// Returns a triple quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullTripleQuotedText()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take(3);
            while (!EOF && !Matches("\"\"\""))
            {
                if (Matches("\\"))
                {
                    sb.Append(PullEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Take();
                }
            }

            if (!Matches("\""))
                throw new JPlusTokenizerException(string.Format(RS.ErrExpectEndOfTripleQuote, GetHelpTextAtIndex(Index)));

            Take(3);
            return JPlusToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Returns a quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullQuotedText()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take();
            while (!EOF && !Matches("\""))
            {
                if (Matches("\\"))
                {
                    sb.Append(PullEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Take();
                }
            }

            if (!Matches("\""))
                throw new JPlusTokenizerException(string.Format(RS.ErrExpectEndOfQuote, GetHelpTextAtIndex(Index)));
            
            Take();
            return JPlusToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Returns a quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullSingleQuotedText()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take();
            while (!EOF && !Matches("'"))
            {
                if (Matches("\\"))
                {
                    sb.Append(PullEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Take();
                }
            }

            if (!Matches("'"))
                throw new JPlusTokenizerException(string.Format(RS.ErrExpectEndOfQuote, GetHelpTextAtIndex(Index)));
            
            Take();
            return JPlusToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Returns a quoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullQuotedKey()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take();
            while (!EOF && !Matches("\""))
            {
                if (Matches("\\"))
                {
                    sb.Append(PullEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Take();
                }
            }
            Take();
            return JPlusToken.Key(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Returns a single quoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullSingleQuotedKey()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take();
            while (!EOF && !Matches("'"))
            {
                if (Matches("\\"))
                {
                    sb.Append(PullEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Take();
                }
            }
            Take();
            return JPlusToken.Key(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Returns an <see cref="TokenType.Include"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>An <see cref="TokenType.Include"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullInclude()
        {
            int start = Index;
            Take("include".Length);
            PullWhitespaceAndComments();
            JPlusToken rest = PullQuotedText();
            string unQuote = rest.Value;
            return JPlusToken.Include(unQuote, start, Index - start);
        }

        private string PullEscapeSequence()
        {
            int start = Index;
            Take(); //consume "\"
            char escaped = Take();
            switch (escaped)
            {
                case '"':
                    return ("\"");
                case '\'':
                    return ("'");
                case '\\':
                    return ("\\");
                case '/':
                    return ("/");
                case 'b':
                    return ("\b");
                case 'f':
                    return ("\f");
                case 'n':
                    return ("\n");
                case 'r':
                    return ("\r");
                case 't':
                    return ("\t");
                case 'u':
                    string hex = "0x" + Take(4);
                    int j = Convert.ToInt32(hex, 16);
#if NETSTANDARD
                    return (j).ToString(CultureInfo.InvariantCulture);
#else
                    return ((char)j).ToString(CultureInfo.InvariantCulture);
#endif
                default:
                    throw new JPlusTokenizerException(string.Format(RS.ErrUnknownEscapeCode, escaped, GetHelpTextAtIndex(start)));
            }
        }

        /// <summary>
        /// Determines whether a comment indicator exists at the tokenizer's current position.
        /// </summary>
        /// <returns>`true` if a comment indicator exists at the tokenizer's current position. Otherwise, `false`.</returns>
        public bool IsStartOfComment()
        {
            return Matches("#", "//");
        }

        /// <summary>
        /// Returns a value token from the tokenizer's current position.
        /// </summary>
        /// <returns>A value token from the tokenizer's current position.</returns>
        /// <exception cref="FormatException">The value token is unsupported. Supported values are: null literal, array, quoted text, unquoted text, triple quoted text, object, end of array.</exception>
        public JPlusToken PullValue()
        {
            int start = Index;
            if (IsObjectStart())
                return PullStartOfObject();

            if (IsStartOfTripleQuotedText())
                return PullTripleQuotedText();

            if (IsStartOfSingleQuotedText())
                return PullSingleQuotedText();

            if (IsStartOfQuotedText())
                return PullQuotedText();

            if (IsUnquotedText())
                return PullUnquotedText();

            if (IsArrayStart())
                return PullArrayStart();

            if (IsArrayEnd())
                return PullArrayEnd();

            if (IsSubstitutionStart())
                return PullSubstitution();

            throw new JPlusTokenizerException(string.Format(RS.ErrUnexpectedValue, "null, array, quoted text, unquoted text, triple quoted text, object, end of array", GetHelpTextAtIndex(start)));
        }

        /// <summary>
        /// Determines whether the current token is the start of a substitution.
        /// </summary>
        /// <returns>`true` if the token is the start of a substitution. Otherwise, `false`.</returns>
        public bool IsSubstitutionStart()
        {
            return Matches("${");
        }

        /// <summary>
        /// Determines whether the current token is an <see cref="TokenType.Include"/> token.
        /// </summary>
        /// <returns>`true` if the token is an <see cref="TokenType.Include"/> token. Otherwise, `false`.</returns>
        public bool IsInclude()
        {
            Push();

            try
            {
                if (Matches("include"))
                {
                    Take("include".Length);

                    if (IsWhitespaceOrComment())
                    {
                        PullWhitespaceAndComments();

                        if (IsStartOfQuotedText())
                        {
                            PullQuotedText();
                            return true;
                        }
                    }
                }
                return false;
            }
            finally
            {
                Pop();
            }
        }

        /// <summary>
        /// Returns a <see cref="TokenType.Substitute"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Substitute"/> token from the tokenizer's current position.</returns>
        public JPlusToken PullSubstitution()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take(2);
            while (!EOF && IsUnquotedText())
            {
                sb.Append(Take());
            }
            Take();
            return JPlusToken.Substitution(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token is a space or a tab.
        /// </summary>
        /// <returns>`true` if the token is a space or tab. Otherwise, `false`.</returns>
        public bool IsSpaceOrTab()
        {
            return Matches(" ", "\t");
        }

        /// <summary>
        /// Determines whether the current token is a space, tab, or the start of an unquoted string literal.
        /// </summary>
        /// <returns>`true` if the token is a space, tab, or the start of an unquoted string literal. Otherwise, `false`.</returns>
        public bool IsStartSimpleValue()
        {
            if (IsSpaceOrTab())
                return true;

            if (IsUnquotedText())
                return true;

            return false;
        }

        /// <summary>
        /// Returns the current token, including whitespace and tabs, as a string literal token.
        /// </summary>
        /// <returns>A token that contains the string literal value.</returns>
        public JPlusToken PullSpaceOrTab()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (IsSpaceOrTab())
            {
                sb.Append(Take());
            }
            return JPlusToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        private JPlusToken PullUnquotedText()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (!EOF && IsUnquotedText())
            {
                sb.Append(Take());
            }

            return JPlusToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        private bool IsUnquotedText()
        {
            return (!EOF && !IsWhitespace() && !IsStartOfComment() && !NotInUnquotedText.Contains(Peek()));
        }

        /// <summary>
        /// Returns the current token as a string literal token.
        /// </summary>
        /// <exception cref="FormatException">The tokenizer is unable to find a string literal value from the current token.</exception>
        /// <returns>A token that contains the string literal value.</returns>
        public JPlusToken PullSimpleValue()
        {
            int start = Index;
            if (IsSpaceOrTab())
                return PullSpaceOrTab();
            if (IsUnquotedText())
                return PullUnquotedText();

            throw new JPlusTokenizerException(string.Format(RS.ErrExpectSimpleValue, GetHelpTextAtIndex(start)));
        }

        /// <summary>
        /// Determines whether the current token is a value.
        /// </summary>
        /// <returns>`true` if the current token is a value. Otherwise, `false`.</returns>
        internal bool IsValue()
        {
            if (IsArrayStart())
                return true;
            if (IsObjectStart())
                return true;
            if (IsStartOfTripleQuotedText())
                return true;
            if (IsSubstitutionStart())
                return true;
            if (IsStartOfSingleQuotedText())
                return true;
            if (IsStartOfQuotedText())
                return true;
            if (IsUnquotedText())
                return true;

            return false;
        }
    }
}
