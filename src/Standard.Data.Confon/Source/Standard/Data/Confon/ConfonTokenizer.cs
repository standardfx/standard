using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class contains methods used to tokenize a string.
    /// </summary>
    public class Tokenizer
    {
        private readonly string _text;
        private int _index;
        private readonly Stack<int> _indexStack = new Stack<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="text">The string that contains the text to tokenize.</param>
        public Tokenizer(string text)
        {
            this._text = text;
        }

        public int Length
        {
            get
            {
                return _text.Length;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public void Push()
        {
            _indexStack.Push(_index);
        }

        public void Pop()
        {
            _index = _indexStack.Pop();
        }

        /// <summary>
        /// A value indicating whether the tokenizer has reached the end of the string.
        /// </summary>
        public bool EOF
        {
            get { return _index >= _text.Length; }
        }

        /// <summary>
        /// Determines whether the given pattern matches the value at the current
        /// position of the tokenizer.
        /// </summary>
        /// <param name="pattern">The string that contains the characters to match.</param>
        /// <returns><c>true</c> if the pattern matches, otherwise <c>false</c>.</returns>
        public bool Matches(string pattern)
        {
            if (pattern.Length + _index > _text.Length)
                return false;

            //Aaron: added this to make it easier to set a breakpoint to debug config issues
            string selected = _text.Substring(_index, pattern.Length);
            if (selected == pattern)
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves a string of the given length from the current position of the tokenizer.
        /// </summary>
        /// <param name="length">The length of the string to return.</param>
        /// <returns>
        /// The string of the given length. If the length exceeds where the
        /// current index is located, then null is returned.
        /// </returns>
        public string Take(int length)
        {
            if (_index + length > _text.Length)
                return null;

            string s = _text.Substring(_index, length);
            _index += length;
            return s;
        }

        /// <summary>
        /// Determines whether any of the given patterns match the value at the current
        /// position of the tokenizer.
        /// </summary>
        /// <param name="patterns">The string array that contains the characters to match.</param>
        /// <returns><c>true</c> if any one of the patterns match, otherwise <c>false</c>.</returns>
        public bool Matches(params string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                if (pattern.Length + _index >= _text.Length)
                    continue;

                if (_text.Substring(_index, pattern.Length) == pattern)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves the next character in the tokenizer without advancing its position.
        /// </summary>
        /// <returns>The character at the tokenizer's current position.</returns>
        public char Peek()
        {
            if (EOF)
                return (char)0;

            return _text[_index];
        }

        /// <summary>
        /// Retrieves the next character in the tokenizer.
        /// </summary>
        /// <returns>The character at the tokenizer's current position.</returns>
        public char Take()
        {
            if (EOF)
                return (char)0;

            return _text[_index++];
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

        public string GetHelpTextAtIndex(int index, int length = 0)
        {
            if (length == 0)
                length = Length - index;

            int l = Math.Min(20, length);

            string snippet = _text.Substring(index, l);
            if (length > l)
                snippet = snippet + "...";

            //escape snippet
            snippet = snippet.Replace("\r", "\\r").Replace("\n", "\\n");

            return string.Format("#{0}: `{1}`", index, snippet);
        }
    }

    /// <summary>
    /// This class contains methods used to tokenize Confon strings.
    /// </summary>
    public class ConfonTokenizer : Tokenizer
    {
        private const string NotInUnquotedKey = "$\"{}'[]:=,#`^?!@*&\\.";
        private const string NotInUnquotedText = "$\"{}'[]:=,#`^?!@*&\\";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonTokenizer"/> class.
        /// </summary>
        /// <param name="text">The string that contains the text to tokenize.</param>
        public ConfonTokenizer(string text)
            : base(text)
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
        /// Retrieves the current line from where the current token
        /// is located in the string.
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
        /// Retrieves the next token from the string.
        /// </summary>
        /// <returns>The next token contained in the string.</returns>
        /// <exception cref="System.FormatException">
        /// This exception is thrown when an unknown token is encountered.
        /// </exception>
        public ConfonToken PullNext()
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
                return new ConfonToken(TokenType.EOF, Index, 0);

            throw new ConfonTokenizerException(string.Format(RS.ErrUnknownToken, GetHelpTextAtIndex(start)));
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
        /// Retrieves a <see cref="TokenType.ArrayEnd"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ArrayEnd"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullArrayEnd()
        {
            int start = Index;
            if (!IsArrayEnd())
                throw new ConfonTokenizerException(string.Format("Expected end of array {0}", GetHelpTextAtIndex(start)));

            Take();
            return new ConfonToken(TokenType.ArrayEnd, start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ArrayEnd"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsArrayEnd()
        {
            return Matches("]");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ArrayStart"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsArrayStart()
        {
            return Matches("[");
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.ArrayStart"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ArrayStart"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullArrayStart()
        {
            int start = Index;
            Take();
            return new ConfonToken(TokenType.ArrayStart, Index, Index - start);
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.Dot"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Dot"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullDot()
        {
            int start = Index;
            Take();
            return new ConfonToken(TokenType.Dot, start, Index - start);
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.Comma"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Comma"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullComma()
        {
            int start = Index;
            Take();
            return new ConfonToken(TokenType.Comma, start, Index - start);
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.ObjectStart"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ObjectStart"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullStartOfObject()
        {
            int start = Index;
            Take();
            return new ConfonToken(TokenType.ObjectStart, start, Index - start);
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.ObjectEnd"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.ObjectEnd"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullObjectEnd()
        {
            int start = Index;
            if (!IsObjectEnd())
                throw new ConfonTokenizerException(string.Format(RS.ErrExpectEndOfObject, GetHelpTextAtIndex(Index)));

            Take();
            return new ConfonToken(TokenType.ObjectEnd, start, Index - start);
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.Assign"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Assign"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullAssignment()
        {
            int start = Index;
            Take();
            return new ConfonToken(TokenType.Assign, start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Comma"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsComma()
        {
            return Matches(",");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Dot"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsDot()
        {
            return Matches(".");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ObjectStart"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsObjectStart()
        {
            return Matches("{");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.ObjectEnd"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsObjectEnd()
        {
            return Matches("}");
        }

        /// <summary>
        /// Determines whether the current token matches an <see cref="TokenType.Assign"/> token.
        /// </summary>
        /// <returns><c>true</c> if the token matches; otherwise, <c>false</c>.</returns>
        public bool IsAssignment()
        {
            return Matches("=", ":");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a single quoted string.
        /// </summary>
        /// <returns><c>true</c> if token matches; otherwise, <c>false</c>.</returns>
        public bool IsStartOfSingleQuotedText()
        {
            return Matches("'");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a quoted string.
        /// </summary>
        /// <returns><c>true</c> if token matches; otherwise, <c>false</c>.</returns>
        public bool IsStartOfQuotedText()
        {
            return Matches("\"");
        }

        /// <summary>
        /// Determines whether the current token matches the start of a triple quoted string.
        /// </summary>
        /// <returns><c>true</c> if token matches; otherwise, <c>false</c>.</returns>
        public bool IsStartOfTripleQuotedText()
        {
            return Matches("\"\"\"");
        }

        /// <summary>
        /// Retrieves a <see cref="TokenType.Comment"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Comment"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullComment()
        {
            int start = Index;
            PullRestOfLine();
            return new ConfonToken(TokenType.Comment, start, Index - start);
        }

        /// <summary>
        /// Retrieves an unquoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullUnquotedKey()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (!EOF && IsUnquotedKey())
            {
                sb.Append(Take());
            }

            return ConfonToken.Key((sb.ToString().Trim()), start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token is an unquoted key.
        /// </summary>
        /// <returns><c>true</c> if token is an unquoted key; otherwise, <c>false</c>.</returns>
        public bool IsUnquotedKey()
        {
            return (!EOF && !IsStartOfComment() && !NotInUnquotedKey.Contains(Peek()));
        }

        /// <summary>
        /// Determines whether the current token is the start of an unquoted key.
        /// </summary>
        /// <returns><c>true</c> if token is the start of an unquoted key; otherwise, <c>false</c>.</returns>
        public bool IsUnquotedKeyStart()
        {
            return (!EOF && !IsWhitespace() && !IsStartOfComment() && !NotInUnquotedKey.Contains(Peek()));
        }

        public bool IsWhitespace()
        {
            return char.IsWhiteSpace(Peek());
        }

        public bool IsWhitespaceOrComment()
        {
            return IsWhitespace() || IsStartOfComment();
        }

        /// <summary>
        /// Retrieves a triple quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullTripleQuotedText()
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
                throw new ConfonTokenizerException(string.Format(RS.ErrExpectEndOfTripleQuote, GetHelpTextAtIndex(Index)));

            Take(3);
            return ConfonToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Retrieves a quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullQuotedText()
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
                throw new ConfonTokenizerException(string.Format(RS.ErrExpectEndOfQuote, GetHelpTextAtIndex(Index)));
            
            Take();
            return ConfonToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Retrieves a quoted <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.LiteralValue"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullSingleQuotedText()
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
                throw new ConfonTokenizerException(string.Format(RS.ErrExpectEndOfQuote, GetHelpTextAtIndex(Index)));
            
            Take();
            return ConfonToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Retrieves a quoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullQuotedKey()
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
            return ConfonToken.Key(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Retrieves a single quoted <see cref="TokenType.Key"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Key"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullSingleQuotedKey()
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
            return ConfonToken.Key(sb.ToString(), start, Index - start);
        }

        public ConfonToken PullInclude()
        {
            int start = Index;
            Take("include".Length);
            PullWhitespaceAndComments();
            ConfonToken rest = PullQuotedText();
            string unQuote = rest.Value;
            return ConfonToken.Include(unQuote, start, Index - start);
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
                    throw new ConfonTokenizerException(string.Format(RS.ErrUnknownEscapeCode, escaped, GetHelpTextAtIndex(start)));
            }
        }

        public bool IsStartOfComment()
        {
            return Matches("#", "//");
        }

        /// <summary>
        /// Retrieves a value token from the tokenizer's current position.
        /// </summary>
        /// <returns>A value token from the tokenizer's current position.</returns>
        /// <exception cref="System.FormatException">
        /// Expected value: Null literal, Array, Quoted Text, Unquoted Text, Triple quoted Text, Object or End of array
        /// </exception>
        public ConfonToken PullValue()
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

            throw new ConfonTokenizerException(string.Format(RS.ErrUnexpectedValue, "null, array, quoted text, unquoted text, triple quoted text, object, end of array", GetHelpTextAtIndex(start)));
        }

        /// <summary>
        /// Determines whether the current token is the start of a substitution.
        /// </summary>
        /// <returns><c>true</c> if token is the start of a substitution; otherwise, <c>false</c>.</returns>
        public bool IsSubstitutionStart()
        {
            return Matches("${");
        }

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
        /// Retrieves a <see cref="TokenType.Substitute"/> token from the tokenizer's current position.
        /// </summary>
        /// <returns>A <see cref="TokenType.Substitute"/> token from the tokenizer's current position.</returns>
        public ConfonToken PullSubstitution()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            Take(2);
            while (!EOF && IsUnquotedText())
            {
                sb.Append(Take());
            }
            Take();
            return ConfonToken.Substitution(sb.ToString(), start, Index - start);
        }

        /// <summary>
        /// Determines whether the current token is a space or a tab.
        /// </summary>
        /// <returns><c>true</c> if token is the start of a space or a tab; otherwise, <c>false</c>.</returns>
        public bool IsSpaceOrTab()
        {
            return Matches(" ", "\t");
        }

        /// <summary>
        /// Determines whether the current token is the start of an unquoted string literal.
        /// </summary>
        /// <returns><c>true</c> if token is the start of an unquoted string literal; otherwise, <c>false</c>.</returns>
        public bool IsStartSimpleValue()
        {
            if (IsSpaceOrTab())
                return true;

            if (IsUnquotedText())
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves the current token, including whitespace and tabs, as a string literal token.
        /// </summary>
        /// <returns>A token that contains the string literal value.</returns>
        public ConfonToken PullSpaceOrTab()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (IsSpaceOrTab())
            {
                sb.Append(Take());
            }
            return ConfonToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        private ConfonToken PullUnquotedText()
        {
            int start = Index;
            StringBuilder sb = new StringBuilder();
            while (!EOF && IsUnquotedText())
            {
                sb.Append(Take());
            }

            return ConfonToken.LiteralValue(sb.ToString(), start, Index - start);
        }

        private bool IsUnquotedText()
        {
            return (!EOF && !IsWhitespace() && !IsStartOfComment() && !NotInUnquotedText.Contains(Peek()));
        }

        /// <summary>
        /// Retrieves the current token as a string literal token.
        /// </summary>
        /// <returns>A token that contains the string literal value.</returns>
        /// <exception cref="System.FormatException">
        /// This exception is thrown when the tokenizer cannot find
        /// a string literal value from the current token.
        /// </exception>
        public ConfonToken PullSimpleValue()
        {
            int start = Index;
            if (IsSpaceOrTab())
                return PullSpaceOrTab();
            if (IsUnquotedText())
                return PullUnquotedText();

            throw new ConfonTokenizerException(string.Format(RS.ErrExpectSimpleValue, GetHelpTextAtIndex(start)));
        }

        /// <summary>
        /// Determines whether the current token is a value.
        /// </summary>
        /// <returns><c>true</c> if the current token is a value; otherwise, <c>false</c>.</returns>
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
