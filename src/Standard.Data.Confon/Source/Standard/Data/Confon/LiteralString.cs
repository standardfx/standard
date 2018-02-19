using System;
using System.Collections.Generic;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class represents a string literal element in a Confon string.
    /// </summary>
    /// <remarks>
    /// <code>
    /// root {  
    ///   child {
    ///     text = "This is a literal text"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public class LiteralString : IConfonElement
    {
        /// <summary>
        /// Gets or sets the value of this element.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Determines whether this element is a string.
        /// </summary>
        /// <returns><c>true</c></returns>
        public bool IsString()
        {
            return true;
        }

        /// <summary>
        /// Retrieves the string representation of this element.
        /// </summary>
        /// <returns>The value of this element.</returns>
        public string GetString()
        {
            return Value;
        }

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns><c>false</c></returns>
        public bool IsArray()
        {
            return false;
        }

        /// <summary>
        /// Retrieves a list of elements associated with this element.
        /// </summary>
        /// <returns>
        /// A list of elements associated with this element.
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// This element is a string literal. It is not an array.
        /// Therefore this method will throw an exception.
        /// </exception>
        public IList<ConfonValue> GetArray()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the string representation of this element.
        /// </summary>
        /// <returns>The value of this element.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}

