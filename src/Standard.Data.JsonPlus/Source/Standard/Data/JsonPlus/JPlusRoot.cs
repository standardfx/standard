using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// The root element in the abstract syntax tree.
    /// </summary>
    public class JPlusRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusRoot"/> class.
        /// </summary>
        protected JPlusRoot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusRoot"/> class.
        /// </summary>
        /// <param name="value">The value to associate with this element.</param>
        /// <param name="substitutions">An enumeration of substitutions to associate with this element.</param>
        public JPlusRoot(JPlusValue value, IEnumerable<JPlusSubstitution> substitutions)
        {
            Value = value;
            Substitutions = substitutions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusRoot"/> class.
        /// </summary>
        /// <param name="value">The value to associate with this element.</param>
        public JPlusRoot(JPlusValue value)
        {
            Value = value;
            Substitutions = Enumerable.Empty<JPlusSubstitution>();
        }

        /// <summary>
        /// Gets the value associated with this element.
        /// </summary>
        public JPlusValue Value 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets an enumeration of substitutions associated with this element.
        /// </summary>
        public IEnumerable<JPlusSubstitution> Substitutions 
        { 
            get; 
            private set; 
        }
    }
}
