using System.Collections.Generic;

namespace Standard.Data.Confon
{
    /// <summary>
    /// Marker interface to make it easier to retrieve Confon for substitutions.
    /// </summary>
    public interface IPossibleConfonObject
    {
        /// <summary>
        /// Determines whether this element is a Confon object.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this element is a Confon object; otherwise <c>false</c>
        /// </returns>
        bool IsObject();

        /// <summary>
        /// Retrieves the Confon object representation of this element.
        /// </summary>
        /// <returns>
        /// The Confon object representation of this element.
        /// </returns>
        ConfonObject GetObject();
    }

    /// <summary>
    /// This interface defines the contract needed to implement a Confon element.
    /// </summary>
    public interface IConfonElement
    {
        /// <summary>
        /// Determines whether this element is a string.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this element is a string; otherwise <c>false</c>
        /// </returns>
        bool IsString();

        /// <summary>
        /// Retrieves the string representation of this element.
        /// </summary>
        /// <returns>
        /// The string representation of this element.
        /// </returns>
        string GetString();

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this element is aan array; otherwise <c>false</c>
        /// </returns>
        bool IsArray();

        /// <summary>
        /// Retrieves a list of elements associated with this element.
        /// </summary>
        /// <returns>
        /// A list of elements associated with this element.
        /// </returns>
        IList<ConfonValue> GetArray();
    }
}

