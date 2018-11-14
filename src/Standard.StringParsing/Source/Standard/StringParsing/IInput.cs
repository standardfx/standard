using System;
using System.Collections.Generic;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents an input that wraps a <see cref="string"/> for parsing.
    /// </summary>
    /// <remarks>
    /// The <see cref="IInput"/> interface is used to represent information on the current cursor position within a <see cref="string"/> 
    /// that is being parsed.
    /// </remarks>
    public interface IInput : IEquatable<IInput>
    {
        /// <summary>
        /// Advances the input.
        /// </summary>
        /// <exception cref="InvalidOperationException">The input is already at the end of the source.</exception>
        /// <returns>
        /// A new <see cref="IInput" /> representing information on the next cursor position.
        /// </returns>
        IInput Advance();

        /// <summary>
        /// Gets the entire source.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets the current <see cref="char"/>.
        /// </summary>
        char Current { get; }

        /// <summary>
        /// Gets a value indicating whether the parser has reached the end of the source.
        /// </summary>
        bool AtEnd { get; }

        /// <summary>
        /// Gets the current position within <see cref="Source"/>. This is the zero-based index position within the source string.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Gets the memos used by this <see cref="IInput"/> instance.
        /// </summary>
        IDictionary<object, object> Memos { get; }
    }
}
