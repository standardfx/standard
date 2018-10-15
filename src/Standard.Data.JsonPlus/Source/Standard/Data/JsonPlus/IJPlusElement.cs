using System.Collections.Generic;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// A marker interface to make it easier to retrieve elements for substitution operations.
    /// </summary>
    public interface IJPlusObjectCandidate
    {
        /// <summary>
        /// Determines whether this element is a <see cref="JPlusObject"/>.
        /// </summary>
        /// <returns>`true` if this element is a <see cref="JPlusObject"/>. Otherwise, `false`.</returns>
        bool IsObject();

        /// <summary>
        /// Return this element as a <see cref="JPlusObject"/>.
        /// </summary>
        /// <returns>The <see cref="JPlusObject"/> value of this element.</returns>
        JPlusObject GetObject();
    }

    /// <summary>
    /// This interface defines the contract for a Json+ element. All elements must implement this interface.
    /// </summary>
    public interface IJPlusElement
    {
        /// <summary>
        /// Determines whether this element is a <see cref="string"/>.
        /// </summary>
        /// <returns>`true` if this element is a <see cref="string"/>. Otherwise, `false`.</returns>
        bool IsString();

        /// <summary>
        /// Return this element as a <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> value of this element.</returns>
        string GetString();

        /// <summary>
        /// Determines whether this element is an enumerable collection of <see cref="JPlusValue"/>.
        /// </summary>
        /// <returns>`true` if this element is an array. Otherwise, `false`.</returns>
        bool IsArray();

        /// <summary>
        /// Returns this element as an enumerable list of <see cref="JPlusValue"/>.
        /// </summary>
        /// <returns>An enumerable list of <see cref="JPlusValue"/>.</returns>
        IList<JPlusValue> GetArray();
    }
}
