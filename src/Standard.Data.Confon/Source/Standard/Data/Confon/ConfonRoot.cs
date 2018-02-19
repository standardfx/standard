using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class represents the root element in a Confon string.
    /// </summary>
    public class ConfonRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonRoot"/> class.
        /// </summary>
        protected ConfonRoot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonRoot"/> class.
        /// </summary>
        /// <param name="value">The value to associate with this element.</param>
        /// <param name="substitutions">An enumeration of substitutions to associate with this element.</param>
        public ConfonRoot(ConfonValue value, IEnumerable<ConfonSubstitution> substitutions)
        {
            Value = value;
            Substitutions = substitutions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonRoot"/> class.
        /// </summary>
        /// <param name="value">The value to associate with this element.</param>
        public ConfonRoot(ConfonValue value)
        {
            Value = value;
            Substitutions = Enumerable.Empty<ConfonSubstitution>();
        }

        /// <summary>
        /// Retrieves the value associated with this element.
        /// </summary>
        public ConfonValue Value 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Retrieves an enumeration of substitutions associated with this element.
        /// </summary>
        public IEnumerable<ConfonSubstitution> Substitutions 
        { 
            get; 
            private set; 
        }
    }
}
