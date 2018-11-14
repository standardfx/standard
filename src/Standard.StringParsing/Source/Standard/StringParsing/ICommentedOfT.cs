using System.Collections.Generic;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents a value with leading and trailing comments.
    /// </summary>
    /// <typeparam name="T">The type of the matched result.</typeparam>
    /// <remarks>
    /// The <see cref="ICommented{T}"/> represents the case where a value has comments immediately before and after itself. 
    /// </remarks>
    public interface ICommented<T>
    {
        /// <summary>
        /// Gets the leading comments.
        /// </summary>
        /// <value>The leading comments.</value>
        IEnumerable<string> LeadingComments { get; }

        /// <summary>
        /// Gets the resulting value.
        /// </summary>
        /// <value>The resulting value.</value>
        T Value { get; }

        /// <summary>
        /// Gets the trailing comments.
        /// </summary>
        /// <value>The trailing comments.</value>
        IEnumerable<string> TrailingComments { get; }
    }
}