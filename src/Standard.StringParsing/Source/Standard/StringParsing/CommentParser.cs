using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard.StringParsing
{
    /// <summary>
    /// A parser specialized at parsing comments in source code.
    /// </summary>
    public class CommentParser : IComment
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CommentParser"/> with C-style headers and Unix newlines.
        /// </summary>
        public CommentParser()
        {
            Single = "//";
            MultiOpen = "/*";
            MultiClose = "*/";
            NewLine = "\n";
        }

        /// <summary>
        /// Initializes a a new instance of <see cref="CommentParser"/> with custom headers and newline characters.
        /// </summary>
        /// <param name="single">The string that denotes the beginning of a single-line comment.</param>
        /// <param name="multiOpen">The string that denotes the beginning of a multi-line comment block.</param>
        /// <param name="multiClose">The string that denotes the end of a multi-line comment block.</param>
        public CommentParser(string single, string multiOpen, string multiClose)
            : this(single, multiOpen, multiClose, "\n")
        {
        }

        /// <summary>
        /// Initializes a a new instance of <see cref="CommentParser"/> with custom headers and newline characters.
        /// </summary>
        /// <param name="single">The string that denotes the beginning of a single-line comment.</param>
        /// <param name="multiOpen">The string that denotes the beginning of a multi-line comment block.</param>
        /// <param name="multiClose">The string that denotes the end of a multi-line comment block.</param>
        /// <param name="newLine">The string used as a line delimiter (new line).</param>
        public CommentParser(string single, string multiOpen, string multiClose, string newLine)
        {
            Single = single;
            MultiOpen = multiOpen;
            MultiClose = multiClose;
            NewLine = newLine;
        }

        /// <see cref="IComment.Single"/>
        public string Single { get; set; }

        /// <see cref="IComment.NewLine"/>
        public string NewLine { get; set; }

        /// <see cref="IComment.MultiOpen"/>
        public string MultiOpen { get; set; }

        /// <see cref="IComment.MultiClose"/>
        public string MultiClose { get; set; }

        /// <see cref="IComment.SingleLineComment"/>
        public Parser<string> SingleLineComment
        {
            get
            {
                if (Single == null)
                    throw new ParseException(RS.SingleLineCommentDisabled);

                return 
                    from first in Parse.String(Single)
                    from rest in Parse.CharExcept(NewLine).Many().Text()
                    select rest;
            }
            private set 
            {
            }
        }

        /// <see cref="IComment.MultiLineComment"/>
        public Parser<string> MultiLineComment
        {
            get
            {
                if (MultiOpen == null)
                    throw new ParseException(string.Format(RS.MultiLineCommentDisabled, "MultiOpen"));
                else if (MultiClose == null)
                    throw new ParseException(string.Format(RS.MultiLineCommentDisabled, "MultiClose"));

                return 
                    from first in Parse.String(MultiOpen)
                    from rest in Parse.AnyChar
                        .Until(Parse.String(MultiClose)).Text()
                    select rest;
            }
            private set 
            {
            }
        }

        /// <see cref="IComment.AnyComment"/>
        /// <remarks>
        /// This property will return a parser that attempts to parse a single line comment, and fallback to parse a multi-line comment block.
        /// However, if <see cref="Single"/> is `null`, a multi-line comment will be returned, and vice verse.
        /// </remarks>
        public Parser<string> AnyComment
        {
            get
            {
                if (Single != null && MultiOpen != null && MultiClose != null)
                    return SingleLineComment.Or(MultiLineComment);                    
                else if (Single != null && (MultiOpen == null || MultiClose == null))
                    return SingleLineComment;
                else if (Single == null && (MultiOpen != null && MultiClose != null))
                    return MultiLineComment;
                else 
                    throw new ParseException(RS.ParseCommentFailure);
            }
            private set 
            {
            }
        }
    }
}
