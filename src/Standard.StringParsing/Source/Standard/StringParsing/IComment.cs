using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents a parser specialized at parsing comments in source code.
    /// </summary>
    public interface IComment
    {
        /// <summary>
        /// Gets or sets the string that denotes the beginning of a single-line comment.
        /// </summary>
        string Single { get; set; }

        /// <summary>
        /// Gets or sets the string used as a line delimiter (new line).
        /// </summary>
        string NewLine { get; set; }

        /// <summary>
        /// Gets or sets the string that denotes the beginning of a multi-line comment block.
        /// </summary>
        string MultiOpen { get; set; }

        /// <summary>
        /// Gets or sets the string that denotes the end of a multi-line comment block.
        /// </summary>
        string MultiClose { get; set; }

        /// <summary>
        /// Returns a <see cref="Parser{T}"/> specialized at parsing a single line comment.
        /// </summary>
        /// <value>A <see cref="Parser{T}"/> specialized at parsing a single line comment.</value>
        Parser<string> SingleLineComment { get; }

        /// <summary>
        /// Returns a <see cref="Parser{T}"/> specialized at parsing a multi-line comment block.
        /// </summary>
        /// <value>A <see cref="Parser{T}"/> specialized at parsing a multi-line comment block.</value>
        Parser<string> MultiLineComment { get; }

        /// <summary>
        /// Returns a <see cref="Parser{T}"/> designed to parse a single line comment and/or a multi-line comment block.
        /// </summary>
        /// <value>A <see cref="Parser{T}"/> designed to parser a single line comment and/or a multi-line comment block.</value>
        Parser<string> AnyComment { get; }
    }
}
