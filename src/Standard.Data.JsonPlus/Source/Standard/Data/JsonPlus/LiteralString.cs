using System;
using System.Collections.Generic;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class represents a string literal element in a Json+ string.
    /// </summary>
    /// <remarks>
    /// ```json+
    /// root {  
    ///   child {
    ///     text = "This is a literal text"
    ///   }
    /// }
    /// ```
    /// </remarks>
    public class LiteralString : IJPlusElement
    {
        /// <summary>
        /// Gets or sets the value of this element.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Determines whether this element is a <see cref="string"/>.
        /// </summary>
        /// <returns>This method will always return `true`.</returns>
        public bool IsString()
        {
            return true;
        }

        /// <summary>
        /// Returns the value of this element as a <see cref="string"/>.
        /// </summary>
        /// <returns>The value of this element as a <see cref="string"/>.</returns>
        public string GetString()
        {
            return Value;
        }

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns>This method will always return `false`.</returns>
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
        /// This element is a string literal and not an array. Therefore this method will always throw an exception.
        /// </exception>
        public IList<JPlusValue> GetArray()
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

