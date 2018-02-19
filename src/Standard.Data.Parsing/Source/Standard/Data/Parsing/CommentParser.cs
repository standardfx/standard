using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard.Data.Parsing
{
    /// <summary>
    /// Constructs customizable comment parsers.
    /// </summary>
    public class CommentParser : IComment
    {
        ///<summary>
        ///Single-line comment header.
        ///</summary>
        public string Single { get; set; }

        ///<summary>
        ///Newline character preference.
        ///</summary>
        public string NewLine { get; set; }

        ///<summary>
        ///Multi-line comment opener.
        ///</summary>
        public string MultiOpen { get; set; }

        ///<summary>
        ///Multi-line comment closer.
        ///</summary>
        public string MultiClose { get; set; }

        /// <summary>
        /// Initializes a comment parser with C-style headers and Windows newlines.
        /// </summary>
        public CommentParser()
        {
            Single = "//";
            MultiOpen = "/*";
            MultiClose = "*/";
            NewLine = "\n";
        }

        /// <summary>
        /// Initializes a comment parser with custom multi-line headers and newline characters.
        /// Single-line headers are made null, it is assumed they would not be used.
        /// </summary>
        public CommentParser(string multiOpen, string multiClose, string newLine = "\n")
        {
            Single = null;
            MultiOpen = multiOpen;
            MultiClose = multiClose;
            NewLine = newLine;
        }

        /// <summary>
        /// Initializes a Comment with custom headers and newline characters.
        /// </summary>
        public CommentParser(string single, string multiOpen, string multiClose, string newLine = "\n")
        {
            Single = single;
            MultiOpen = multiOpen;
            MultiClose = multiClose;
            NewLine = newLine;
        }

        /// <summary>
        /// Parse a single-line comment.
        /// </summary>
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
            private set { }
        }

        /// <summary>
        /// Parse a multi-line comment.
        /// </summary>
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
            private set { }
        }

        /// <summary>
        /// Parse a comment.
        /// </summary>
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
            private set { }
        }
    }
}
