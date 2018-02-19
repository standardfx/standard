using System.Collections.Generic;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class represents a substitution element in a Confon string.
    /// </summary>
    /// <remarks>
    /// <code>
    /// foo {  
    ///   defaultInstances = 10
    ///   deployment{
    ///     /user/time{
    ///       nr-of-instances = $defaultInstances
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public class ConfonSubstitution : IConfonElement, IPossibleConfonObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonSubstitution"/> class.
        /// </summary>
        protected ConfonSubstitution()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonSubstitution" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ConfonSubstitution(string path)
        {
            Path = path;
        }

        /// <summary>
        /// The full path to the value which should substitute this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The evaluated value from the Path property
        /// </summary>
        public ConfonValue ResolvedValue { get; set; }

        /// <summary>
        /// Determines whether this element is a string.
        /// </summary>
        /// <returns><c>true</c> if this element is a string; otherwise <c>false</c></returns>
        public bool IsString()
        {
            return ResolvedValue.IsString();
        }

        /// <summary>
        /// Retrieves the string representation of this element.
        /// </summary>
        /// <returns>The string representation of this element.</returns>
        public string GetString()
        {
            return ResolvedValue.GetString();
        }

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns><c>true</c> if this element is aan array; otherwise <c>false</c></returns>
        public bool IsArray()
        {
            return ResolvedValue.IsArray();
        }

        /// <summary>
        /// Retrieves a list of elements associated with this element.
        /// </summary>
        /// <returns>A list of elements associated with this element.</returns>
        public IList<ConfonValue> GetArray()
        {
            return ResolvedValue.GetArray();
        }

        /// <summary>
        /// Determines whether this element is a Confon object.
        /// </summary>
        /// <returns><c>true</c> if this element is a Confon object; otherwise <c>false</c></returns>
        public bool IsObject()
        {
            return ResolvedValue != null && ResolvedValue.IsObject();
        }

        /// <summary>
        /// Retrieves the Confon object representation of this element.
        /// </summary>
        /// <returns>The Confon object representation of this element.</returns>
        public ConfonObject GetObject()
        {
            return ResolvedValue.GetObject();
        }
    }
}

