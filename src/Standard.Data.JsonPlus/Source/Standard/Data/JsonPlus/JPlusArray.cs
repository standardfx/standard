using System;
using System.Collections.Generic;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class represents an array element.
    /// </summary>
    /// <remarks>
    /// <code>
    /// root {
    ///     items = [
    ///       "1",
    ///       "2"]
    /// }
    /// </code>
    /// </remarks>
    public class JPlusArray : List<JPlusValue>, IJPlusElement
    {
        /// <summary>
        /// Determines whether this element is a string.
        /// </summary>
        /// <returns>This method will always return `false`.</returns>
        public bool IsString()
        {
            return false;
        }

        /// <summary>
        /// Returns this element as a string.
        /// </summary>
        /// <exception cref="NotImplementedException">This element is an array and not a string. Therefore, calling this method will always result in an exception.</exception>
        /// <returns>Calling this method will result in an <see cref="NotImplementedException"/>.</returns>
        public string GetString()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns>This method will always return `true`.</returns>
        public bool IsArray()
        {
            return true;
        }

        /// <see cref="IJPlusElement.GetArray()"/>
        public IList<JPlusValue> GetArray()
        {
            return this;
        }

        /// <summary>
        /// Returns a string representation of this element.
        /// </summary>
        /// <returns>A string representation of this element.</returns>
        public override string ToString()
        {
            return "[" + string.Join(",", this) + "]";
        }
    }
}
