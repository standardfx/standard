using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class contains methods used to parse Confon strings.
    /// </summary>
    public class ConfonParser
    {
        private readonly List<ConfonSubstitution> _substitutions = new List<ConfonSubstitution>();
        private ConfonTokenizer _reader;
        private ConfonValue _root;
        private Func<string, ConfonRoot> _includeCallback;
        private Stack<string> _diagnosticsStack = new Stack<string>();

        private void PushDiagnostics(string message)
        {
            _diagnosticsStack.Push(message);
        }

        private void PopDiagnostics()
        {
            _diagnosticsStack.Pop();
        }

        public string GetDiagnosticsStackTrace()
        {
            string currentPath = string.Join(string.Empty, _diagnosticsStack.Reverse());
            return string.Format("Path: {0}", currentPath);
        }

        /// <summary>
        /// Parses the supplied Confon string into a root element.
        /// </summary>
        /// <param name="text">The string that contains a Confon string.</param>
        /// <param name="includeCallback">Callback used to resolve includes.</param>
        /// <exception cref="System.Exception">
        /// An unresolved substitution is encountered, or the end of the file has been reached while trying to read a value.
        /// </exception>
        /// <returns>The root element created from the supplied Confon configuration string.</returns>
        public static ConfonRoot Parse(string text, Func<string, ConfonRoot> includeCallback)
        {
            return new ConfonParser().ParseText(text, includeCallback);
        }

        private ConfonRoot ParseText(string text, Func<string, ConfonRoot> includeCallback)
        {
            _includeCallback = includeCallback;
            _root = new ConfonValue();
            _reader = new ConfonTokenizer(text);
            _reader.PullWhitespaceAndComments();
            ParseObject(_root, true, string.Empty);

            ConfonContext c = new ConfonContext(new ConfonRoot(_root, Enumerable.Empty<ConfonSubstitution>()));
            foreach (ConfonSubstitution sub in _substitutions)
            {
                ConfonValue res = c.GetValue(sub.Path);
                if (res == null)
                    throw new ConfonParserException(string.Format(RS.ErrUnresolvedSubstitution, sub.Path));

                sub.ResolvedValue = res;
            }
            return new ConfonRoot(_root, _substitutions);
        }

        private void ParseObject(ConfonValue owner, bool root, string currentPath)
        {
            try
            {
                PushDiagnostics("{");

                if (owner.IsObject())
                {
                    //the value of this KVP is already an object
                }
                else
                {
                    //the value of this KVP is not an object, thus, we should add a new
                    owner.NewValue(new ConfonObject());
                }

                ConfonObject currentObject = owner.GetObject();

                while (!_reader.EOF)
                {
                    ConfonToken t = _reader.PullNext();
                    switch (t.Type)
                    {
                        case TokenType.Include:
                            ConfonRoot included = _includeCallback(t.Value);
                            IEnumerable<ConfonSubstitution> substitutions = included.Substitutions;
                            foreach (ConfonSubstitution substitution in substitutions)
                            {
                                //fixup the substitution, add the current path as a prefix to the substitution path
                                substitution.Path = currentPath + "." + substitution.Path;
                            }
                            _substitutions.AddRange(substitutions);
                            ConfonObject otherObj = included.Value.GetObject();
                            owner.GetObject().Merge(otherObj);

                            break;

                        case TokenType.EOF:
                            if (!string.IsNullOrEmpty(currentPath))
                                throw new ConfonParserException(string.Format(RS.ErrObjectUnexpectedEOF, GetDiagnosticsStackTrace()));

                            break;

                        case TokenType.Key:
                            ConfonValue value = currentObject.GetOrCreateKey(t.Value);

                            string currentKey = t.Value;
                            if (currentKey.IndexOf('.') >= 0) 
                                currentKey = "\"" + currentKey + "\"";
                            string nextPath = currentPath == string.Empty ? currentKey : currentPath + "." + currentKey;
                            
                            ParseKeyContent(value, nextPath);
                            if (!root)
                                return;
                            break;

                        case TokenType.ObjectEnd:
                            return;
                    }
                }
            }
            finally
            {
                PopDiagnostics();
            }
        }

        private void ParseKeyContent(ConfonValue value, string currentPath)
        {
            try
            {
                string last = new ConfonPath(currentPath).AsArray().Last();
                PushDiagnostics(string.Format("{0} = ", last));
                while (!_reader.EOF)
                {
                    ConfonToken t = _reader.PullNext();
                    switch (t.Type)
                    {
                        case TokenType.Dot:
                            ParseObject(value, false, currentPath);
                            return;

                        case TokenType.Assign:
                            if (!value.IsObject())
                            {
                                //if not an object, then replace the value.
                                //if object. value should be merged
                                value.Clear();
                            }
                            ParseValue(value, currentPath);
                            return;

                        case TokenType.ObjectStart:
                            ParseObject(value, true, currentPath);
                            return;
                    }
                }
            }
            finally
            {
                PopDiagnostics();
            }
        }

        /// <summary>
        /// Retrieves the next value token from the tokenizer and appends it
        /// to the supplied element <paramref name="owner"/>.
        /// </summary>
        /// <param name="owner">The element to append the next token.</param>
        /// <param name="currentPath">Current AST path.</param>
        /// <exception cref="System.Exception">End of file reached while trying to read a value</exception>
        public void ParseValue(ConfonValue owner, string currentPath)
        {
            if (_reader.EOF)
                throw new ConfonParserException(RS.ErrUnexpectedEOF);

            _reader.PullWhitespaceAndComments();
            int start = _reader.Index;
            try
            {
                while (_reader.IsValue())
                {
                    ConfonToken t = _reader.PullValue();

                    switch (t.Type)
                    {
                        case TokenType.EOF:
                            break;

                        case TokenType.LiteralValue:
                            // needed to allow for override objects
                            if (owner.IsObject())
                                owner.Clear();

                            LiteralString lit = new LiteralString { Value = t.Value };
                            owner.AppendValue(lit);

                            break;

                        case TokenType.ObjectStart:
                            ParseObject(owner, true, currentPath);
                            break;

                        case TokenType.ArrayStart:
                            ConfonArray arr = ParseArray(currentPath);
                            owner.AppendValue(arr);
                            break;

                        case TokenType.Substitute:
                            ConfonSubstitution sub = ParseSubstitution(t.Value);
                            _substitutions.Add(sub);
                            owner.AppendValue(sub);
                            break;
                    }

                    if (_reader.IsSpaceOrTab())
                        ParseTrailingWhitespace(owner);
                }

                IgnoreComma();
            }
            catch(ConfonTokenizerException tokenizerException)
            {
                throw new ConfonParserException(string.Format("{0}\r{1}", tokenizerException.Message, GetDiagnosticsStackTrace()),tokenizerException);
            }
            finally
            {
                // no value was found, tokenizer is still at the same position
                if (_reader.Index == start)
                    throw new ConfonParserException(string.Format(RS.ErrBadSyntax, _reader.GetHelpTextAtIndex(start), GetDiagnosticsStackTrace()));
            }
        }

        private void ParseTrailingWhitespace(ConfonValue owner)
        {
            ConfonToken ws = _reader.PullSpaceOrTab();

            //single line ws should be included if string concat
            if (ws.Value.Length > 0)
            {
                LiteralString wsLit = new LiteralString { Value = ws.Value, };
                owner.AppendValue(wsLit);
            }
        }

        private static ConfonSubstitution ParseSubstitution(string value)
        {
            return new ConfonSubstitution(value);
        }

        /// <summary>
        /// Retrieves the next array token from the tokenizer.
        /// </summary>
        /// <returns>An array of elements retrieved from the token.</returns>
        public ConfonArray ParseArray(string currentPath)
        {
            try
            {
                PushDiagnostics("[");

                ConfonArray arr = new ConfonArray();
                while (!_reader.EOF && !_reader.IsArrayEnd())
                {
                    ConfonValue v = new ConfonValue();
                    ParseValue(v, currentPath);
                    arr.Add(v);
                    _reader.PullWhitespaceAndComments();
                }
                _reader.PullArrayEnd();
                return arr;
            }
            finally
            {
                PopDiagnostics();
            }
        }

        private void IgnoreComma()
        {
            // optional end of value
            if (_reader.IsComma()) 
                _reader.PullComma();
        }
    }
}
