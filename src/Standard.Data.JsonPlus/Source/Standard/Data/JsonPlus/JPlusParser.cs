using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class contains methods used to parse Json+ source text.
    /// </summary>
    public class JPlusParser
    {
        private readonly List<JPlusSubstitution> _substitutions = new List<JPlusSubstitution>();
        private JPlusTokenizer _reader;
        private JPlusValue _root;
        private Func<string, JPlusRoot> _includeCallback;
        private Stack<string> _diagnosticsStack = new Stack<string>();

        private void PushDiagnostics(string message)
        {
            _diagnosticsStack.Push(message);
        }

        private void PopDiagnostics()
        {
            _diagnosticsStack.Pop();
        }

        /// <summary>
        /// Returns the current path.
        /// </summary>
        /// <returns>The current transversal path.</returns>
        public string GetDiagnosticsStackTrace()
        {
            string currentPath = string.Join(string.Empty, _diagnosticsStack.Reverse());
            return string.Format("Path: {0}", currentPath);
        }

        /// <summary>
        /// Parses the supplied Json+ source text into a <see cref="JPlusRoot"/> element.
        /// </summary>
        /// <param name="source">The Json+ source text.</param>
        /// <param name="includeCallback">Callback used to resolve the `include` keyword.</param>
        /// <exception cref="JPlusParserException">An unresolved substitution is encountered, or the end of the file has been reached while trying to read a value.</exception>
        /// <returns>A <see cref="JPlusRoot"/> element from the Json+ source in <paramref name="source"/>.</returns>
        public static JPlusRoot Parse(string source, Func<string, JPlusRoot> includeCallback)
        {
            return new JPlusParser().ParseText(source, includeCallback);
        }

        private JPlusRoot ParseText(string source, Func<string, JPlusRoot> includeCallback)
        {
            _includeCallback = includeCallback;
            _root = new JPlusValue();
            _reader = new JPlusTokenizer(source);
            _reader.PullWhitespaceAndComments();
            ParseObject(_root, true, string.Empty);

            JPlusContext c = new JPlusContext(new JPlusRoot(_root, Enumerable.Empty<JPlusSubstitution>()));
            foreach (JPlusSubstitution sub in _substitutions)
            {
                JPlusValue res = c.GetValue(sub.Path);
                if (res == null)
                    throw new JPlusParserException(string.Format(RS.ErrUnresolvedSubstitution, sub.Path));

                sub.ResolvedValue = res;
            }
            return new JPlusRoot(_root, _substitutions);
        }

        private void ParseObject(JPlusValue owner, bool root, string currentPath)
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
                    owner.NewValue(new JPlusObject());
                }

                JPlusObject currentObject = owner.GetObject();

                while (!_reader.EOF)
                {
                    JPlusToken t = _reader.PullNext();
                    switch (t.Type)
                    {
                        case TokenType.Include:
                            JPlusRoot included = _includeCallback(t.Value);
                            IEnumerable<JPlusSubstitution> substitutions = included.Substitutions;
                            foreach (JPlusSubstitution substitution in substitutions)
                            {
                                //fixup the substitution, add the current path as a prefix to the substitution path
                                substitution.Path = currentPath + "." + substitution.Path;
                            }
                            _substitutions.AddRange(substitutions);
                            JPlusObject otherObj = included.Value.GetObject();
                            owner.GetObject().Merge(otherObj);

                            break;

                        case TokenType.EOF:
                            if (!string.IsNullOrEmpty(currentPath))
                                throw new JPlusParserException(string.Format(RS.ErrObjectUnexpectedEOF, GetDiagnosticsStackTrace()));

                            break;

                        case TokenType.Key:
                            JPlusValue value = currentObject.GetOrCreateKey(t.Value);

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

        private void ParseKeyContent(JPlusValue value, string currentPath)
        {
            try
            {
                string last = new JPlusPath(currentPath).AsArray().Last();
                PushDiagnostics(string.Format("{0} = ", last));
                while (!_reader.EOF)
                {
                    JPlusToken t = _reader.PullNext();
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
        /// to the parent element specified.
        /// </summary>
        /// <param name="owner">The element to append the next token.</param>
        /// <param name="currentPath">Current AST path.</param>
        /// <exception cref="Exception">End of file reached while trying to read a value.</exception>
        public void ParseValue(JPlusValue owner, string currentPath)
        {
            if (_reader.EOF)
                throw new JPlusParserException(RS.ErrUnexpectedEOF);

            _reader.PullWhitespaceAndComments();
            int start = _reader.Index;
            try
            {
                while (_reader.IsValue())
                {
                    JPlusToken t = _reader.PullValue();

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
                            JPlusArray arr = ParseArray(currentPath);
                            owner.AppendValue(arr);
                            break;

                        case TokenType.Substitute:
                            JPlusSubstitution sub = ParseSubstitution(t.Value);
                            _substitutions.Add(sub);
                            owner.AppendValue(sub);
                            break;
                    }

                    if (_reader.IsSpaceOrTab())
                        ParseTrailingWhitespace(owner);
                }

                IgnoreComma();
            }
            catch(JPlusTokenizerException tokenizerException)
            {
                throw new JPlusParserException(string.Format("{0}\r{1}", tokenizerException.Message, GetDiagnosticsStackTrace()),tokenizerException);
            }
            finally
            {
                // no value was found, tokenizer is still at the same position
                if (_reader.Index == start)
                    throw new JPlusParserException(string.Format(RS.ErrBadSyntax, _reader.GetHelpTextAtIndex(start), GetDiagnosticsStackTrace()));
            }
        }

        private void ParseTrailingWhitespace(JPlusValue owner)
        {
            JPlusToken ws = _reader.PullSpaceOrTab();

            //single line ws should be included if string concat
            if (ws.Value.Length > 0)
            {
                LiteralString wsLit = new LiteralString { Value = ws.Value, };
                owner.AppendValue(wsLit);
            }
        }

        private static JPlusSubstitution ParseSubstitution(string value)
        {
            return new JPlusSubstitution(value);
        }

        /// <summary>
        /// Retrieves the next array token from the tokenizer.
        /// </summary>
        /// <returns>An array of elements retrieved from the token.</returns>
        public JPlusArray ParseArray(string currentPath)
        {
            try
            {
                PushDiagnostics("[");

                JPlusArray arr = new JPlusArray();
                while (!_reader.EOF && !_reader.IsArrayEnd())
                {
                    JPlusValue v = new JPlusValue();
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
